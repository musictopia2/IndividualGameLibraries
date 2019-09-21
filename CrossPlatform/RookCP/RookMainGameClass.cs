using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.ColorCards;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace RookCP
{
    [SingletonGame]
    public class RookMainGameClass : TrickGameClass<EnumColorTypes, RookCardInformation,
        RookPlayerItem, RookSaveInfo>, IMiscDataNM, IStartNewGame
    {
        private IAdvancedTrickProcesses? _aTrick;
        public RookMainGameClass(IGamePackageResolver container) : base(container) { }

        internal RookViewModel? ThisMod;
        public new TrickAreaCP? TrickArea1;
        private bool _showedOnce = false;
        private bool _isLoaded;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<RookViewModel>();
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            _showedOnce = SaveRoot!.GameStatus != EnumStatusList.Bidding;
            SaveRoot.LoadMod(ThisMod!);
            ThisMod!.Dummy1!.HandList.ReplaceRange(SaveRoot.DummyList);
            if (SaveRoot.GameStatus == EnumStatusList.SelectNest)
            {
                ThisMod.PlayerHand1!.AutoSelect = HandViewModel<RookCardInformation>.EnumAutoType.SelectAsMany;
                ThisMod.Status = "Choose the 5 cards to get rid of";
            }
            else
            {
                ThisMod!.PlayerHand1!.AutoSelect = HandViewModel<RookCardInformation>.EnumAutoType.SelectOneOnly;
            }
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
            ThisMod.ShowVisibleChange();//try this.
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.DummyList = ThisMod!.Dummy1!.HandList.ToObservableDeckDict();
            return base.PopulateSaveRootAsync();
        }
        private void LoadControls()
        {
            if (_isLoaded == true)
                return;
            LoadTrickAreas();
            _aTrick = MainContainer.Resolve<IAdvancedTrickProcesses>(); //decided to be here.
            TrickArea1 = MainContainer.Resolve<TrickAreaCP>();
            if (PlayerList.Count() == 2)
                ThisMod!.Dummy1!.FirstDummy();
            _isLoaded = true; //i think needs to be here.
        }
        public override async Task ContinueTurnAsync()
        {
            if (_showedOnce == false)
            {
                await BeginBiddingAsync();
                return;
            }
            await base.ContinueTurnAsync();
        }
        protected override bool CanEnableTrickAreas => SaveRoot!.GameStatus == EnumStatusList.Normal;
        private async Task BeginBiddingAsync()
        {
            _showedOnce = true;
            if (SaveRoot!.HighestBidder == 0)
                throw new BasicBlankException("The highest bidder cannot be 0");
            ThisMod!.PopulateBids();
            ThisMod.CanPass = await CanPassAsync();
            await StartNewTurnAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.25);
            if (SaveRoot!.GameStatus == EnumStatusList.Bidding)
            {
                await ComputerAI.CardToBidAsync(this);
                if (ThisMod!.BidChosen == -1)
                {
                    await PassBidAsync();
                    return;
                }
                await ProcessBidAsync();
                return;
            }
            if (SaveRoot.GameStatus == EnumStatusList.ChooseTrump)
            {
                ComputerAI.ColorToCall(this);
                await ProcessTrumpAsync();
                return;
            }
            if (SaveRoot.GameStatus == EnumStatusList.SelectNest)
            {
                await ProcessNestAsync(ComputerAI.CardsToRemove(this));
                return;
            }
            await PlayCardAsync(ComputerAI.CardToPlay(this));
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            _showedOnce = false;
            LoadControls();
            LoadVM();
            if (isBeginning)
                SaveRoot!.LoadMod(ThisMod!);
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.Pass = false;
                thisPlayer.IsDummy = false;
                thisPlayer.BidAmount = 0;
                thisPlayer.TricksWon = 0;
            });
            SaveRoot!.HighestBidder = 35;
            SaveRoot.WonSoFar = 0;
            SaveRoot.TrumpSuit = EnumColorTypes.None;
            ThisMod!.ColorChosen = EnumColorTypes.None;
            SaveRoot.GameStatus = EnumStatusList.Bidding;
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (PlayerList.Count() == 2)
            {
                DeckRegularDict<RookCardInformation> dummyList = new DeckRegularDict<RookCardInformation>();
                12.Times(x =>
                {
                    dummyList.Add(ThisMod!.Deck1!.DrawCard());
                });
                ThisMod!.Dummy1!.LoadDummyCards(dummyList, this);
            }
            SaveRoot!.NestList.Clear();
            SaveRoot.CardList.Clear();
            5.Times(x =>
            {
                SaveRoot.NestList.Add(ThisMod!.Deck1!.DrawCard());
            });
            ThisMod!.ResetTrumps();
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                //put in cases here.
                case "pass":
                    await PassBidAsync();
                    return;
                case "bid":
                    ThisMod!.BidChosen = int.Parse(content);
                    await ProcessBidAsync();
                    return;
                case "colorselected":
                    ThisMod!.ColorChosen = await js.DeserializeObjectAsync<EnumColorTypes>(content);
                    await ProcessTrumpAsync();
                    return;
                case "nestlist":
                    var thisList = await js.DeserializeObjectAsync<DeckRegularDict<RookCardInformation>>(content);
                    await ProcessNestAsync(thisList);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
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
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.

            if (SaveRoot!.DummyPlay)
            {
                SaveRoot.DummyPlay = false;
                WhoTurn = SaveRoot.WonSoFar;
                await StartNewTurnAsync();
                return;
            }
            if (WhoTurn == SaveRoot.WonSoFar)
            {
                WhoTurn = await PlayerList.CalculateWhoTurnAsync();
                await StartNewTurnAsync();
                return;
            }
            SaveRoot.DummyPlay = true;
            if (PlayerList.Count() == 2)
            {
                await StartNewTurnAsync();
                return;
            }
            SingleInfo = PlayerList.Where(items => items.IsDummy == true).Single();
            WhoTurn = SingleInfo.Id;
            await StartNewTurnAsync();
        }
        private int WhoWonTrick(DeckObservableDict<RookCardInformation> thisCol, out bool isDummy)
        {
            var tempCol = thisCol.Where(items => items.Color == SaveRoot!.TrumpSuit).OrderByDescending(items => items.CardValue).ToRegularDeckDict();
            if (tempCol.Count > 0)
            {
                isDummy = tempCol.First().IsDummy;
                return tempCol.First().Player;
            }
            var thisCard = thisCol.First();
            var leadColor = thisCard.Color;
            tempCol = thisCol.Where(items => items.Color == leadColor).OrderByDescending(items => items.CardValue).ToRegularDeckDict();
            isDummy = tempCol.First().IsDummy;
            return tempCol.First().Player;
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList, out bool dummys);
            RookPlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            await TrickArea1!.AnimateWinAsync(wins, dummys);
            trickList.ForEach(thisCard =>
            {
                RookCardInformation newCard = new RookCardInformation();
                newCard.Populate(thisCard.Deck);
                newCard.Player = wins;
                newCard.IsDummy = thisCard.IsDummy;
                SaveRoot.CardList.Add(newCard);
            });
            if (SingleInfo!.MainHandList.Count == 0)
            {
                if (SaveRoot.NestList.Count != 8)
                    throw new BasicBlankException("Must have 8 cards for the nest list");
                SaveRoot.NestList.ForEach(thisCard =>
                {
                    thisCard.Player = wins;
                    thisCard.IsDummy = false;
                    SaveRoot.CardList.Add(thisCard);
                });
                await EndRoundAsync();
                return; //most of the time its in rounds.
            }
            ThisMod!.PlayerHand1!.EndTurn();
            WhoTurn = wins; //most of the time, whoever wins leads again.
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            await StartNewTurnAsync(); //hopefully this simple.
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.CurrentScore = 0;
            });
            return Task.CompletedTask;
        }
        private async Task EndBiddingAsync()
        {
            ThisMod!.CanPass = false;
            WhoTurn = SaveRoot!.WonSoFar;
            TrickArea1!.NewRound(); //i think.
            SaveRoot.GameStatus = EnumStatusList.ChooseTrump;
            await StartNewTurnAsync();
        }
        private int CalculatePoints(int player)
        {
            return SaveRoot!.CardList.Where(items => items.Player == SaveRoot.WonSoFar & player == SaveRoot.WonSoFar | items.Player != SaveRoot.WonSoFar & player != SaveRoot.WonSoFar).Sum(items => items.Points);
        }
        private void Scoring()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                int points = CalculatePoints(thisPlayer.Id);
                if (thisPlayer.Id == SaveRoot!.WonSoFar && points < SaveRoot.HighestBidder)
                    points = SaveRoot.HighestBidder * -1;
                thisPlayer.CurrentScore = points;
                thisPlayer.TotalScore += points;
            });
        }
        private bool CanEndGame()
        {
            if (PlayerList.Any(items => items.TotalScore >= 300))
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
                return true;
            }
            return false;

        }
        public override async Task EndRoundAsync()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.CurrentScore = 0);
            Scoring();
            if (CanEndGame())
            {
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }

        public async Task<bool> CanPassAsync()
        {
            var temps = await PlayerList!.CalculateWhoTurnAsync();
            SingleInfo = PlayerList.GetWhoPlayer();
            if (temps == WhoStarts && SaveRoot!.WonSoFar == 0)
                return false;
            return true;
        }
        public async Task PassBidAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("pass");
            SingleInfo!.Pass = true;
            await ContinueBidProcessAsync();
        }
        private async Task ContinueBidProcessAsync()
        {
            if (SaveRoot!.HighestBidder == 100)
            {
                await EndBiddingAsync();
                return;
            }
            if (SaveRoot.WonSoFar > 0)
            {
                if (PlayerList.Count(items => items.Pass == true) == 1)
                {
                    await EndBiddingAsync();
                    return;
                }
            }
            int olds = WhoTurn;
            do
            {
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                SingleInfo = PlayerList.GetWhoPlayer();
                if (SingleInfo.Pass == false)
                    break;
            } while (true);
            if (WhoTurn == olds)
                throw new BasicBlankException("Cannot be the same player again");
            SingleInfo = PlayerList.GetWhoPlayer();
            ThisMod!.CanPass = await CanPassAsync();
            await BeginBiddingAsync();
        }
        public async Task ProcessBidAsync()
        {
            if (ThisMod!.BidChosen == -1)
                throw new BasicBlankException("The bid amount cannot be -1");
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                ThisMod.Bid1!.SelectNumberValue(ThisMod.BidChosen);
            if (SingleInfo.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("bid", ThisMod.BidChosen);
            SaveRoot!.WonSoFar = WhoTurn;
            SingleInfo.BidAmount = ThisMod.BidChosen;
            SaveRoot.HighestBidder = ThisMod.BidChosen;
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.Id != WhoTurn)
                    thisPlayer.BidAmount = 0; //because somebody else won it.
            });
            ThisMod.ResetBids();
            await ContinueBidProcessAsync();
        }
        public async Task ProcessTrumpAsync()
        {
            SaveRoot!.TrumpSuit = ThisMod!.ColorChosen;
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                ThisMod.Color1!.SelectSpecificItem(ThisMod.ColorChosen);
            if (SingleInfo.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("colorselected", ThisMod.ColorChosen);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            ThisMod.ResetTrumps();
            SaveRoot.GameStatus = EnumStatusList.SelectNest;
            SingleInfo.MainHandList.AddRange(SaveRoot.NestList);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                ThisMod.PlayerHand1!.AutoSelect = HandViewModel<RookCardInformation>.EnumAutoType.SelectAsMany;
                SortCards(); //hopefully this simple.
            }
            if (PlayerList.Count() == 2)
            {
                ThisMod.Dummy1!.MakeAllKnown();
                ThisMod.Dummy1.HandList.Sort(); //hopefully this simple
            }
            ThisMod.Status = "Choose the 5 cards to get rid of";
            await StartNewTurnAsync();
        }
        public async Task ProcessNestAsync(DeckRegularDict<RookCardInformation> thisCol)
        {
            if (thisCol.Count != 5)
                throw new BasicBlankException("The nest must contain exactly 5 cards");
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("nestlist", thisCol);
            SaveRoot!.NestList.ReplaceRange(thisCol);
            var newCol = ThisMod!.Deck1!.DrawSeveralCards(3);
            SaveRoot.NestList.AddRange(newCol);
            SingleInfo!.MainHandList.RemoveSelectedItems(thisCol); //hopefully this works.
            SingleInfo.MainHandList.UnhighlightObjects();
            this.StartingStatus(); //hopefully this simple.
            SaveRoot.GameStatus = EnumStatusList.Normal;
            ThisMod.PlayerHand1!.AutoSelect = HandViewModel<RookCardInformation>.EnumAutoType.SelectOneOnly;
            if (PlayerList.Count() == 3)
            {
                if (SaveRoot.WonSoFar == 1)
                    SingleInfo = PlayerList![3];
                else if (SaveRoot.WonSoFar == 2)
                    SingleInfo = PlayerList![1];
                else
                    SingleInfo = PlayerList![2];
                SingleInfo.IsDummy = true;
                WhoTurn = SingleInfo.Id;
            }
            else
            {
                if (SaveRoot.WonSoFar == 1)
                    WhoTurn = 2;
                else
                    WhoTurn = 1;
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            SaveRoot.DummyPlay = true;
            await StartNewTrickAsync();
        }
    }
}