using BasicGameFramework.CommandClasses; //its common to have command classes.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CribbagePatienceCP
{
    public class CribbagePatienceViewModel : SimpleGameVM, ISoloCardGameVM<CribbageCard>
    {
        public CribbagePatienceViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC)
        {
        }

        public DeckViewModel<CribbageCard>? DeckPile { get; set; }

        public async Task DeckClicked()
        {
            //if we click on deck, will do code for this.
            await Task.CompletedTask;
        }
        private CribbagePatienceGameClass? _mainGame;
        public PileViewModel<CribbageCard>? StartPile;
        public HandViewModel<CribbageCard>? TempCrib;
        public HandViewModel<CribbageCard>? Hand1;
        public CustomBasicList<ScoreHandCP>? HandScores;
        public ScoreSummaryCP? Scores;
        public PlainCommand? CribCommand { get; set; }
        public PlainCommand? ContinueCommand { get; set; }

        public ScoreHandCP GetScoreHand(EnumHandCategory thisCategory)
        {
            return HandScores.Single(items => items.HandCategory == thisCategory);
        }

        public (int Row, int Column) GetRowColumn(EnumHandCategory ThisCategory)
        {
            var ThisHand = GetScoreHand(ThisCategory);
            return ThisHand.GetRowColumn();
        }

        public override void Init()
        {
            _mainGame = MainContainer!.Resolve<CribbagePatienceGameClass>();
            DeckPile = MainContainer.Resolve<DeckViewModel<CribbageCard>>(); //i think.
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            DeckPile.NeverAutoDisable = false; //if it needs to autoreshuffle. do this.  if you don't want to allow reshuffling set to false.
            DeckPile.SendEnableProcesses(this, () =>
            {
                return false; //because we can't enable it this time.

            });
            Hand1 = new HandViewModel<CribbageCard>(this);
            Hand1.Visible = false;
            Hand1.Maximum = 6;
            Hand1.AutoSelect = HandViewModel<CribbageCard>.EnumAutoType.SelectAsMany;
            _mainGame.SaveRoot.HandScores = new CustomBasicList<ScoreHandCP>();
            3.Times(x =>
            {
                var tempHand = new ScoreHandCP();
                tempHand.HandCategory = (EnumHandCategory)x;
                _mainGame.SaveRoot.HandScores.Add(tempHand);
            });
            EventAggregator thisE = MainContainer.Resolve<EventAggregator>();
            StartPile = new PileViewModel<CribbageCard>(thisE, this);
            StartPile.Text = "Start Card";
            StartPile.Visible = true;
            StartPile.CurrentOnly = true;
            StartPile.SendEnableProcesses(this, () => false);
            Scores = new ScoreSummaryCP();
            TempCrib = new HandViewModel<CribbageCard>(this);
            TempCrib.Visible = false;
            TempCrib.Text = "Crib So Far";
            TempCrib.Maximum = 4;
            CribCommand = new PlainCommand(async items =>
            {
                int manys = Hand1.HowManySelectedObjects;
                if (manys == 0)
                {
                    await ShowGameMessageAsync("Must choose cards");
                    return;
                }
                if (manys != 2)
                {
                    await ShowGameMessageAsync("Must choose 2 cards for crib");
                    return;
                }
                var thisList = Hand1.ListSelectedObjects(true);
                if (Hand1.HandList.Count == 6)
                    throw new BasicBlankException("Did not remove cards before starting to put to crib");
                _mainGame.RemoveTempCards(thisList);
                _mainGame.CardsToCrib(thisList);
                if (DeckPile.IsEndOfDeck())
                {
                    NewGameVisible = true; //i think.
                    _mainGame.GameGoing = false;
                }
                //await _mainGame.SaveStateAsync(); //try this way.
            }, items =>
            {
                return Hand1.Visible;
            }, this, CommandContainer);
            ContinueCommand = new PlainCommand(items =>
            {
                _mainGame.NewRound();
            }, items =>
            {
                if (Hand1.Visible == true)
                    return false;
                return !DeckPile.IsEndOfDeck();
            }, this, CommandContainer);
        }

        private async void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return;
            //code to run when its not busy.

            if (_mainGame!.GameGoing)
                await _mainGame.SaveStateAsync();
        }

        public override async Task StartNewGameAsync()
        {
            NewGameVisible = false;
            await _mainGame!.NewGameAsync();
        }
        public override bool CanEnableBasics() //since a person can do new game but still do other things.
        {
            return true;
        }
    }
}