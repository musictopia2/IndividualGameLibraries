using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace RageCardGameCP
{
    [SingletonGame]
    public class RageCardGameMainGameClass : TrickGameClass<EnumColor, RageCardGameCardInformation,
        RageCardGamePlayerItem, RageCardGameSaveInfo>, IMiscDataNM, IStartNewGame
    {
        private IAdvancedTrickProcesses? _aTrick;
        public new SpecificTrickAreaViewModel? TrickArea1;
        public async Task ChooseColorAsync()
        {
            SaveRoot!.Status = EnumStatus.ChooseColor;
            await ContinueTurnAsync();
        }
        public async Task ColorChosenAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("color", ThisMod!.ColorChosen);
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
            {
                ThisMod!.ColorVM!.ChooseItem(ThisMod.ColorChosen);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.5);
            }
            ThisMod!.ColorVisible = false;
            await TrickArea1!.ColorChosenAsync(ThisMod.ColorChosen); //hopefully this is it.
        }
        public async Task ProcessBidAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("bid", ThisMod!.BidAmount);
            SingleInfo.BidAmount = ThisMod!.BidAmount;
            ThisMod.BiddingVisible = false;
            await EndTurnAsync();
        }
        public RageCardGameMainGameClass(IGamePackageResolver container) : base(container) { }

        internal RageCardGameViewModel? ThisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<RageCardGameViewModel>();
        }

        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            if (TrickArea1 == null)
                TrickArea1 = MainContainer.Resolve<SpecificTrickAreaViewModel>();
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
            if (SaveRoot!.Status == EnumStatus.Regular)
                TrickArea1.Visible = true;
            else
                TrickArea1.Visible = false;
            RevealBids();
        }
        private void RevealBids()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.RevealBid = thisPlayer.PlayerCategory == EnumPlayerCategory.Self || thisPlayer.MainHandList.Count == 1;
            });
        }
        protected override bool CanEnableTrickAreas => SaveRoot!.Status == EnumStatus.Regular;
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
                await Delay!.DelaySeconds(.75);
            if (SaveRoot!.Status == EnumStatus.Bidding)
            {
                ThisMod!.BidAmount = ThisMod.Bid1!.NumberToChoose();
                await ProcessBidAsync();
                return;
            }
            if (SaveRoot.Status == EnumStatus.ChooseColor)
            {
                ThisMod!.ColorChosen = ThisMod.ColorVM!.ItemToChoose();
                await ColorChosenAsync();
                return;
            }
            var thisList = SingleInfo!.MainHandList.Where(items => IsValidMove(items.Deck)).Select(Items => Items.Deck).ToCustomBasicList();
            await PlayCardAsync(thisList.GetRandomItem());
        }
        public override Task ContinueTurnAsync()
        {
            if (SaveRoot!.Status == EnumStatus.ChooseColor)
                TrickArea1!.LoadColorLists();
            else if (SaveRoot.Status == EnumStatus.Bidding)
                ThisMod!.LoadBiddingScreen();
            return base.ContinueTurnAsync();
        }
        private RandomGenerator? _rs;
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (_rs == null)
                _rs = MainContainer.Resolve<RandomGenerator>();
            int ask1 = _rs.GetRandomNumber(6);
            SaveRoot!.TrumpSuit = (EnumColor)ask1;
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (isBeginning == true)
                SaveRoot!.CardsToPassOut = 11;
            else
                SaveRoot!.CardsToPassOut--;
            if (TrickArea1 == null)
                TrickArea1 = MainContainer.Resolve<SpecificTrickAreaViewModel>();
            LoadControls();
            LoadVM();
            TrickArea1.Visible = false;
            SaveRoot.CardList.Clear();
            SaveRoot.Status = EnumStatus.Bidding;
            RevealBids();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.BidAmount = -1;
                thisPlayer.TricksWon = 0;
            });
            return base.StartSetUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "color":
                    ThisMod!.ColorChosen = await js.DeserializeObjectAsync<EnumColor>(content);
                    await ColorChosenAsync();
                    break;
                case "bid":
                    ThisMod!.BidAmount = int.Parse(content);
                    await ProcessBidAsync();
                    break;
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
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            if (SaveRoot!.Status == EnumStatus.Bidding && WhoTurn == WhoStarts)
            {
                SaveRoot.Status = EnumStatus.Regular;
                ThisMod.BiddingVisible = false;
                await StartNewTrickAsync();
                return;
            }
            await StartNewTurnAsync();
        }
        public override bool IsValidMove(int deck)
        {
            if (deck == 0)
                throw new BasicBlankException("Deck cannot be 0 for IsValidMove");
            var thisList = SaveRoot!.TrickList;
            if (thisList.Count == 0)
                return true;
            var LeadCard = thisList.FirstOrDefault(Items => Items.Color != EnumColor.None);
            if (LeadCard == null)
                return true;
            var cardPlayed = DeckList!.GetSpecificItem(deck);
            if (cardPlayed.Color == LeadCard.Color)
                return true;
            return base.IsValidMove(deck); //try this way.
        }
        private int WhoWonTrick(DeckObservableDict<RageCardGameCardInformation> thisCol)
        {
            if (thisCol.Any(Items => Items.SpecialType == EnumSpecialType.Wild && Items.Color == EnumColor.None))
                throw new BasicBlankException("Must have chosen a color for the wild");
            var tempList = thisCol.Where(items => items.Color != EnumColor.None).ToRegularDeckDict();
            if (tempList.Count == 0)
                return thisCol.First().Player;
            var leadColor = tempList.First().Color;
            var trumpList = tempList.Where(items => items.Color == SaveRoot!.TrumpSuit).ToRegularDeckDict();
            if (trumpList.Count == 0)
                return tempList.Where(items => items.Color == leadColor).OrderByDescending(Items => Items.Value).Take(1).Single().Player;
            return trumpList.OrderByDescending(Items => Items.Value).Take(1).Single().Player;
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            RageCardGamePlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            await _aTrick!.AnimateWinAsync(wins);
            trickList.ForEach(thisCard =>
            {
                thisCard.Player = wins;
                SaveRoot.CardList.Add(thisCard);
            });
            if (SingleInfo!.MainHandList.Count == 0)
            {
                await EndRoundAsync();
                return; //most of the time its in rounds.
            }
            ThisMod!.PlayerHand1!.EndTurn();
            RevealBids();
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
            SaveRoot!.CardsToPassOut = 11;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.ScoreGame = 0;
                thisPlayer.ScoreRound = 0;
                thisPlayer.CorrectlyBidded = 0;
            });
            return Task.CompletedTask;
        }
        private void CalculateScore()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.ScoreRound = 0;
                int extras;
                if (thisPlayer.TricksWon == thisPlayer.BidAmount)
                {
                    thisPlayer.CorrectlyBidded++;
                    if (thisPlayer.BidAmount == 0)
                        extras = 5;
                    else
                        extras = 10;
                }
                else
                    extras = 0;
                thisPlayer.ScoreRound += extras;
                int score = SaveRoot!.CardList.Where(items => items.Player == thisPlayer.Id).Sum(Items => Items.GetPoints);
                thisPlayer.ScoreRound += score;
                if (thisPlayer.BidAmount > 0)
                {
                    int wons;
                    if (thisPlayer.TricksWon == SaveRoot.CardsToPassOut)
                        wons = SaveRoot.CardsToPassOut * 2;
                    else
                        wons = thisPlayer.TricksWon;
                    thisPlayer.ScoreRound += wons;
                }
                thisPlayer.ScoreGame += thisPlayer.ScoreRound;
            });
        }
        public override async Task EndRoundAsync()
        {
            CalculateScore();
            if (SaveRoot!.CardsToPassOut == 1)
            {
                await GameOverAsync();
                return;
            }
            this.RoundOverNext();
        }
        private async Task GameOverAsync()
        {
            SetWinPlayer();
            await ShowWinAsync();
        }
        private void SetWinPlayer()
        {
            var PossibleItem = PlayerList.OrderByDescending(Items => Items.ScoreGame).Take(1).Single();
            if (PlayerList.Count(Items => Items.ScoreGame == PossibleItem.ScoreGame) == 1)
            {
                SingleInfo = PossibleItem;
                return;
            }
            var TieList = PlayerList.Where(Items => Items.ScoreGame == PossibleItem.ScoreGame).ToCustomBasicList();
            if (TieList.Count == 0)
                throw new BasicBlankException("No Tie");
            SingleInfo = TieList.OrderByDescending(Items => Items.CorrectlyBidded).First();
        }
    }
}