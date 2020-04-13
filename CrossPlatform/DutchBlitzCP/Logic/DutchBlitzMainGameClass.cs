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
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using DutchBlitzCP.Cards;
using DutchBlitzCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace DutchBlitzCP.Logic
{
    [SingletonGame]
    public class DutchBlitzMainGameClass : CardGameClass<DutchBlitzCardInformation, DutchBlitzPlayerItem, DutchBlitzSaveInfo>, IMiscDataNM, IStartNewGame
    {


        private readonly DutchBlitzVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly DutchBlitzGameContainer _gameContainer; //if we don't need it, take it out.
        private ComputerAI? _ai;

        public DutchBlitzMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            DutchBlitzVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<DutchBlitzCardInformation> cardInfo,
            CommandContainer command,
            DutchBlitzGameContainer gameContainer
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
        }
        private DeckRegularDict<DutchBlitzCardInformation> _otherList = new DeckRegularDict<DutchBlitzCardInformation>();

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            ShuffleOwnCards();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            _otherList = new DeckRegularDict<DutchBlitzCardInformation>();
            if (IsLoaded == true)
                return;
            _model.LoadDiscards();
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (HasBlitz)
            {
                await BlitzAsync();
                return;
            }
            if (_ai!.CanEndTurn)
            {
                await EndTurnAsync();
                return;
            }
            var thisMove = _ai.ComputerMove();
            var thisType = _ai.CalculateMoveType(thisMove);
            if (thisType == ComputerAI.EnumMoveType.ContinueMove)
            {
                _ai.DrawCards();
                await ContinueTurnAsync();
                return;
            }
            if (thisType == ComputerAI.EnumMoveType.Transfer)
            {
                _ai.TransferCards();
                await ContinueTurnAsync();
                return;
            }
            var thisCard = _ai.CardToUseForPublic(thisMove);
            if (thisMove.NewPublicPile)
            {
                if (thisCard.CardValue != 1)
                    throw new BasicBlankException("Cannot create a new pile because the number is not 1");
                await AddNewPublicPileAsync(thisCard);
                return;
            }
            if (thisMove.PublicPile == -1)
                throw new BasicBlankException("Cannot expand on a public pile if its -1");
            await ExpandPublicPileAsync(thisCard, thisMove.PublicPile);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            SaveRoot!.ImmediatelyStartTurn = true;
            ShuffleOwnCards();
            return FinishUpAsync!.Invoke(isBeginning);
        }
        private void ShuffleOwnCards()
        {
            int whatPlayer = PlayerList!.GetSelf().Id;
            int minNum;
            int maxNum;
            if (PlayerList.Count() > 2)
            {
                DutchBlitzDeckCount.DoubleDeck = false;
                switch (whatPlayer)
                {
                    case 1:
                        minNum = 1;
                        maxNum = 40;
                        break;
                    case 2:
                        minNum = 41;
                        maxNum = 80;
                        break;
                    case 3:
                        minNum = 81;
                        maxNum = 120;
                        break;
                    case 4:
                        minNum = 121;
                        maxNum = 160;
                        break;
                    default:
                        throw new BasicBlankException("Not Supported");
                }
            }
            else
            {
                DutchBlitzDeckCount.DoubleDeck = true;
                if (whatPlayer == 1)
                {
                    minNum = 1;
                    maxNum = 80;
                }
                else if (whatPlayer == 2)
                {
                    minNum = 81;
                    maxNum = 160;
                }
                else
                {
                    throw new BasicBlankException("Not Supported");
                }
            }
            _gameContainer.DeckList!.ClearObjects();
            _gameContainer.DeckList.ShuffleObjects(); //i think.
            var thisList = GetOwnDutchBlitzCards(minNum, maxNum);
            _model!.PublicPiles1!.ClearBoard();
            _model.StockPile!.ClearCards();
            _model.DiscardPiles!.ClearCards();
            _model.Pile1.ClearCards(); //try this too.
            PlayerList.ForEach(thisPlayer => thisPlayer.StockLeft = 10);
            10.Times(x =>
            {
                _model.StockPile.AddCard(thisList.First());
                thisList.RemoveFirstItem();
            });
            _gameContainer.MaxDiscard.Times(x =>
            {
                _model.DiscardPiles.AddToDiscard(x, thisList.First());
                thisList.RemoveFirstItem();
            });
            _model!.Deck1!.OriginalList(thisList);
            if (BasicData!.MultiPlayer == false)
                ComputerShuffle();
        }
        private void ComputerShuffle()
        {
            int minNum;
            int maxNum;
            _gameContainer.ComputerPlayers = new CustomBasicList<ComputerCards>();
            PlayerList!.ForEach(thisPlayer =>
            {
                int whatPlayer = thisPlayer.Id;
                if (PlayerList.Count() > 2)
                {
                    DutchBlitzDeckCount.DoubleDeck = false;
                    switch (whatPlayer)
                    {
                        case 1:
                            minNum = 1;
                            maxNum = 40;
                            break;
                        case 2:
                            minNum = 41;
                            maxNum = 80;
                            break;
                        case 3:
                            minNum = 81;
                            maxNum = 120;
                            break;
                        case 4:
                            minNum = 121;
                            maxNum = 160;
                            break;
                        default:
                            throw new BasicBlankException("Not Supported");
                    }
                }
                else
                {
                    DutchBlitzDeckCount.DoubleDeck = true;
                    if (whatPlayer == 1)
                    {
                        minNum = 1;
                        maxNum = 80;
                    }
                    else if (whatPlayer == 2)
                    {
                        minNum = 81;
                        maxNum = 160;
                    }
                    else
                    {
                        throw new BasicBlankException("Not Supported");
                    }
                }
                var thisComputer = new ComputerCards();
                thisComputer.Player = thisPlayer.Id;
                var thisList = GetOwnDutchBlitzCards(minNum, maxNum);
                thisList.ForEach(thisCard => thisCard.Player = whatPlayer);
                thisComputer.DeckList.AddRange(thisList);
                10.Times(x =>
                {
                    thisComputer.StockList.Add(thisComputer.DeckList.First());
                    thisComputer.DeckList.RemoveFirstItem();
                });
                _gameContainer.MaxDiscard.Times(x =>
                {
                    thisComputer.Discard.Add(thisComputer.DeckList.First());
                    thisComputer.DeckList.RemoveFirstItem();
                });
                _gameContainer.ComputerPlayers.Add(thisComputer);
            });
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "blitz":
                    await BlitzAsync();
                    return;
                case "newpile":
                    var newCard = _gameContainer.DeckList!.GetSpecificItem(int.Parse(content));
                    newCard.Player = WhoTurn;
                    await AddNewPublicPileAsync(newCard);
                    return;
                case "expandpile":
                    var thisSend = await js.DeserializeObjectAsync<SendExpand>(content);
                    var exCard = _gameContainer.DeckList!.GetSpecificItem(thisSend.Deck);
                    exCard.Player = WhoTurn;
                    await ExpandPublicPileAsync(exCard, thisSend.Pile);
                    return;
                case "stock":
                    await UpdateStockAsync(int.Parse(content));
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
            {
                _gameContainer.CurrentComputer = GetComputerPlayer();
                _ai = new ComputerAI(_gameContainer, _model);
            }
            else
            {
                _gameContainer.CurrentComputer = null;
            }

            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model!.Stops!.StartTimer();
                _model.DidStartTimer = true;
                await Task.Delay(5);
                _model.Stops.PauseTimer();
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                UnselectCards();
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public void UnselectCards()
        {
            _model!.DiscardPiles!.UnselectAllCards();
            _model.StockPile!.UnselectCard();
            _model.Pile1!.UnselectCard();
        }
        private ComputerCards GetComputerPlayer()
        {
            return _gameContainer.ComputerPlayers.Single(items => items.Player == WhoTurn);
        }
        internal DeckRegularDict<DutchBlitzCardInformation> GetOwnDutchBlitzCards(int startat, int endat)
        {
            return _gameContainer.DeckList.Where(items => items.Deck >= startat && items.Deck <= endat).ToRegularDeckDict();
        }

        private bool CanEndGame()
        {
            if (PlayerList.Any(items => items.PointsGame >= 30))
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.PointsGame).First();
                return true;
            }
            return false;
        }
        public async Task UpdateStockAsync(int howMany)
        {
            SingleInfo!.StockLeft = howMany;
            await ContinueTurnAsync();
        }
        public async Task BlitzAsync()
        {
            await EndRoundAsync();
        }
        public bool HasBlitz => SingleInfo!.StockLeft == 0 || Test!.AllowAnyMove;

        private int CalculateScore(int player)
        {
            int played = _model!.PublicPiles1!.PointsPlayed(player, _otherList);
            var thisPlayer = PlayerList![player];
            int lefts = thisPlayer.StockLeft;
            int minuss = lefts * 2;
            if (played - minuss < 0)
                return 0;
            return played - minuss;
        }
        public override async Task EndRoundAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                int points = CalculateScore(thisPlayer.Id);
                thisPlayer.PointsRound = points;
                thisPlayer.PointsGame += points;
            });
            if (CanEndGame())
            {
                await ShowWinAsync();
                return;
            }
            await this.RoundOverNextAsync();
        }

        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PointsGame = 0;
                thisPlayer.PointsRound = 0;
            });
            return Task.CompletedTask;
        }
        public async Task SendStockAsync()
        {
            if (BasicData!.MultiPlayer == false)
                return;
            await Network!.SendAllAsync("stock", SingleInfo!.StockLeft);
        }
        public async Task AddNewPublicPileAsync(DutchBlitzCardInformation thisCard)
        {
            thisCard.Player = WhoTurn;
            thisCard.Drew = false;
            thisCard.IsUnknown = false;
            thisCard.IsSelected = false;
            _model!.PublicPiles1!.CreateNewPile(thisCard);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer && Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.25);
            await ContinueTurnAsync();
        }
        public async Task ExpandPublicPileAsync(DutchBlitzCardInformation thisCard, int index)
        {
            thisCard.Player = WhoTurn;
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            thisCard.IsUnknown = false; //just in case.
            _model!.PublicPiles1!.AddCardToPile(index, thisCard);
            if (_model.PublicPiles1.NeedToRemovePile(index))
            {
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                var thisList = _model.PublicPiles1.EmptyPileList(index);
                _otherList.AddRange(thisList);
            }
            else
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer && Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(.25);
            }
            await ContinueTurnAsync();
        }
        public bool HumanHasSelected()
        {
            if (_model!.StockPile!.CardSelected() > 0)
                return true;
            if (_model.Pile1!.CardSelected() > 0)
                return true;
            _ = _model.DiscardPiles!.CardSelected(out int pile);
            return pile > -1;
        }
        public bool CanHumanPlayCard(int index)
        {
            int decks;
            if (_model!.StockPile!.CardSelected() > 0)
                decks = _model.StockPile.CardSelected();
            else if (_model.Pile1!.CardSelected() > 0)
                decks = _model.Pile1.GetCardInfo().Deck;
            else
                decks = _model.DiscardPiles!.CardSelected(out int _); //iffy
            if (index == -1)
                return _model.PublicPiles1!.CanCreatePile(decks);
            else
                return _model.PublicPiles1!.CanAddToPile(decks, index);
        }
        public DutchBlitzCardInformation HumanCardToUse(out bool fromStock, out bool doSendStock)
        {
            fromStock = false;
            doSendStock = false;
            DutchBlitzCardInformation thisCard;
            if (_model!.StockPile!.CardSelected() > 0)
            {
                thisCard = _model.StockPile.GetCard();
                fromStock = true;
                doSendStock = true; //another function will have to do this (because when using byref, cannot await those)
                SingleInfo!.StockLeft--;
                _model.StockPile.RemoveCard();
                return thisCard;
            }
            if (_model.Pile1!.CardSelected() > 0)
            {
                thisCard = _model.Pile1.GetCardInfo();
                _model.Pile1.RemoveFromPile();
                return thisCard;
            }
            int decks = _model.DiscardPiles!.CardSelected(out int piles);
            if (piles == -1)
                throw new BasicBlankException("No card was selected");
            thisCard = _gameContainer.DeckList!.GetSpecificItem(decks);
            _model.DiscardPiles.RemoveCard(piles, decks);
            return thisCard;
        }
    }
}