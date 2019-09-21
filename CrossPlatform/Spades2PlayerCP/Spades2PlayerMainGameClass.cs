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
using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;

namespace Spades2PlayerCP
{
    [SingletonGame]
    public class Spades2PlayerMainGameClass : TrickGameClass<EnumSuitList, Spades2PlayerCardInformation,
        Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>, IMiscDataNM, IStartNewGame
    {
        private IAdvancedTrickProcesses? _aTrick;
        public Spades2PlayerMainGameClass(IGamePackageResolver container) : base(container) { }
        private bool MustKeep;
        private Spades2PlayerViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
			base.Init();
            _thisMod = MainContainer.Resolve<Spades2PlayerViewModel>();
        }

        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
                OtherPile!.SavedDiscardPiles(SaveRoot.OtherPile!);
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
            TrickArea1!.Visible = SaveRoot.GameStatus == EnumGameStatus.Normal;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadTrickAreas();
            _aTrick = MainContainer.Resolve<IAdvancedTrickProcesses>(); //decided to be here.
            IsLoaded = true; //i think needs to be here.
        }
        protected override void LinkHand()
        {
            SingleInfo = PlayerList!.GetSelf();
            _thisMod!.PlayerHand1!.HandList = SingleInfo.MainHandList;
            PrepSort();
        }
        protected override void LoadVM()
        {
            base.LoadVM();
            SaveRoot!.LoadMod(_thisMod!);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            PrepStartTurn(); //because needs cards first.
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        public override async Task DiscardAsync(Spades2PlayerCardInformation thisCard)
        {
            OtherPile!.ClearCards();
            thisCard.Drew = false;
            await AnimatePlayAsync(thisCard);
            if (SaveRoot!.FirstCard == true)
            {
                MustKeep = true;
                SaveRoot.FirstCard = false;
                await DrawAsync();
                return;
            }
        }
        protected override bool CardToCurrentPile()
        {
            return SaveRoot!.FirstCard;
        }
        protected override async Task AfterDrawingAsync()
        {
            if (SaveRoot!.FirstCard == true)
                await ContinueTurnAsync();
            else
                await EndTurnAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
                throw new BasicBlankException("Must be computer player to begin with");
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
            {
                if (rs == null)
                    rs = MainContainer.Resolve<RandomGenerator>();
                int ask1 = rs.GetRandomNumber(2);
                if (ask1 == 1)
                {
                    await AcceptCardAsync();
                    return;
                }
                await DiscardAsync(OtherPile!.GetCardInfo());
                return;
            }
            if (SaveRoot.GameStatus == EnumGameStatus.Bidding)
            {
                _thisMod!.BidAmount = _thisMod!.Bid1!.NumberToChoose();
                await ProcessBidAsync();
                return;
            }
            var MoveList = SingleInfo.MainHandList.Where(Items => IsValidMove(Items.Deck)).Select(Items => Items.Deck).ToCustomBasicList();
            await PlayCardAsync(MoveList.GetRandomItem());
        }
        private RandomGenerator? rs;
        protected override async Task AddCardAsync(Spades2PlayerCardInformation thisCard, Spades2PlayerPlayerItem tempPlayer)
        {
            if (MustKeep == true)
                await base.AddCardAsync(thisCard, tempPlayer);
            else
                await DiscardAsync(thisCard); //it will draw but since you can't keep will discard it.
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.NeedsToDraw == true)
            {
                SaveRoot.NeedsToDraw = false;
                await DrawAsync();
                return;
            }
            if (SaveRoot.GameStatus != EnumGameStatus.ChooseCards && SingleInfo!.MainHandList.Count == 0)
                throw new BasicBlankException("If you are not choosing cards, you must have at least one card");
            if (SaveRoot.GameStatus == EnumGameStatus.ChooseCards && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                OtherPile!.Visible = true;
            await base.ContinueTurnAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            SaveRoot!.RoundNumber++;
            SaveRoot.TrickStatus = EnumTrickStatus.FirstTrick;
            SaveRoot.GameStatus = EnumGameStatus.ChooseCards;
            LoadControls();
            LoadVM();
            SaveRoot.NeedsToDraw = true;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TricksWon = 0;
                thisPlayer.HowManyBids = -1;
            });
            return base.StartSetUpAsync(isBeginning);
        }
        protected override void PrepStartTurn()
        {
            base.PrepStartTurn();
            _thisMod!.BidAmount = -1;
            _thisMod.Bid1!.UnselectAll();
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
            {
                if (_thisMod.Deck1!.IsEndOfDeck() == true)
                    SaveRoot.GameStatus = EnumGameStatus.Bidding;
                else
                    SaveRoot.FirstCard = true;
            }
        }
        public override async Task PopulateSaveRootAsync()
        {
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
                SaveRoot.OtherPile = OtherPile!.GetSavedPile();
            await base.PopulateSaveRootAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "acceptcard":
                    await AcceptCardAsync();
                    break;
                case "bid":
                    _thisMod!.BidAmount = int.Parse(content);
                    await ProcessBidAsync();
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                _thisMod!.PlayerHand1!.EndTurn();
            if (_thisMod!.Deck1!.IsEndOfDeck() == false && SaveRoot!.GameStatus != EnumGameStatus.ChooseCards)
                throw new BasicBlankException("If its not the end of the deck, then status must be choose cards");
            if (_thisMod.Deck1.IsEndOfDeck() == true && SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
                throw new BasicBlankException("If its the end of the deck, gamestatus cannot be choosecards");
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
                SaveRoot.NeedsToDraw = true;
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        private int WhoWonTrick(DeckObservableDict<Spades2PlayerCardInformation> thisCol)
        {
            var leadCard = thisCol.First();
            var thisCard = thisCol.Last();
            if (thisCard.Suit == EnumSuitList.Spades && leadCard.Suit != EnumSuitList.Spades)
                return WhoTurn;
            if (leadCard.Suit == EnumSuitList.Spades && thisCard.Suit != EnumSuitList.Spades)
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
            if (trickList.Any(Items => Items.Suit == EnumSuitList.Spades))
                SaveRoot.TrickStatus = EnumTrickStatus.SuitBroken;
            else if (SaveRoot.TrickStatus == EnumTrickStatus.FirstTrick)
                SaveRoot.TrickStatus = EnumTrickStatus.NoSuit;
            int wins = WhoWonTrick(trickList);
            Spades2PlayerPlayerItem ThisPlayer = PlayerList![wins];
            ThisPlayer.TricksWon++;
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
            await StartNewTurnAsync(); //hopefully this simple.
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.Bags = 0;
                thisPlayer.CurrentScore = 0;
            });
            SaveRoot!.RoundNumber = 0;
            return Task.CompletedTask;
        }
        private void GetWinningPlayer()
        {
            SingleInfo = PlayerList.OrderByDescending(Items => Items.TotalScore).Take(1).Single();
        }

        private bool CanEndGame()
        {
            if (SaveRoot!.RoundNumber >= 15)
            {
                GetWinningPlayer();
                return true;
            }
            if (PlayerList.Any(items => items.TotalScore >= 500))
            {
                GetWinningPlayer();
                return true;
            }
            return false;
        }
        public override async Task EndRoundAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CalculateScore();
            });
            if (CanEndGame() == true)
            {
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        public async Task AcceptCardAsync()
        {
            SaveRoot!.FirstCard = false;
            MustKeep = false;
            if (OtherPile!.PileEmpty() == true)
                throw new BasicBlankException("Nothing in other pile.  Rethink");
            var thisCard = OtherPile.GetCardInfo();
            SingleInfo!.MainHandList.Add(thisCard);
            OtherPile.Visible = false;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisCard.Drew = true;
                SortCards();
            }
            await DrawAsync();
        }
        public async Task ProcessBidAsync()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
            {
                _thisMod!.Bid1!.SelectNumberValue(_thisMod.BidAmount);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.5);
            }
            SingleInfo.HowManyBids = _thisMod!.BidAmount;
            if (PlayerList.All(Items => Items.HowManyBids > -1))
            {
                SaveRoot!.GameStatus = EnumGameStatus.Normal;
                _aTrick!.ClearBoard(); //i think it needs to happen here.
            }
            await EndTurnAsync();
        }
    }
}