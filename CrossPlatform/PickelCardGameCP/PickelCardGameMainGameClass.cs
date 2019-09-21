using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace PickelCardGameCP
{
    [SingletonGame]
    public class PickelCardGameMainGameClass : TrickGameClass<EnumSuitList, PickelCardGameCardInformation,
        PickelCardGamePlayerItem, PickelCardGameSaveInfo>, IMiscDataNM, IStartNewGame
    {
        public override bool CanMakeMainOptionsVisibleAtBeginning => CanEnableTrickAreas;
        private IAdvancedTrickProcesses? _aTrick;
        public PickelCardGameMainGameClass(IGamePackageResolver container) : base(container) { }
        internal PickelCardGameViewModel? ThisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<PickelCardGameViewModel>();
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            //anything else needed is here.
            await base.FinishGetSavedAsync();
            if (SaveRoot!.GameStatus == EnumStatusList.Bidding)
                ThisMod!.PopulateBids();
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
                await Delay!.DelaySeconds(.25);
            if (SaveRoot!.GameStatus == EnumStatusList.Bidding)
            {
                int bids = ComputerAI.HowManyToBid(this);
                if (bids == -1)
                {
                    await PassBidAsync();
                    return;
                }
                if (bids == 0)
                    throw new BasicBlankException("The computer cannot choose 0 bids.");
                var suit = ComputerAI.SuitToCall(this);
                ThisMod!.SelectBidAndSuit(bids, suit);
                await ProcessBidAsync();
                return;
            }
            int Deck = ComputerAI.CardToPlay(this);
            await PlayCardAsync(Deck);
        }
        protected override bool CanEnableTrickAreas => SaveRoot!.GameStatus != EnumStatusList.Bidding;
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            LoadVM();
            SaveRoot!.GameStatus = EnumStatusList.Bidding;
            SaveRoot.CardList.Clear();
            SaveRoot.WonSoFar = 0;
            if (PlayerList.Count() == 2)
                SaveRoot.HighestBid = 3;
            else if (PlayerList.Count() == 3)
                SaveRoot.HighestBid = 2;
            else
                throw new BasicBlankException("Only 2 or 3 players are supported");
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.SuitDesired = EnumSuitList.None;
                thisPlayer.BidAmount = 0;
                thisPlayer.TricksWon = 0;
            });
            SaveRoot.TrumpSuit = EnumSuitList.None; //start with none.
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            ThisMod!.PopulateBids();
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "pass":
                    await PassBidAsync();
                    return;
                case "bid":
                    SendBid thisBid = await js.DeserializeObjectAsync<SendBid>(content);
                    ThisMod!.SelectBidAndSuit(thisBid.Bid, thisBid.Suit);
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
            this.ShowTurn();
            await SaveStateAsync();
            await ContinueTurnAsync();
        }
        internal bool CanPass()
        {
            //int Temps = PlayerList.CalculateWhoTurnAsync();
            int temps;
            if (WhoTurn == 1)
                temps = 2;
            else if (WhoTurn == 2 && PlayerList.Count() == 3)
                temps = 3;
            else
                temps = 1; //has to do manually because otherwise, can't do it since its a function to determine whether to enable.  that can't be async unfortunately
            if (temps == WhoStarts && SaveRoot!.WonSoFar == 0)
                return false;
            return true;
        }
        public override bool IsValidMove(int deck)
        {
            if (SaveRoot!.TrickList.Count == 0)
                return true;
            var OriginalCard = SaveRoot.TrickList.First();
            if (OriginalCard.CardType == EnumCardTypeList.Joker)
                return true;
            return base.IsValidMove(deck); //hopefully this simple
        }
        private int WhoWonTrick(DeckObservableDict<PickelCardGameCardInformation> thisCol)
        {
            var firstItem = thisCol.Where(Items => Items.Suit == SaveRoot!.TrumpSuit).OrderByDescending(Items => Items.Value).FirstOrDefault();
            if (firstItem != null)
                return firstItem.Player;
            DeckRegularDict<PickelCardGameCardInformation> tempCol;
            int x;
            PickelCardGameCardInformation thisCard;
            if (thisCol.Any(Items => Items.CardType == EnumCardTypeList.Joker))
            {
                EnumCardValueList Nums = 0;
                int firstPlayer;
                firstItem = thisCol.Where(Items => Items.CardType != EnumCardTypeList.Joker).OrderByDescending(Items => Items.Value).FirstOrDefault();
                if (firstItem != null)
                {
                    Nums = firstItem.Value;
                    firstPlayer = firstItem.Player;
                    if (Nums <= EnumCardValueList.Nine)
                    {
                        tempCol = thisCol.Where(Items => Items.CardType == EnumCardTypeList.Joker).ToRegularDeckDict();
                        if (tempCol.Count == 1)
                            return tempCol.Single().Player;
                    }
                }
                for (x = thisCol.Count; x >= 1; x += -1)
                {
                    thisCard = thisCol[x - 1];
                    if (thisCard.CardType == EnumCardTypeList.Joker)
                        return thisCard.Player;
                }
                tempCol = thisCol.Where(Items => Items.CardType != EnumCardTypeList.Joker && Items.Value == Nums).ToRegularDeckDict();
                if (tempCol.Count == 1)
                    return tempCol.Single().Player;
                for (x = thisCol.Count; x >= 1; x += -1)
                {
                    thisCard = thisCol[x - 1];
                    if (thisCard.Value == Nums)
                        return thisCard.Player;
                }
                throw new BasicBlankException($"Did not find anyone with the highest card of {Nums}.  Find out what happened");
            }
            EnumSuitList leadSuit = thisCol.First().Suit;
            return thisCol.Where(Items => Items.Suit == leadSuit).OrderByDescending(Items => Items.Value).First().Player;
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            PickelCardGamePlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            await _aTrick!.AnimateWinAsync(wins);
            trickList.ForEach(thisCard =>
            {
                PickelCardGameCardInformation newCard = new PickelCardGameCardInformation();
                newCard.Populate(thisCard.Deck);
                newCard.Player = wins;
                SaveRoot.CardList.Add(newCard);
            });
            if (SaveRoot.CardList.Count > 54)
                throw new BasicBlankException("The cardlist can't have more than 54 cards");
            if (SingleInfo!.MainHandList.Count == 0)
            {
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
                thisPlayer.CurrentScore = 0;
                thisPlayer.TotalScore = 0;
            });
            return Task.CompletedTask;
        }
        private bool CanEndGame()
        {
            if (PlayerList.Any(items => items.TotalScore >= 300) == false)
                return false;
            SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
            return true;
        }
        private async Task EndBidAsync()
        {
            SaveRoot!.GameStatus = EnumStatusList.Normal;
            ThisMod!.BiddingScreenVisible = false;
            var ThisPlayer = PlayerList![SaveRoot.WonSoFar];
            SaveRoot.TrumpSuit = ThisPlayer.SuitDesired;
            await StartNewTrickAsync();
        }
        private async Task ContinueBidProcessAsync()
        {
            if (SaveRoot!.HighestBid == 10)
            {
                await EndBidAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            if (WhoTurn == WhoStarts)
            {
                await EndBidAsync();
                return;
            }
            SingleInfo = PlayerList.GetWhoPlayer();
            ThisMod!.PopulateBids();
            await StartNewTurnAsync(); //hopefully this simple.
        }
        public async Task PassBidAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("pass");
            await ContinueBidProcessAsync();
        }
        public async Task ProcessBidAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
            {
                SendBid thisSend = new SendBid()
                {
                    Bid = ThisMod!.BidAmount,
                    Suit = ThisMod.TrumpSuit
                };
                await ThisNet!.SendAllAsync("bid", thisSend);
            }
            SingleInfo!.BidAmount = ThisMod!.BidAmount;
            SingleInfo.SuitDesired = ThisMod.TrumpSuit;
            SaveRoot!.HighestBid = ThisMod.BidAmount;
            SaveRoot.WonSoFar = WhoTurn;
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);

            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.Equals(SingleInfo) == false)
                {
                    thisPlayer.SuitDesired = EnumSuitList.None; //because somebody overrided it.
                    thisPlayer.BidAmount = 0;
                }
            });
            ThisMod.ResetBids();
            await ContinueBidProcessAsync();
        }
        private void FirstScoring()
        {
            var thisPlayer = PlayerList![SaveRoot!.WonSoFar]; //this is the one who won the bidding.
            int tricksReceived = thisPlayer.TricksWon;
            int amountBidded = thisPlayer.BidAmount;
            if (amountBidded == 0)
                throw new BasicBlankException("The amount bidded cannot be 0");
            int totalPoints;
            if (tricksReceived >= amountBidded)
            {
                int diffs = tricksReceived - amountBidded;
                int bonus;
                if (amountBidded == 8)
                    bonus = 10;
                else if (amountBidded == 9)
                    bonus = 30;
                else if (amountBidded == 10)
                    bonus = 50;
                else
                    bonus = 0;
                int extras = 0;
                if (diffs > 0)
                    extras = diffs * 2;
                int normalPoints = amountBidded * 5;
                totalPoints = normalPoints + bonus + extras;
                thisPlayer.CurrentScore = totalPoints;
                thisPlayer.TotalScore += totalPoints;
                return;
            }
            PlayerList.ForEach(otherPlayer =>
            {
                if (otherPlayer.Equals(thisPlayer) == false)
                {
                    totalPoints = otherPlayer.TricksWon * 5;
                    otherPlayer.CurrentScore = totalPoints;
                    otherPlayer.TotalScore += totalPoints;
                }
            });
        }
        private int PointsForPlayer(int player)
        {
            return SaveRoot!.CardList.Where(items => items.Player == player).Sum(Items => Items.Points);
        }
        private void SecondScoring()
        {
            CardScoring();
            int points;
            PlayerList!.ForEach(thisPlayer =>
            {
                points = PointsForPlayer(thisPlayer.Id);
                thisPlayer.CurrentScore = points;
                thisPlayer.TotalScore += points;
            });
        }
        private void CardScoring()
        {
            SaveRoot!.CardList.ForEach(items => items.Points = 0);
            PickelCardGameCardInformation tempCard = SaveRoot.CardList.Where(items => items.Suit == SaveRoot.TrumpSuit).OrderBy(Items => Items.Value).FirstOrDefault();
            int decks = 0;
            if (tempCard != null)
            {
                decks = tempCard.Deck;
                tempCard.Points = 15;
            }
            SaveRoot.CardList.ForEach(thisCard =>
            {
                if (thisCard.CardType == EnumCardTypeList.Joker)
                    thisCard.Points = 15;
                else if (thisCard.Deck == decks)
                {
                    if (thisCard.Points != 15)
                        throw new BasicBlankException("Did not update the points.  Find out what happened");
                }
                else if (thisCard.Value == EnumCardValueList.Five)
                    thisCard.Points = 5;
                else if (thisCard.Value == EnumCardValueList.Ten)
                    thisCard.Points = 10;
                else if (thisCard.Value == EnumCardValueList.HighAce)
                    thisCard.Points = 4;
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
        public override async Task EndRoundAsync()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.CurrentScore = 0);
            FirstScoring();
            if (CanEndGame() == true)
            {
                await ShowWinAsync();
                return;
            }
            SecondScoring();
            if (CanEndGame() == true)
            {
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
    }
}