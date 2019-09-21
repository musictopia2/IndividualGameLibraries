using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CaliforniaJackCP
{
    [SingletonGame]
    public class CaliforniaJackMainGameClass : TrickGameClass<EnumSuitList, CaliforniaJackCardInformation,
        CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>, IStartNewGame
    {
        private IAdvancedTrickProcesses? _aTrick;
        public CaliforniaJackMainGameClass(IGamePackageResolver container) : base(container) { }
        private CaliforniaJackViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<CaliforniaJackViewModel>();
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
                thisPlayer.Points = 0; //i think.
            });
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
        private void CardScoring()
        {
            DeckList!.ForEach(thisCard =>
            {
                if (thisCard.Value == EnumCardValueList.HighAce || thisCard.Value == EnumCardValueList.LowAce)
                    thisCard.Points = 4;
                else if (thisCard.Value == EnumCardValueList.Ten)
                    thisCard.Points = 10;
                else if (thisCard.Value == EnumCardValueList.Jack)
                    thisCard.Points = 1;
                else if (thisCard.Value == EnumCardValueList.Queen)
                    thisCard.Points = 2;
                else if (thisCard.Value == EnumCardValueList.King)
                    thisCard.Points = 3;
                else
                    thisCard.Points = 0;
            });
        }
        private void CalculatePointsSoFar(DeckObservableDict<CaliforniaJackCardInformation> trickList, int wins)
        {
            int points = trickList.Count(thisCard => thisCard.Suit == SaveRoot!.TrumpSuit && (thisCard.Value == EnumCardValueList.Two
                || thisCard.Value == EnumCardValueList.Jack ||
                thisCard.Value == EnumCardValueList.HighAce || thisCard.Value == EnumCardValueList.LowAce));
            PlayerList![wins].Points += points;
        }
        private int WhoWonTrick(DeckObservableDict<CaliforniaJackCardInformation> thisCol)
        {
            CaliforniaJackCardInformation leadCard = thisCol.First();
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
            CaliforniaJackPlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            CalculatePointsSoFar(trickList, wins);
            await _aTrick!.AnimateWinAsync(wins);
            if (SingleInfo!.MainHandList.Count == 0)
            {
                await EndRoundAsync();
                return; //most of the time its in rounds.
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
                CaliforniaJackCardInformation thisCard;
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
                CaliforniaJackPlayerItem tempPlayer;
                if (WhoTurn == 1)
                    tempPlayer = PlayerList[2];
                else
                    tempPlayer = PlayerList[1];
                thisCard = _thisMod.Deck1.DrawCard();
                tempPlayer.MainHandList.Add(thisCard);
                if (tempPlayer.PlayerCategory == EnumPlayerCategory.Self)
                {
                    thisCard.Drew = true;
                    SortCards(); //i think.
                }
            }
            await StartNewTurnAsync(); //hopefully this simple.
        }
        private int _game;
        public override async Task EndRoundAsync()
        {
            CardScoring();
            _game = WhoReceivesLastPoint();
            if (_game > 0)
                PlayerList![_game].Points++;
            AddToTotals();
            if (PlayerList.Any(items => items.TotalScore >= 10))
            {
                await GameOverAsync();
                return;
            }
            this.RoundOverNext();
        }
        private int WhoReceivesLastPoint()
        {
            int output = 0;
            int x;
            int previousPoints = 0;
            int points;
            for (x = 1; x <= 2; x++)
            {
                points = PointsReceived(x);
                if (points > previousPoints)
                {
                    previousPoints = points;
                    output = x;
                }
                else if ((points == previousPoints) & (previousPoints > 0))
                    output = 0;// i think at this point, it will be 0.
            }
            return output;
        }
        private int PointsReceived(int player)
        {
            return DeckList.Where(items => items.Player == player).Sum(Items => Items.Points);
        }
        private RandomGenerator? _rs;
        private int WhoWonGame()
        {
            int firsts;
            int Seconds;
            firsts = PlayerList.First().TotalScore;
            Seconds = PlayerList.Last().TotalScore; // because its 2 player only.
            if (firsts > Seconds)
                return 1;
            if (Seconds > firsts)
                return 2;
            if (firsts == Seconds)
            {
                var thisPlayer = PlayerList![_game]; // not 0 based anymore.
                thisPlayer.Points -= 1; // somehow reduced by 1
                thisPlayer.TotalScore -= 1; // has to reduce by one i think
                if (_game == 1)
                    return 2;
                return 1;
            }
            if (_rs == null)
                _rs = MainContainer.Resolve<RandomGenerator>();
            return _rs.GetRandomNumber(2);
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList![WhoWonGame()];
            await this.ShowWinAsync();
        }
        private void AddToTotals()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore += thisPlayer.Points;
            });
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.Points = 0;
                thisPlayer.TricksWon = 0;
            });
            return Task.CompletedTask;
        }
    }
}