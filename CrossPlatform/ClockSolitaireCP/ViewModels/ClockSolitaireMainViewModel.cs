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
using ClockSolitaireCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using ClockSolitaireCP.Data;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.ClockClasses;
using ClockSolitaireCP.EventModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGameFrameworkLibrary.BasicEventModels;
namespace ClockSolitaireCP.ViewModels
{
    [InstanceGame]
    public class ClockSolitaireMainViewModel : Screen, 
        IBasicEnableProcess,
        IBlankGameVM, 
        IAggregatorContainer,
        IClockVM,
        IHandle<CardsLeftEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ClockSolitaireMainGameClass _mainGame;


        private int _cardsLeft;

        public int CardsLeft
        {
            get { return _cardsLeft; }
            set
            {
                if (SetProperty(ref _cardsLeft, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public DeckObservablePile<SolitaireCard> DeckPile { get; set; }
        public ClockBoard? Clock1;

        public ClockSolitaireMainViewModel(IEventAggregator aggregator, 
            CommandContainer commandContainer,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            CommandContainer = commandContainer;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
            DeckPile = resolver.ReplaceObject<DeckObservablePile<SolitaireCard>>();
            DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
            DeckPile.NeverAutoDisable = true;
            DeckPile.SendEnableProcesses(this, () =>
            {
                return false; //i think.
            });
            _mainGame = resolver.ReplaceObject<ClockSolitaireMainGameClass>(); //hopefully this works.  means you have to really rethink.
            Clock1 = new ClockBoard(this, _mainGame, commandContainer, resolver, _aggregator);
            _aggregator.Subscribe(this);
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

        public CommandContainer CommandContainer { get; set; }

        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        public bool CanEnableBasics()
        {
            return true; //because maybe you can't enable it.
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            await _mainGame.NewGameAsync(this);
            await _aggregator.SendLoadAsync();
        }
        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }

        async Task IClockVM.ClockClickedAsync(int index)
        {
            if (Clock1!.IsValidMove(index) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            Clock1.MakeMove(index);
            if (Clock1.HasWon())
            {
                await _mainGame!.ShowWinAsync();
                return;
            }
            if (Clock1.IsGameOver())
            {
                await _mainGame!.ShowLossAsync();
            }
        }

        void IHandle<CardsLeftEventModel>.Handle(CardsLeftEventModel message)
        {
            CardsLeft = message.CardsLeft;
        }
    }
}
