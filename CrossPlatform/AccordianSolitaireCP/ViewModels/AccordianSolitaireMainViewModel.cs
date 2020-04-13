using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Messenging;
using AccordianSolitaireCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using AccordianSolitaireCP.Data;
using AccordianSolitaireCP.EventModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;

namespace AccordianSolitaireCP.ViewModels
{
    [InstanceGame]
    public class AccordianSolitaireMainViewModel : Screen, 
        IBasicEnableProcess,
        IBlankGameVM, 
        IAggregatorContainer,
        IHandle<ScoreEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly AccordianSolitaireMainGameClass _mainGame;

        private int _score;

        public int Score
        {
            get { return _score; }
            set
            {
                if (SetProperty(ref _score, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


        public DeckObservablePile<AccordianSolitaireCardInfo> DeckPile { get; set; }
        public GameBoard GameBoard1;

        public void UnselectAll()
        {
            GameBoard1.UnselectAllObjects();
        }

        public AccordianSolitaireMainViewModel(IEventAggregator aggregator, 
            CommandContainer commandContainer,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            CommandContainer = commandContainer;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
            DeckPile = resolver.ReplaceObject<DeckObservablePile<AccordianSolitaireCardInfo>>();
            DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
            DeckPile.NeverAutoDisable = true;
            DeckPile.SendEnableProcesses(this, () =>
            {
                return false;
            });
            GameBoard1 = new GameBoard(commandContainer, aggregator);
            GameBoard1.ObjectClickedAsync += GameBoard1_ObjectClickedAsync;
            _aggregator.Subscribe(this);
            _mainGame = resolver.ReplaceObject<AccordianSolitaireMainGameClass>(); //hopefully this works.  means you have to really rethink.
        }
        private async Task GameBoard1_ObjectClickedAsync(AccordianSolitaireCardInfo card, int index)
        {
            if (index == -1)
                throw new BasicBlankException("Index cannot be -1.  Rethink");
            if (GameBoard1!.IsCardSelected(card) == false)
            {
                GameBoard1.SelectUnselectCard(card);
                return;
            }
            if (GameBoard1.IsValidMove(card) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            GameBoard1.MakeMove(card);
            if (Score == 52)
                await _mainGame!.ShowWinAsync();
        }
        private async Task DeckPile_DeckClickedAsync()
        {
            //if we click on deck, will do code for this.
            await Task.CompletedTask;
        }

        private async void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer.IsExecuting)
                return;
            //code to run when its not busy.

            if (_mainGame.GameGoing)
                await _mainGame.SaveStateAsync();
        }
        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }
        public CommandContainer CommandContainer { get; set; }

        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        public bool CanEnableBasics()
        {
            return true; //because maybe you can't enable it.
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            await _mainGame.NewGameAsync(DeckPile, GameBoard1);
        }

        void IHandle<ScoreEventModel>.Handle(ScoreEventModel message)
        {
            Score = message.Score;
        }
    }
}
