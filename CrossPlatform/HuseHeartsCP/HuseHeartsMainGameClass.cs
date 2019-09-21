using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HuseHeartsCP
{
    [SingletonGame]
    public class HuseHeartsMainGameClass : TrickGameClass<EnumSuitList, HuseHeartsCardInformation,
        HuseHeartsPlayerItem, HuseHeartsSaveInfo>, IMiscDataNM, IStartNewGame, IFinishStart
    {
        private IAdvancedTrickProcesses? _aTrick;
        public new TrickAreaCP? TrickArea1;
        public HuseHeartsMainGameClass(IGamePackageResolver container) : base(container) { }

        private HuseHeartsViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<HuseHeartsViewModel>();
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            if (TrickArea1 == null)
                TrickArea1 = MainContainer.Resolve<TrickAreaCP>();
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
            FinalLoading(); //i think
        }
        private void FinalLoading()
        {
            _thisMod!.Dummy1!.HandList.ReplaceRange(SaveRoot!.DummyList); //i think
            _thisMod.Blind1!.HandList.ReplaceRange(SaveRoot.BlindList);
            if (_thisMod.Blind1.HandList.Count != 4)
                throw new BasicBlankException("Blind must have 4 cards for finalloading");
            if (SaveRoot.WhoWinsBlind > 0)
            {
                var TempPlayer = PlayerList![SaveRoot.WhoWinsBlind];
                if (TempPlayer.PlayerCategory != EnumPlayerCategory.Self)
                {
                    _thisMod.Blind1.Visible = false;
                    return;
                }
            }

            _thisMod.Blind1.Visible = true;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadTrickAreas();
            _aTrick = MainContainer.Resolve<IAdvancedTrickProcesses>(); //decided to be here.
            IsLoaded = true; //i think needs to be here.
        }
        protected override void LoadVM()
        {
            base.LoadVM();
            SaveRoot!.LoadMod(_thisMod!);
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SaveRoot!.GameStatus == EnumStatus.ShootMoon)
                throw new BasicBlankException("I doubt the computer would ever shoot the moon.  If so; rethinking is required");
            if (SaveRoot.GameStatus == EnumStatus.Passing)
                throw new BasicBlankException("The computer should have already passed the cards");
            CustomBasicList<int> moveList;
            if (TrickArea1!.FromDummy == false)
                moveList = SingleInfo!.MainHandList.Where(Items => IsValidMove(Items.Deck)).Select(Items => Items.Deck).ToCustomBasicList();
            else
                moveList = _thisMod!.Dummy1!.HandList.Where(Items => IsValidMove(Items.Deck)).Select(Items => Items.Deck).ToCustomBasicList();
            await PlayCardAsync(moveList.First()); //for now.
        }
        protected override bool CanEndTurnToContinueTrick
        {
            get
            {
                if (TrickArea1!.FromDummy == true)
                    return false;
                else
                    return true;
            }
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            LoadVM();
            if (TrickArea1 == null)
                TrickArea1 = MainContainer.Resolve<TrickAreaCP>();
            SaveRoot!.RoundNumber++;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentScore = 0;
                thisPlayer.TricksWon = 0;
                thisPlayer.HadPoints = false;
            });
            SaveRoot.TrickStatus = EnumTrickStatus.FirstTrick;
            SaveRoot.GameStatus = EnumStatus.Passing;
            SaveRoot.WhoWinsBlind = 0;
            SaveRoot.WhoLeadsTrick = 0;
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            SaveRoot!.BlindList = _thisMod!.Deck1!.DrawSeveralCards(4);
            SaveRoot.BlindList.ForEach(ThisCard => ThisCard.IsUnknown = true); //this is very important too obviously.
            FinalLoading();
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.GameStatus != EnumStatus.Passing)
            {
                await base.ContinueTurnAsync();
                if (_thisMod!.PlayerHand1!.IsEnabled == false && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    _thisMod.PlayerHand1.ReportCanExecuteChange(); //try this way.
                return;
            }
            SingleInfo = PlayerList!.GetSelf();
            WhoTurn = SingleInfo.Id;
            this.ShowTurn();
            if (SingleInfo.CardsPassed.Count == 0)
            {
                await base.ContinueTurnAsync();
                return;
            }
            await SaveStateAsync();
            if (ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Rethink because computer would have pass cards");
            ThisCheck!.IsEnabled = true;
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "takepointsaway":
                    await GiveSelfMinusPointsAsync();
                    break;
                case "givepointseverybodyelse":
                    await GiveEverybodyElsePointsAsync();
                    break;
                case "passcards":
                    var thisList = await content.GetSavedIntegerListAsync();
                    await CardsPassedAsync(thisList);
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task PopulateSaveRootAsync()
        {
            await base.PopulateSaveRootAsync();
            SaveRoot!.DummyList = _thisMod!.Dummy1!.HandList.ToObservableDeckDict();
            SaveRoot.BlindList = _thisMod.Blind1!.HandList.ToRegularDeckDict();
            if (SaveRoot.DummyList.Count != _thisMod.Dummy1.HandList.Count)
                throw new BasicBlankException("Dummy does not reconcile when populating the saveroot");
            if (SaveRoot.BlindList.Count != 4)
                throw new BasicBlankException("The blind must have 4 cards");
            //hints i have to rethink.
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
        Task IFinishStart.FinishStartAsync()
        {
            if (SaveRoot!.GameStatus == EnumStatus.Passing)
            {
                SingleInfo = PlayerList!.GetSelf();
                WhoTurn = SingleInfo.Id; //i think needs to be this way because its passing.
            }
            return Task.CompletedTask;
        }
        private int WhoWonTrick(DeckObservableDict<HuseHeartsCardInformation> thisCol)
        {
            var leadCard = thisCol.First();
            var tempList = thisCol.ToRegularDeckDict();
            tempList.RemoveSpecificItem(leadCard);
            if (tempList.Any(Items => Items.Suit == leadCard.Suit && Items.Value > leadCard.Value) == false)
                return leadCard.Player;
            return WhoTurn;
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            if (trickList.Any(Items => Items.Suit == EnumSuitList.Hearts))
                SaveRoot.TrickStatus = EnumTrickStatus.SuitBroken;
            else if (SaveRoot.TrickStatus == EnumTrickStatus.FirstTrick)
                SaveRoot.TrickStatus = EnumTrickStatus.NoSuit;

            int wins = WhoWonTrick(trickList);
            HuseHeartsPlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            WhoTurn = wins; //most of the time, whoever wins leads again.
            SingleInfo = PlayerList.GetWhoPlayer();
            int Points = trickList.Sum(Items => Items.HeartPoints);
            if (SingleInfo.HadPoints == false)
            {
                SingleInfo.HadPoints = trickList.Any(Items => Items.ContainPoints == true);
            }
            if (Points != 0)
            {
                SingleInfo.CurrentScore += Points;
                if (SaveRoot.WhoWinsBlind == 0 && SingleInfo.HadPoints == true)
                {
                    SaveRoot.WhoWinsBlind = wins;
                    if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
                        _thisMod!.Blind1!.Visible = false;
                    _thisMod!.Blind1!.HandList.MakeAllObjectsKnown();
                }
            }
            await _aTrick!.AnimateWinAsync(wins);
            if (SingleInfo.MainHandList.Count == 0)
            {
                await EndRoundAsync();
                return; //most of the time its in rounds.
            }
            _thisMod!.PlayerHand1!.EndTurn();
            SaveRoot.WhoLeadsTrick = WhoTurn;
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            await StartNewTurnAsync(); //hopefully this simple.
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.PreviousScore = 0; //hopefully, this simple.
            });
            SaveRoot!.RoundNumber = 0;
            return Task.CompletedTask;
        }
        public override async Task EndRoundAsync()
        {
            WhoTurn = SaveRoot!.WhoWinsBlind;
            if (SaveRoot.WhoWinsBlind == 0)
                throw new BasicBlankException("Somebody has to win the blind eventually");
            if (SaveRoot.BlindList.Count != 4)
                throw new BasicBlankException("The blind has to have 4 cards at the end of the round");
            if (PlayerList.Count() != 2)
                throw new BasicBlankException("Huse Hearts Is A 2 Player Game");
            int points = SaveRoot.BlindList.Sum(Items => Items.HeartPoints);
            SingleInfo!.CurrentScore += points;
            var FirstPlayer = PlayerList.First();
            var LastPlayer = PlayerList.Last();
            FirstPlayer.PreviousScore = FirstPlayer.CurrentScore;
            LastPlayer.PreviousScore = LastPlayer.CurrentScore;
            if (FirstPlayer.PreviousScore + LastPlayer.PreviousScore != 16)
                throw new BasicBlankException("The total points for the players has to be 16 points");
            int Shoots = PlayerList!.WhoShotMoon();
            if (Shoots == 0)
            {
                await FinishEndAsync();
                return;
            }
            WhoTurn = Shoots;
            SaveRoot.GameStatus = EnumStatus.ShootMoon;
            SingleInfo = PlayerList!.GetWhoPlayer();
            await _thisMod!.ShowGameMessageAsync($"{SingleInfo.NickName}  has shot the moon.  The player needs to choose to either give 26 points to the other player or take 26 points off their own score");
            await StartNewTurnAsync(); //hopefully this simple.
        }
        private async Task FinishEndAsync()
        {
            SaveRoot!.GameStatus = EnumStatus.EndRound;
            PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore += thisPlayer.CurrentScore);
            if (CanEndGame() == true)
            {
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        private void GetWinningPlayer()
        {
            SingleInfo = PlayerList.OrderBy(items => items.TotalScore).Take(1).Single();
        }
        private bool CanEndGame()
        {
            if (SaveRoot!.RoundNumber > 17)
            {
                GetWinningPlayer();
                return true;
            }
            if (PlayerList.Any(items => items.TotalScore >= 100))
            {
                GetWinningPlayer();
                return true;
            }
            return false;
        }
        internal async Task GiveSelfMinusPointsAsync()
        {
            SingleInfo!.CurrentScore -= 52;
            SingleInfo.PreviousScore = SingleInfo.CurrentScore;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
                await _thisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} chose to give him/herself -26 points");
            await FinishEndAsync();
        }
        internal async Task GiveEverybodyElsePointsAsync()
        {
            int newPlayer;
            if (WhoTurn == 1)
                newPlayer = 2;
            else
                newPlayer = 1;
            var thisPlayer = PlayerList![newPlayer];
            SingleInfo!.CurrentScore -= 26;
            SingleInfo.PreviousScore = SingleInfo.CurrentScore;
            thisPlayer.CurrentScore += 26;
            thisPlayer.PreviousScore = thisPlayer.CurrentScore;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
                await _thisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} chose to give the other player 26 points");
            await FinishEndAsync();
        }
        private void TransferToPassed(CustomBasicList<int> thisList, HuseHeartsPlayerItem thisPlayer)
        {
            thisPlayer.CardsPassed = thisList;
            thisList.ForEach(index => thisPlayer.MainHandList.RemoveObjectByDeck(index));
        }
        private async Task ComputerPassCardsAsync()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
                throw new BasicBlankException("Must be computer player when the computer is passing cards");
            var thisList = SingleInfo.MainHandList.GetRandomList(false, 3);
            await CardsPassedAsync(thisList.Select(items => items.Deck).ToCustomBasicList());
        }
        public async Task CardsPassedAsync(CustomBasicList<int> thisList)
        {
            if (thisList.Count != 3)
                throw new BasicBlankException("Must pass 3 cards");
            SingleInfo = PlayerList!.GetSelf();
            HuseHeartsPlayerItem newPlayer;
            if (SingleInfo.Id == 1)
                newPlayer = PlayerList[2];
            else
                newPlayer = PlayerList[1];
            if (ThisData!.MultiPlayer == false)
            {
                if (SingleInfo.CardsPassed.Count == 0)
                {
                    TransferToPassed(thisList, SingleInfo);
                    SaveRoot!.GameStatus = EnumStatus.WaitingForPlayers;
                    SingleInfo = newPlayer;
                    await ComputerPassCardsAsync();
                    return;
                }
                if (newPlayer.PlayerCategory != EnumPlayerCategory.Computer)
                    throw new BasicBlankException("Must show computer player");
                TransferToPassed(thisList, newPlayer);
                await AfterCardsPassedAsync();
                return;
            }
            if (SingleInfo.CardsPassed.Count == 0)
            {
                TransferToPassed(thisList, SingleInfo);
                SaveRoot!.GameStatus = EnumStatus.WaitingForPlayers;
                ThisCheck!.IsEnabled = true; //now wait for others.
                return;
            }
            TransferToPassed(thisList, newPlayer);
            await AfterCardsPassedAsync();
        }
        private async Task AfterCardsPassedAsync()
        {
            if (PlayerList.Any(Items => Items.CardsPassed.Count != 3))
                throw new BasicBlankException("All players must have passed 3 cards");
            var firstPlayer = PlayerList.First();
            var secondPlayer = PlayerList.Last();
            HuseHeartsCardInformation ThisCard;
            DeckRegularDict<HuseHeartsCardInformation> firstList = new DeckRegularDict<HuseHeartsCardInformation>();
            DeckRegularDict<HuseHeartsCardInformation> secondList = new DeckRegularDict<HuseHeartsCardInformation>();
            firstPlayer.CardsPassed.ForEach(deck =>
            {
                ThisCard = DeckList!.GetSpecificItem(deck);
                ThisCard.IsSelected = false;
                ThisCard.Drew = true;
                secondList.Add(ThisCard);
            });
            secondPlayer.MainHandList.AddRange(secondList);
            secondPlayer.CardsPassed.ForEach(deck =>
            {
                ThisCard = DeckList!.GetSpecificItem(deck);
                ThisCard.IsSelected = false;
                ThisCard.Drew = true;
                firstList.Add(ThisCard);
            });
            firstPlayer.MainHandList.AddRange(firstList);
            SortCards(); //try this way.
            if (firstPlayer.MainHandList.Count != 16 || secondPlayer.MainHandList.Count != 16)
                throw new BasicBlankException("All players must have 16 cards in hand after passing");
            firstPlayer.CardsPassed.Clear();
            secondPlayer.CardsPassed.Clear();
            SaveRoot!.WhoLeadsTrick = WhoLeadsFirstTrick();
            SaveRoot.GameStatus = EnumStatus.Normal;
            TrickArea1!.ClearBoard();
            WhoTurn = SaveRoot.WhoLeadsTrick;
            SingleInfo = PlayerList!.GetWhoPlayer();
            await StartNewTurnAsync(); //hopefully this simple.
        }
        private int WhoLeadsFirstTrick()
        {
            var tempList = PlayerList!.CardsFromAllPlayers<HuseHeartsCardInformation, HuseHeartsPlayerItem>();
            int tempDeck;
            if (tempList.Any(Items => Items.Suit == EnumSuitList.Clubs))
                tempDeck = tempList.Where(Items => Items.Suit == EnumSuitList.Clubs).OrderBy(Items => Items.Value).First().Deck;
            else
                tempDeck = tempList.Where(Items => Items.Suit == EnumSuitList.Diamonds).OrderBy(Items => Items.Value).First().Deck;
            return PlayerList!.WhoHasCardFromDeck<HuseHeartsCardInformation, HuseHeartsPlayerItem>(tempDeck);
        }
    }
}