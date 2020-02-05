using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace SkipboCP
{
    [SingletonGame]
    public class SkipboMainGameClass : CardGameClass<SkipboCardInformation, SkipboPlayerItem, SkipboSaveInfo>, IMiscDataNM
    {
        public SkipboMainGameClass(IGamePackageResolver container) : base(container) { }

        private SkipboViewModel? _thisMod;
        internal CustomBasicList<ComputerData> ComputerList = new CustomBasicList<ComputerData>(); //the computer ai needs it.

        public PublicPilesViewModel? PublicPiles; //has to create this too.
        private SkipboComputerAI? _ai;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<SkipboViewModel>();
        }

        public override async Task FinishGetSavedAsync()
        {
            SaveRoot!.LoadMod(_thisMod!);
            LoadPlayerStocks();
            LoadControls();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.StockPile!.StockFrame.PileList!.ReplaceRange(thisPlayer.StockList);
                thisPlayer.DiscardPiles!.PileList!.ReplaceRange(thisPlayer.DiscardList);
            });
            PublicPiles!.PileList!.ReplaceRange(SaveRoot.PublicPileList);
            if (SaveRoot.ImmediatelyStartTurn == false)
                PrepStartTurn();
            await base.FinishGetSavedAsync();
        }
        protected override Task MiddleReshuffleCardsAsync(IDeckDict<SkipboCardInformation> thisList, bool canSend)
        {
            if (thisList.Count != SaveRoot!.CardsToShuffle)
                throw new BasicBlankException($"Must have {SaveRoot!.CardsToShuffle}, not {thisList.Count}");
            var nextList = _thisMod!.Pile1!.DiscardList();
            if (nextList.Count > 0)
                throw new BasicBlankException("The discard list somehow did not get cleared out");
            return base.MiddleReshuffleCardsAsync(thisList, canSend);
        }
        protected override void PrepStartTurn()
        {
            base.PrepStartTurn();
            ComputerList = new CustomBasicList<ComputerData>(); //because we don't have autoresume computer data for now.
            SingleInfo!.DiscardPiles!.Visible = true;
            SingleInfo.StockPile!.StockFrame.Visible = true;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            PublicPiles = new PublicPilesViewModel(_thisMod!); //hopefully don't have to set
            PublicPiles.IsEnabled = false; //hopefully this is fine too.
            PublicPiles.PileClickedAsync += PublicPiles_PileClickedAsync;
            if (ThisData!.MultiPlayer == false || ThisData.Client == false)
                _ai = MainContainer.Resolve<SkipboComputerAI>();
            IsLoaded = true; //i think needs to be here.
        }
        private async Task ComputerDiscardAsync()
        {
            SkipboComputerAI.ComputerDiscardInfo thisDiscard;
            thisDiscard = _ai!.ComputerDiscard();
            await AddToDiscardAsync(thisDiscard.Pile, thisDiscard.Deck);
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelayMilli(200);
            if (ComputerList.Count == 0)
            {
                ComputerList = _ai!.ComputerMoves();
                if (ComputerList.Count == 0)
                {
                    await ComputerDiscardAsync();
                    return;
                }
            }
            ComputerData thisItem = ComputerList.First();
            if (thisItem.WhichType == EnumCardType.IsNone)
                throw new BasicBlankException("Needs to find a card type to play one");
            await PlayOnPileAsync(thisItem.Pile, thisItem.ThisCard!.Deck, thisItem.WhichType, thisItem.Discard);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (isBeginning == true)
                LoadPlayerStocks(); //this has to be done before you can pass out cards anyways.
            LoadControls();
            SaveRoot!.ImmediatelyStartTurn = true;
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            SaveRoot!.CardsToShuffle = 0;
            PublicPiles!.ClearBoard();
            SaveRoot.LoadMod(_thisMod!); //hopefully this works.
            PlayerList!.ForEach(thisPlayer =>
            {

                thisPlayer.StockPile!.ClearCards();
                thisPlayer.DiscardPiles!.ClearBoard();
                thisPlayer.MainHandList.ForEach(ThisCard =>
                {
                    thisPlayer.StockPile.AddCard(ThisCard);
                });
                thisPlayer.MainHandList.Clear();
                UpdateStockData(thisPlayer); //i think here is correct.  well see.
                thisPlayer.Discard1 = "0";
                thisPlayer.Discard2 = "0";
                thisPlayer.Discard3 = "0";
                thisPlayer.Discard4 = "0";
                thisPlayer.Discard5 = "0";
            });
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        private async Task PublicPiles_PileClickedAsync(int index, BasicPileInfo<SkipboCardInformation> thisPile)
        {
            int decks = CardSelected(out EnumCardType types, out int discardNum);
            if (decks == 0 && types == EnumCardType.IsNone)
            {
                await _thisMod!.ShowGameMessageAsync("Sorry, there was nothing selected");
                return;
            }
            if (decks == 0)
                throw new BasicBlankException("Nothing selected but the type was not none");
            bool rets = IsValidMove(index, decks);
            if (rets == false)
            {
                await _thisMod!.ShowGameMessageAsync("Illegal Move");
                return;
            }
            await PlayOnPileAsync(index, decks, types, discardNum);
        }
        private void LoadPlayerStocks()
        {
            PlayerList!.ForEach(items =>
            {
                items.LoadPlayerPiles(this); //i think.
            });
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "discardnew":
                    SendDiscard ThisDiscard = await js.DeserializeObjectAsync<SendDiscard>(content);
                    await AddToDiscardAsync(ThisDiscard.Pile, ThisDiscard.Deck);
                    return;
                case "play":
                    SendPlay ThisPlay = await js.DeserializeObjectAsync<SendPlay>(content);
                    await PlayOnPileAsync(ThisPlay.Pile, ThisPlay.Deck, ThisPlay.WhichType, ThisPlay.Discard);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await StartDrawingAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                _thisMod!.PlayerHand1!.EndTurn();
            else if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
                SingleInfo.MainHandList.UnhighlightObjects();
            SingleInfo.DiscardPiles!.Visible = false;
            SingleInfo.StockPile!.StockFrame.Visible = false;
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await StartNewTurnAsync();
        }
        public async override Task PopulateSaveRootAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.StockList = thisPlayer.StockPile!.StockFrame.PileList.ToCustomBasicList();
                thisPlayer.DiscardList = thisPlayer.DiscardPiles!.PileList.ToCustomBasicList();
            });
            SaveRoot!.PublicPileList = PublicPiles!.PileList.ToCustomBasicList();
            await base.PopulateSaveRootAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer(); //just in case.
            if (SingleInfo.MainHandList.Count == 0)
            {
                await SaveStateAsync(); //just in case.
                await StartDrawingAsync();
            }
            else
                await base.ContinueTurnAsync();
        }
        private bool _wasNew;
        private async Task StartDrawingAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer(); //just in case.
            if (SingleInfo.MainHandList.Count == 5)
            {
                await ContinueTurnAsync();
                return; // because you already have 5 cards now.
            }
            if (SingleInfo.MainHandList.Count == 0)
                _wasNew = true;
            else
                _wasNew = false;
            LeftToDraw = 5 - SingleInfo.MainHandList.Count;
            PlayerDraws = WhoTurn;
            if (LeftToDraw == 1)
            {
                LeftToDraw = 0;
                PlayerDraws = 0;
            }
            await DrawAsync();
        }
        public void UnselectAllCards()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.StockPile!.UnselectCard();
            PublicPiles!.UnselectAllPiles();
            _thisMod!.PlayerHand1!.UnselectAllObjects();
            SingleInfo.DiscardPiles!.UnselectAllCards();
        }
        private void UpdateStockData(SkipboPlayerItem thisPlayer)
        {
            thisPlayer.InStock = thisPlayer.StockPile!.NextCardInStock();
            thisPlayer.StockLeft = thisPlayer.StockPile.CardsLeft();
        }
        private void UpdateDiscardData(SkipboPlayerItem thisPlayer, int thisPile)
        {
            string thisNum;
            if (thisPlayer.DiscardPiles!.HasCard(thisPile) == false)
                thisNum = "0";
            else
            {
                var ThisCard = thisPlayer.DiscardPiles.GetLastCard(thisPile);
                thisNum = ThisCard.Display; // i think needs to be value.  So can even be W
            }
            switch (thisPile)
            {
                case 0:
                    {
                        thisPlayer.Discard1 = thisNum;
                        break;
                    }

                case 1:
                    {
                        thisPlayer.Discard2 = thisNum;
                        break;
                    }

                case 2:
                    {
                        thisPlayer.Discard3 = thisNum;
                        break;
                    }

                case 3:
                    {
                        thisPlayer.Discard4 = thisNum;
                        break;
                    }
                case 4:
                    {
                        thisPlayer.Discard5 = thisNum;
                        break; //to support flinch.
                    }
                default:
                    {
                        throw new BasicBlankException("Piles are only 0 to 3, not " + thisPile + ".  Remember, 0 based now");
                    }
            }
        }
        private void RemoveFromHand(int deck)
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
            if (SingleInfo.MainHandList.Count == 5)
                throw new BasicBlankException($"After removing a card, must have less than 5 cards left for {SingleInfo.NickName}");
        }
        protected override Task AfterDrawingAsync()
        {
            if (_wasNew == true)
                SingleInfo!.MainHandList.UnhighlightObjects();
            if (SingleInfo!.MainHandList.Any(items => items.IsUnknown))
                throw new BasicBlankException("Should never have unknown cards");
            return base.AfterDrawingAsync();
        }
        protected override Task AfterReshuffleAsync()
        {
            SaveRoot!.CardsToShuffle = 0; //because reset now.
            var firstList = _thisMod!.Deck1!.DeckList();
            firstList.ForConditionalItems(items => items.IsWild && items.Number > 0, items => items.Number = 0); //try this way to resolve the first problem at least.  
            return base.AfterReshuffleAsync();
        }
        private void RemoveFromPile(int pile)
        {
            var thisCol = PublicPiles!.EmptyPileList(pile);
            thisCol.ForEach(thisCard =>
            {
                SaveRoot!.CardsToShuffle++;
                _thisMod!.Pile1!.AddCard(thisCard); //for reshuffling eventually.
            });
        }
        public async Task AddToDiscardAsync(int pile, int deck)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!) == true)
            {
                SendDiscard thisDiscard = new SendDiscard();
                thisDiscard.Deck = deck;
                thisDiscard.Pile = pile;
                await ThisNet!.SendAllAsync("discardnew", thisDiscard);
            }
            if (SingleInfo.MainHandList.Count > 5)
                throw new BasicBlankException($"The hand must be 5 or less, not {SingleInfo.MainHandList.Count} for {SingleInfo.NickName}");
            RemoveFromHand(deck);
            var thisCard = DeckList!.GetSpecificItem(deck);
            if (thisCard.IsUnknown)
                throw new BasicBlankException("Can not be unknown when playing on discard pile.  Rethink");
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            await SingleInfo.AnimateDiscardAsync(thisCard, pile, EnumAnimcationDirection.StartUpToCard);
            SingleInfo.DiscardPiles!.AddCardToPile(pile, thisCard);
            UpdateDiscardData(SingleInfo, pile);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            await EndTurnAsync();
        }
        public bool IsValidMove(int pile, int deck)
        {
            var thisCard = DeckList!.GetSpecificItem(deck);
            if (thisCard.IsWild == true)
                return true;
            int nexts = PublicPiles!.NextNumberNeeded(pile);
            return nexts == thisCard.Number; //i don't we can allow any move because that can hose it up later.
        }
        public int CardSelected(out EnumCardType whatType, out int discardNum)
        {
            int thisNum = _thisMod!.PlayerHand1!.ObjectSelected();
            if (thisNum > 0)
            {
                whatType = EnumCardType.MyCards;
                discardNum = -1;
                return thisNum;
            }
            thisNum = SingleInfo!.DiscardPiles!.CardSelected(out int thisPile);
            if (thisNum > 0)
            {
                whatType = EnumCardType.Discard;
                discardNum = thisPile;
                if (discardNum == -1)
                    throw new BasicBlankException("Must be something from the discard pile");
                return thisNum;
            }
            discardNum = -1;
            thisNum = SingleInfo.StockPile!.CardSelected();
            if (thisNum > 0)
            {
                whatType = EnumCardType.Stock;
                return thisNum;
            }
            whatType = EnumCardType.IsNone;
            return 0;
        }
        public async Task PlayOnPileAsync(int pile, int deck, EnumCardType whichType, int discardNum)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (whichType == EnumCardType.IsNone)
                throw new BasicBlankException("Must have a card type in order to play on a pile");
            if (SingleInfo.CanSendMessage(ThisData!) == true)
            {
                SendPlay thisPlay = new SendPlay();
                thisPlay.Deck = deck;
                thisPlay.Pile = pile;
                thisPlay.WhichType = whichType;
                thisPlay.Discard = discardNum;
                await ThisNet!.SendAllAsync("play", thisPlay);
            }
            int nexts = PublicPiles!.NextNumberNeeded(pile);
            var thisCard = DeckList!.GetSpecificItem(deck);
            if (thisCard.IsUnknown)
                throw new BasicBlankException("Should not have been unknown.  Rethink");
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            if (whichType == EnumCardType.MyCards)
                RemoveFromHand(deck);
            else if (whichType == EnumCardType.Stock)
            {
                SingleInfo.StockPile!.RemoveCard();
                await SingleInfo.AnimateStockAsync(thisCard);
                UpdateStockData(SingleInfo);
            }
            else
            {
                if (discardNum == -1)
                    throw new BasicBlankException("The discard number cannot be -1");
                if (SingleInfo.DiscardPiles!.PileList![discardNum].ObjectList.Count == 0)
                    throw new BasicBlankException("The discard must have at least one item to play from");
                SingleInfo.DiscardPiles.RemoveCard(discardNum, deck);
                UpdateDiscardData(SingleInfo, discardNum);
                await SingleInfo.AnimateDiscardAsync(thisCard, discardNum, EnumAnimcationDirection.StartCardToUp);
                SingleInfo.DiscardPiles.UnselectAllCards();
            }
            var thisPile = PublicPiles.PileList![pile];
            if (thisCard.IsWild == true)
                thisCard.Number = thisPile.ObjectList.Count + 1;
            await ThisE.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartDownToCard, "public", thisPile);
            PublicPiles.AddCardToPile(pile, thisCard); //hopefully this is still okay.
            if (PublicPiles.NeedToRemovePile(pile))
            {
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                RemoveFromPile(pile);
            }
            if (whichType == EnumCardType.Stock)
            {
                if (SingleInfo.StockPile!.DidGoOut() == true)
                {
                    await ShowWinAsync();
                    return;
                }
            }
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
            {
                if (ThisData!.MultiPlayer == false || ThisData.Client == false)
                    ComputerList.RemoveFirstItem(); //hopefully this is still fine (?)
            }
            int manys = _thisMod!.Pile1!.HowManyInDiscard();
            if (_thisMod.Pile1.PileEmpty() == false)
                manys++; //to account for the current card as well because of this part ReshuffleAllCardsFromDiscard
            int expected = SaveRoot!.CardsToShuffle;
            //try here to see if it reconciles.  so if it does not, can hopefully get to the root of the problem each time.
            if (manys != expected)
                throw new BasicBlankException("After playing on pile, does not reconcile for the discard pile.  Rethink further");
            await ContinueTurnAsync(); //since this will check first to see if it has to draw 5 more cards.
        }
    }
}