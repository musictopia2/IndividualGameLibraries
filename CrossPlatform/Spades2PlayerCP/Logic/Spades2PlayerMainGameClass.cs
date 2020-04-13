using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using Spades2PlayerCP.Cards;
using Spades2PlayerCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace Spades2PlayerCP.Logic
{
    [SingletonGame]
    public class Spades2PlayerMainGameClass
        : TrickGameClass<EnumSuitList, Spades2PlayerCardInformation, Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>
        , IMiscDataNM, IStartNewGame
    {


        private readonly Spades2PlayerVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly Spades2PlayerGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly IAdvancedTrickProcesses _aTrick;

        public Spades2PlayerMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            Spades2PlayerVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<Spades2PlayerCardInformation> cardInfo,
            CommandContainer command,
            Spades2PlayerGameContainer gameContainer,
            ITrickData trickData,
            ITrickPlay trickPlay,
            IAdvancedTrickProcesses aTrick
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _aTrick = aTrick;
        }
        private bool _mustKeep;

        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
                _model.OtherPile!.SavedDiscardPiles(SaveRoot.OtherPile!);
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override void LinkHand()
        {
            SingleInfo = PlayerList!.GetSelf();
            _model!.PlayerHand1!.HandList = SingleInfo.MainHandList;
            PrepSort();
        }
        protected override void LoadVM()
        {
            base.LoadVM();
            SaveRoot!.LoadMod(_model!);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            PrepStartTurn(); //because needs cards first.
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        public override async Task DiscardAsync(Spades2PlayerCardInformation thisCard)
        {
            _model.OtherPile!.ClearCards();
            thisCard.Drew = false;
            await AnimatePlayAsync(thisCard);
            if (SaveRoot!.FirstCard == true)
            {
                _mustKeep = true;
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
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
                throw new BasicBlankException("Must be computer player to begin with");
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
            {
                int ask1 = _gameContainer.Random.GetRandomNumber(2);
                if (ask1 == 1)
                {
                    await AcceptCardAsync();
                    return;
                }
                await DiscardAsync(_model.OtherPile!.GetCardInfo());
                return;
            }
            if (SaveRoot.GameStatus == EnumGameStatus.Bidding)
            {
                _model!.BidAmount = _model!.Bid1!.NumberToChoose();
                await ProcessBidAsync();
                return;
            }
            var MoveList = SingleInfo.MainHandList.Where(Items => IsValidMove(Items.Deck)).Select(Items => Items.Deck).ToCustomBasicList();
            await PlayCardAsync(MoveList.GetRandomItem());
        }
        protected override async Task AddCardAsync(Spades2PlayerCardInformation thisCard, Spades2PlayerPlayerItem tempPlayer)
        {
            if (_mustKeep == true)
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
                _model.OtherPile!.Visible = true;
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
            _model!.BidAmount = -1;
            _model.Bid1!.UnselectAll();
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
            {
                if (_model.Deck1!.IsEndOfDeck() == true)
                    SaveRoot.GameStatus = EnumGameStatus.Bidding;
                else
                    SaveRoot.FirstCard = true;
            }
        }
        public override async Task PopulateSaveRootAsync()
        {
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
                SaveRoot.OtherPile = _model.OtherPile!.GetSavedPile();
            await base.PopulateSaveRootAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "acceptcard":
                    await AcceptCardAsync();
                    break;
                case "bid":
                    _model!.BidAmount = int.Parse(content);
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
                _model!.PlayerHand1!.EndTurn();
            if (_model!.Deck1!.IsEndOfDeck() == false && SaveRoot!.GameStatus != EnumGameStatus.ChooseCards)
                throw new BasicBlankException("If its not the end of the deck, then status must be choose cards");
            if (_model.Deck1.IsEndOfDeck() == true && SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
                throw new BasicBlankException("If its the end of the deck, gamestatus cannot be choosecards");
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseCards)
                SaveRoot.NeedsToDraw = true;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _command.ManuelFinish = true; //because it could be somebody else's turn.
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
            _model!.PlayerHand1!.EndTurn();
            WhoTurn = wins; //most of the time, whoever wins leads again.
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            _command!.ManuelFinish = true; //because it could be somebody else's turn.
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
            await this.RoundOverNextAsync();
        }
        public async Task AcceptCardAsync()
        {
            SaveRoot!.FirstCard = false;
            _mustKeep = false;
            if (_model.OtherPile!.PileEmpty() == true)
                throw new BasicBlankException("Nothing in other pile.  Rethink");
            var thisCard = _model.OtherPile.GetCardInfo();
            SingleInfo!.MainHandList.Add(thisCard);
            _model.OtherPile.Visible = false;
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
                _model!.Bid1!.SelectNumberValue(_model.BidAmount);
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(.5);
            }
            SingleInfo.HowManyBids = _model!.BidAmount;
            if (PlayerList.All(Items => Items.HowManyBids > -1))
            {
                SaveRoot!.GameStatus = EnumGameStatus.Normal;
                _aTrick!.ClearBoard(); //i think it needs to happen here.
            }
            await EndTurnAsync();
        }

    }
}
