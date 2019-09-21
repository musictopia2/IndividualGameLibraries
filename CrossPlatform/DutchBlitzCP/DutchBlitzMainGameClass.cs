using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace DutchBlitzCP
{
    [SingletonGame]
    public class DutchBlitzMainGameClass : CardGameClass<DutchBlitzCardInformation, DutchBlitzPlayerItem, DutchBlitzSaveInfo>, IMiscDataNM, IStartNewGame
    {
        public int MaxDiscard; //try this way.
        public ComputerCards? CurrentComputer;
        private ComputerAI? _cis;
        public CustomBasicList<ComputerCards> ComputerPlayers = new CustomBasicList<ComputerCards>();
        private DeckRegularDict<DutchBlitzCardInformation> _otherList = new DeckRegularDict<DutchBlitzCardInformation>();
        public DutchBlitzMainGameClass(IGamePackageResolver container) : base(container) { }

        internal DutchBlitzViewModel? ThisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<DutchBlitzViewModel>();
        }

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
            OtherPile = ThisMod!.Pile1;
            ThisMod.LoadDiscards();
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (HasBlitz)
            {
                await BlitzAsync();
                return;
            }
            if (_cis!.CanEndTurn)
            {
                await EndTurnAsync();
                return;
            }
            var thisMove = _cis.ComputerMove();
            var thisType = _cis.CalculateMoveType(thisMove);
            if (thisType == ComputerAI.EnumMoveType.ContinueMove)
            {
                _cis.DrawCards();
                await ContinueTurnAsync();
                return;
            }
            if (thisType == ComputerAI.EnumMoveType.Transfer)
            {
                _cis.TransferCards();
                await ContinueTurnAsync();
                return;
            }
            var thisCard = _cis.CardToUseForPublic(thisMove);
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
        protected override async Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            SaveRoot!.ImmediatelyStartTurn = true;
            ShuffleOwnCards();
            await ThisLoader!.FinishUpAsync(isBeginning);
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
            DeckList!.ClearObjects();
            DeckList.ShuffleObjects(); //i think.
            var thisList = GetOwnDutchBlitzCards(minNum, maxNum);
            ThisMod!.PublicPiles1!.ClearBoard();
            ThisMod.StockPile!.ClearCards();
            ThisMod.DiscardPiles!.ClearCards();
            PlayerList.ForEach(thisPlayer => thisPlayer.StockLeft = 10);
            10.Times(x =>
            {
                ThisMod.StockPile.AddCard(thisList.First());
                thisList.RemoveFirstItem();
            });
            MaxDiscard.Times(x =>
            {
                ThisMod.DiscardPiles.AddToDiscard(x, thisList.First());
                thisList.RemoveFirstItem();
            });
            ThisMod!.Deck1!.OriginalList(thisList);
            if (ThisData!.MultiPlayer == false)
                ComputerShuffle();
        }
        private void ComputerShuffle()
        {
            int minNum;
            int maxNum;
            ComputerPlayers = new CustomBasicList<ComputerCards>();
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
                MaxDiscard.Times(x =>
                {
                    thisComputer.Discard.Add(thisComputer.DeckList.First());
                    thisComputer.DeckList.RemoveFirstItem();
                });
                ComputerPlayers.Add(thisComputer);
            });
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "blitz":
                    await BlitzAsync();
                    return;
                case "newpile":
                    var newCard = DeckList!.GetSpecificItem(int.Parse(content));
                    newCard.Player = WhoTurn;
                    await AddNewPublicPileAsync(newCard);
                    return;
                case "expandpile":
                    var thisSend = await js.DeserializeObjectAsync<SendExpand>(content);
                    var exCard = DeckList!.GetSpecificItem(thisSend.Deck);
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
                _cis = new ComputerAI(this);
                CurrentComputer = GetComputerPlayer();
            }
            else
            {
                CurrentComputer = null;
            }

            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                ThisMod!.Stops!.StartTimer();
                ThisMod.DidStartTimer = true;
                await Task.Delay(5);
                ThisMod.Stops.PauseTimer();
            }
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                UnselectCards();
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public void UnselectCards()
        {
            ThisMod!.DiscardPiles!.UnselectAllCards();
            ThisMod.StockPile!.UnselectCard();
            ThisMod.Pile1!.UnselectCard();
        }
        private ComputerCards GetComputerPlayer()
        {
            return ComputerPlayers.Single(items => items.Player == WhoTurn);
        }
        internal DeckRegularDict<DutchBlitzCardInformation> GetOwnDutchBlitzCards(int startat, int endat)
        {
            return DeckList.Where(items => items.Deck >= startat && items.Deck <= endat).ToRegularDeckDict();
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
        public bool HasBlitz => SingleInfo!.StockLeft == 0 || ThisTest!.AllowAnyMove;

        private int CalculateScore(int player)
        {
            int played = ThisMod!.PublicPiles1!.PointsPlayed(player, _otherList);
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
            this.RoundOverNext();
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
            if (ThisData!.MultiPlayer == false)
                return;
            await ThisNet!.SendAllAsync("stock", SingleInfo!.StockLeft);
        }
        public async Task AddNewPublicPileAsync(DutchBlitzCardInformation thisCard)
        {
            thisCard.Player = WhoTurn;
            thisCard.Drew = false;
            thisCard.IsUnknown = false;
            thisCard.IsSelected = false;
            ThisMod!.PublicPiles1!.CreateNewPile(thisCard);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer && ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.25);
            await ContinueTurnAsync();
        }
        public async Task ExpandPublicPileAsync(DutchBlitzCardInformation thisCard, int index)
        {
            thisCard.Player = WhoTurn;
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            thisCard.IsUnknown = false; //just in case.
            ThisMod!.PublicPiles1!.AddCardToPile(index, thisCard);
            if (ThisMod.PublicPiles1.NeedToRemovePile(index))
            {
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                var thisList = ThisMod.PublicPiles1.EmptyPileList(index);
                _otherList.AddRange(thisList);
            }
            else
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer && ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.25);
            }
            await ContinueTurnAsync();
        }
        public bool HumanHasSelected()
        {
            if (ThisMod!.StockPile!.CardSelected() > 0)
                return true;
            if (ThisMod.Pile1!.CardSelected() > 0)
                return true;
            _ = ThisMod.DiscardPiles!.CardSelected(out int pile);
            return pile > -1;
        }
        public bool CanHumanPlayCard(int index)
        {
            int decks;
            if (ThisMod!.StockPile!.CardSelected() > 0)
                decks = ThisMod.StockPile.CardSelected();
            else if (ThisMod.Pile1!.CardSelected() > 0)
                decks = ThisMod.Pile1.GetCardInfo().Deck;
            else
                decks = ThisMod.DiscardPiles!.CardSelected(out int _); //iffy
            if (index == -1)
                return ThisMod.PublicPiles1!.CanCreatePile(decks);
            else
                return ThisMod.PublicPiles1!.CanAddToPile(decks, index);
        }
        public DutchBlitzCardInformation HumanCardToUse(out bool fromStock, out bool doSendStock)
        {
            fromStock = false;
            doSendStock = false;
            DutchBlitzCardInformation thisCard;
            if (ThisMod!.StockPile!.CardSelected() > 0)
            {
                thisCard = ThisMod.StockPile.GetCard();
                fromStock = true;
                doSendStock = true; //another function will have to do this (because when using byref, cannot await those)
                SingleInfo!.StockLeft--;
                ThisMod.StockPile.RemoveCard();
                return thisCard;
            }
            if (ThisMod.Pile1!.CardSelected() > 0)
            {
                thisCard = ThisMod.Pile1.GetCardInfo();
                ThisMod.Pile1.RemoveFromPile();
                return thisCard;
            }
            int decks = ThisMod.DiscardPiles!.CardSelected(out int piles);
            if (piles == -1)
                throw new BasicBlankException("No card was selected");
            thisCard = DeckList!.GetSpecificItem(decks);
            ThisMod.DiscardPiles.RemoveCard(piles, decks);
            return thisCard;
        }
    }
}