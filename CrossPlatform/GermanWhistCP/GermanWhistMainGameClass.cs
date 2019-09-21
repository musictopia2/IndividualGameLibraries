using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GermanWhistCP
{
    [SingletonGame]
    public class GermanWhistMainGameClass : TrickGameClass<EnumSuitList, GermanWhistCardInformation,
        GermanWhistPlayerItem, GermanWhistSaveInfo>
    {
        private IAdvancedTrickProcesses? _aTrick;
        public GermanWhistMainGameClass(IGamePackageResolver container) : base(container) { }

        private GermanWhistViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<GermanWhistViewModel>();
        }

        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            //anything else needed is here.
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadTrickAreas();
            _aTrick = MainContainer.Resolve<IAdvancedTrickProcesses>(); //decided to be here.
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.DoubleCheck == true)
                return; //so will be stuck.  this way i can test the human player first.
            if (ThisTest.NoAnimations == true)
                await Delay!.DelaySeconds(.75);
            var MoveList = SingleInfo!.MainHandList.Where(ThisCard => IsValidMove(ThisCard.Deck)).Select(Items => Items.Deck).ToCustomBasicList();
            await PlayCardAsync(MoveList.GetRandomItem());
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            LoadVM();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TricksWon = 0;
            });
            SaveRoot!.WasEnd = false;
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            var thisCard = _thisMod!.Deck1!.RevealCard();
            SaveRoot!.TrumpSuit = thisCard.Suit;
            _aTrick!.ClearBoard(); //i think.
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await ContinueTurnAsync();
        }
        private int WhoWonTrick(DeckObservableDict<GermanWhistCardInformation> thisCol)
        {
            GermanWhistCardInformation leadCard = thisCol.First();
            var thisCard = thisCol.Last();
            if (thisCard.Suit == SaveRoot!.TrumpSuit && leadCard.Suit != SaveRoot.TrumpSuit)
                return WhoTurn;
            if (leadCard.Suit == SaveRoot.TrumpSuit && thisCard.Suit != SaveRoot.TrumpSuit)
                return leadCard.Player;
            if (thisCard.Suit == leadCard.Suit)
            {
                if (thisCard.Value > leadCard.Value)
                    return WhoTurn;
            }
            return leadCard.Player;
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            GermanWhistPlayerItem ThisPlayer = PlayerList![wins];
            if (SaveRoot.WasEnd == true)
                ThisPlayer.TricksWon++;
            else if (_thisMod!.Deck1!.IsEndOfDeck() == true)
            {
                SaveRoot.WasEnd = true;
                ThisPlayer.TricksWon++;
            }
            await _aTrick!.AnimateWinAsync(wins);
            if (SingleInfo!.MainHandList.Count == 0)
            {
                await GameOverAsync();
                return;
            }
            _thisMod!.PlayerHand1!.EndTurn();
            WhoTurn = wins; //most of the time, whoever wins leads again.
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            if (_thisMod.Deck1!.IsEndOfDeck() == false)
            {
                GermanWhistCardInformation thisCard;
                thisCard = _thisMod.Deck1.DrawCard();
                SingleInfo = PlayerList!.GetWhoPlayer();
                SingleInfo.MainHandList.Add(thisCard);
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                {
                    thisCard.Drew = true;
                    SortCards(); //i think.
                }
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                GermanWhistPlayerItem TempPlayer;
                if (WhoTurn == 1)
                    TempPlayer = PlayerList[2];
                else
                    TempPlayer = PlayerList[1];
                thisCard = _thisMod.Deck1.DrawCard();
                TempPlayer.MainHandList.Add(thisCard);
                if (TempPlayer.PlayerCategory == EnumPlayerCategory.Self)
                {
                    thisCard.Drew = true;
                    SortCards(); //i think.
                }
            }
            await StartNewTurnAsync(); //hopefully this simple.
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderByDescending(Items => Items.TricksWon).Take(1).Single();
            await this.ShowWinAsync();
        }
    }
}