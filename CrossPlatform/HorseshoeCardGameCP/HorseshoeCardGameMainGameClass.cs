using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HorseshoeCardGameCP
{
    [SingletonGame]
    public class HorseshoeCardGameMainGameClass : TrickGameClass<EnumSuitList, HorseshoeCardGameCardInformation,
        HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo>, IStartNewGame
    {
        private IAdvancedTrickProcesses? _aTrick;
        public HorseshoeCardGameMainGameClass(IGamePackageResolver container) : base(container) { }

        private HorseshoeCardGameViewModel? _thisMod;
        public new TrickAreaCP? TrickArea1;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<HorseshoeCardGameViewModel>();
        }
        public override Task PopulateSaveRootAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.SavedTemp = thisPlayer.TempHand!.CardList.ToRegularDeckDict();
            });
            return base.PopulateSaveRootAsync();
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            if (TrickArea1 == null)
                TrickArea1 = MainContainer.Resolve<TrickAreaCP>();
            _thisMod!.LoadPlayerControls(); //i think
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
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            var moveList = SingleInfo!.MainHandList.Where(items => IsValidMove(items.Deck)).Select(items => items.Deck).ToCustomBasicList();
            var otherList = SingleInfo.TempHand!.ValidCardList;
            otherList.KeepConditionalItems(items => IsValidMove(items.Deck));
            var finList = otherList.Select(items => items.Deck).ToCustomBasicList();
            moveList.AddRange(finList);
            if (moveList.Count == 0)
                throw new BasicBlankException("There must be at least one move for the computer");
            await PlayCardAsync(moveList.GetRandomItem());
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(thisPlayer => thisPlayer.SavedTemp.Clear());
            _thisMod!.LoadPlayerControls();
            LoadVM();
            if (TrickArea1 == null)
                TrickArea1 = MainContainer.Resolve<TrickAreaCP>();
            SaveRoot!.FirstCardPlayed = false;
            PlayerList.ForEach(thisPlayer => thisPlayer.TricksWon = 0);
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                var thisList = _thisMod!.Deck1!.DrawSeveralCards(8);
                thisPlayer.TempHand!.ClearBoard(thisList);
            });
            TrickArea1!.ClearBoard(); //i think i forgot this part.
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
            if (TrickArea1!.DidPlay2Cards == true)
            {
                SaveRoot!.FirstCardPlayed = true;
                await ContinueTurnAsync();
                return;
            }
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await ContinueTurnAsync();
        }
        private int WhoWonTrick(DeckObservableDict<HorseshoeCardGameCardInformation> thisCol)
        {
            if (thisCol.Count != 4)
                throw new BasicBlankException("Must have 4 cards for the trick list to see who won");
            var thisCard = thisCol.First();
            int begins = thisCard.Player;
            EnumCardValueList nums = thisCard.Value;
            if (thisCol.All(Items => Items.Value == nums))
                return begins;
            DeckRegularDict<HorseshoeCardGameCardInformation> playerStarted = new DeckRegularDict<HorseshoeCardGameCardInformation>();
            DeckRegularDict<HorseshoeCardGameCardInformation> otherPlayer = new DeckRegularDict<HorseshoeCardGameCardInformation>();
            playerStarted.Add(thisCol.First());
            playerStarted.Add(thisCol.Last());
            otherPlayer.Add(thisCol[1]);
            otherPlayer.Add(thisCol[2]);
            HorseshoeCardGameCardInformation firstCard = playerStarted.First();
            HorseshoeCardGameCardInformation secondCard = playerStarted.Last();
            EnumSuitList whichSuit = firstCard.Suit;
            EnumCardValueList pairAmount;
            EnumCardValueList highestSuitNumber = EnumCardValueList.None;
            if (firstCard.Value == secondCard.Value)
                pairAmount = firstCard.Value;
            else
            {
                pairAmount = EnumCardValueList.None;
                if (secondCard.Value > firstCard.Value && secondCard.Suit == firstCard.Suit)
                    highestSuitNumber = secondCard.Value;
                else if (secondCard.Suit == firstCard.Suit)
                    highestSuitNumber = firstCard.Value;
            }
            firstCard = otherPlayer.First();
            secondCard = otherPlayer.Last();
            if (firstCard.Value != secondCard.Value && pairAmount > 0)
                return begins;
            if (firstCard.Value == secondCard.Value)
            {
                if (firstCard.Value > pairAmount)
                {
                    if (begins == 1)
                        return 2;
                    return 1;
                }
            }
            if (pairAmount > 0)
                return begins;
            if (firstCard.Suit == whichSuit)
            {
                if (firstCard.Value > highestSuitNumber)
                {
                    if (begins == 1)
                        return 2;
                    return 1;
                }
            }
            if (secondCard.Suit == whichSuit)
            {
                if (secondCard.Value > highestSuitNumber)
                {
                    if (begins == 1)
                        return 2;
                    return 1;
                }
            }
            return begins;
        }
        public override bool IsValidMove(int deck)
        {
            if (deck == 0)
                throw new BasicBlankException("Deck cannot be 0 for isvalidmove");
            var thisList = SaveRoot!.TrickList;
            if (thisList.Count == 0 || thisList.Count == 2)
                return true;
            HorseshoeCardGameCardInformation firstCard;
            if (thisList.Count == 1)
                firstCard = thisList.First();
            else
                firstCard = thisList[2];
            HorseshoeCardGameCardInformation cardPlayed = DeckList!.GetSpecificItem(deck);
            if (cardPlayed.Suit == firstCard.Suit)
                return true;
            var fullList = _thisMod!.GetCurrentHandList();
            if (fullList.Count > 14)
                throw new BasicBlankException("The temp list cannot be more than 14.  Find out what happened");
            return !(fullList.Any(Items => Items.Suit == firstCard.Suit));
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            HorseshoeCardGamePlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            await _aTrick!.AnimateWinAsync(wins);
            if (SingleInfo!.MainHandList.Count == 0 && SingleInfo.TempHand!.IsFinished == true)
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
            SaveRoot!.FirstCardPlayed = false;
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            await StartNewTurnAsync(); //hopefully this simple.
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.PreviousScore = 0;
            });
            return Task.CompletedTask;
        }
        private bool CanEndGame()
        {
            HorseshoeCardGamePlayerItem tempPlayer = PlayerList.Where(items => items.TotalScore >= 11).OrderByDescending(Items => Items.TotalScore).FirstOrDefault();
            if (tempPlayer == null)
                return false;
            SingleInfo = tempPlayer;
            return true;
        }
        private int MostTricks()
        {
            if (PlayerList.First().TricksWon > PlayerList.Last().TricksWon)
                return 1;
            return 2;
        }
        private void CalculateScore()
        {
            int most = MostTricks();
            HorseshoeCardGamePlayerItem thisPlayer = PlayerList![most];
            int wons = thisPlayer.TricksWon;
            int points;
            if (wons == 4)
                points = 2;
            else if (wons == 5)
                points = 3;
            else if (wons == 6)
                points = 5;
            else if (wons == 7)
                points = 7;
            else
                throw new BasicBlankException($"Sorry, no points for {wons}");
            thisPlayer.PreviousScore = points;
            thisPlayer.TotalScore += points;
            if (most == 1)
                most = 2;
            else
                most = 1;
            PlayerList[most].PreviousScore = 0;
        }
        public override async Task EndRoundAsync()
        {
            CalculateScore();
            if (CanEndGame() == true)
            {
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
    }
}