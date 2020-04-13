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
using CribbagePatienceCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using CribbagePatienceCP.Data;
using CribbagePatienceCP.EventModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGameFrameworkLibrary.BasicEventModels;
namespace CribbagePatienceCP.ViewModels
{
    [InstanceGame]
    public class CribbagePatienceMainViewModel : Screen, 
        IBasicEnableProcess,
        IBlankGameVM, 
        IAggregatorContainer,
        IHandle<HandScoresEventModel>

    {
        private readonly IEventAggregator _aggregator;
        private readonly CribbagePatienceMainGameClass _mainGame;

        public PileObservable<CribbageCard>? StartPile;
        //i have 2 choices:
        //1.  a separate view model for the crib and hand.
        //2.  have a visible property for the observable hand.
        public HandObservable<CribbageCard> TempCrib;
        public HandObservable<CribbageCard> Hand1;
        public CustomBasicList<ScoreHandCP>? HandScores;
        public ScoreSummaryCP? Scores;

        public ScoreHandCP GetScoreHand(EnumHandCategory thisCategory)
        {
            return HandScores.Single(items => items.HandCategory == thisCategory);
        }

        public (int row, int column) GetRowColumn(EnumHandCategory thisCategory)
        {
            var hand = GetScoreHand(thisCategory);
            return hand.GetRowColumn();
        }

        public DeckObservablePile<CribbageCard> DeckPile { get; set; }
        public bool CanCrib => Hand1.Visible;
        [Command(EnumCommandCategory.Plain)]
        public async Task CribAsync()
        {
            int manys = Hand1.HowManySelectedObjects;
            if (manys == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose cards");
                return;
            }
            if (manys != 2)
            {
                await UIPlatform.ShowMessageAsync("Must choose 2 cards for crib");
                return;
            }
            var thisList = Hand1.ListSelectedObjects(true);
            if (Hand1.HandList.Count == 6)
                throw new BasicBlankException("Did not remove cards before starting to put to crib");
            _mainGame.RemoveTempCards(thisList);
            _mainGame.CardsToCrib(thisList);
            if (DeckPile.IsEndOfDeck())
            {
                await UIPlatform.ShowMessageAsync("Game Over.  Check Results");
                await _mainGame.SendGameOverAsync();
                //NewGameVisible = true; //i think.
                //_mainGame.GameGoing = false;
            }
        }
        //serious bug because somehow on tablets, it thinks you can continue when you can't.  was with a method function.
        public bool CanContinue()
        {
            if (Hand1.Visible == true)
            {
                return false;
            }
            return !DeckPile.IsEndOfDeck();
        }

        [Command(EnumCommandCategory.Plain)]
        public void Continue()
        {
            _mainGame.NewRound();
        }

        public CribbagePatienceMainViewModel(IEventAggregator aggregator, 
            CommandContainer commandContainer,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            CommandContainer = commandContainer;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
            DeckPile = resolver.ReplaceObject<DeckObservablePile<CribbageCard>>();
            DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
            DeckPile.NeverAutoDisable = false;
            DeckPile.SendEnableProcesses(this, () =>
            {
                return false;
            });

            Hand1 = new HandObservable<CribbageCard>(commandContainer);
            Hand1.Visible = false; //has to be proven true.
            Hand1.Maximum = 6;
            Hand1.AutoSelect = HandObservable<CribbageCard>.EnumAutoType.SelectAsMany;



            _mainGame = resolver.ReplaceObject<CribbagePatienceMainGameClass>(); //hopefully this works.  means you have to really rethink.
            _aggregator.Subscribe(this);
            _mainGame._saveRoot.HandScores = new CustomBasicList<ScoreHandCP>();
            3.Times(x =>
            {
                var tempHand = new ScoreHandCP();
                tempHand.HandCategory = (EnumHandCategory)x;
                _mainGame._saveRoot.HandScores.Add(tempHand);
            });
            StartPile = new PileObservable<CribbageCard>(_aggregator, CommandContainer);
            StartPile.Text = "Start Card";
            StartPile.CurrentOnly = true;
            StartPile.SendEnableProcesses(this, () => false);
            Scores = new ScoreSummaryCP();
            TempCrib = new HandObservable<CribbageCard>(CommandContainer);
            TempCrib.Visible = false;
            TempCrib.Text = "Crib So Far";
            TempCrib.Maximum = 4;

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

        void IHandle<HandScoresEventModel>.Handle(HandScoresEventModel message)
        {
            HandScores = message.HandScores;
        }
    }
}
