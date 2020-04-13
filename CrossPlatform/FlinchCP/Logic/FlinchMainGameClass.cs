using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FlinchCP.Cards;
using FlinchCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace FlinchCP.Logic
{
    [SingletonGame]
    public class FlinchMainGameClass : CardGameClass<FlinchCardInformation, FlinchPlayerItem, FlinchSaveInfo>, IMiscDataNM
    {


        private readonly FlinchVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly FlinchGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly FlinchComputerAI _ai;
        internal CustomBasicList<ComputerData> ComputerList { get; set; } = new CustomBasicList<ComputerData>(); //the computer ai needs it.
        public FlinchMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            FlinchVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<FlinchCardInformation> cardInfo,
            CommandContainer command,
            FlinchGameContainer gameContainer,
            FlinchComputerAI ai
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _ai = ai;
            _model.PublicPiles.PileClickedAsync += PublicPiles_PileClickedAsync;
            _gameContainer.IsValidMove = IsValidMove;
        }
        private async Task PublicPiles_PileClickedAsync(int index)
        {
            if (SaveRoot!.GameStatus == EnumStatusList.DiscardAll)
            {
                await UIPlatform.ShowMessageAsync("Sorry; you must discard all your cards");
                return;
            }
            int decks = CardSelected(out EnumCardType types, out int discardNum);
            if (decks == 0 && types == EnumCardType.IsNone)
            {
                await UIPlatform.ShowMessageAsync("Sorry, there was nothing selected");
                return;
            }
            if (decks == 0)
                throw new BasicBlankException("Nothing selected but the type was not none");
            bool rets = IsValidMove(index, decks);
            if (rets == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            await PlayOnPileAsync(index, decks, types, discardNum);
        }
        public override Task FinishGetSavedAsync()
        {
            SaveRoot!.LoadMod(_model!);
            LoadControls();
            _model.PublicPiles!.PileList!.ReplaceRange(SaveRoot.PublicPileList);
            return base.FinishGetSavedAsync();
        }
        protected override Task MiddleReshuffleCardsAsync(IDeckDict<FlinchCardInformation> thisList, bool canSend)
        {
            if (thisList.Count != SaveRoot!.CardsToShuffle)
                throw new BasicBlankException($"Must have {SaveRoot!.CardsToShuffle}, not {thisList.Count}");
            var nextList = _model!.Pile1!.DiscardList();
            if (nextList.Count > 0)
                throw new BasicBlankException("The discard list somehow did not get cleared out");
            return base.MiddleReshuffleCardsAsync(thisList, canSend);
        }
        protected override void PrepStartTurn()
        {
            base.PrepStartTurn();
            ComputerList = new CustomBasicList<ComputerData>(); //because we don't have autoresume computer data for now.
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        private async Task ComputerDiscardAsync()
        {
            FlinchComputerAI.ComputerDiscardInfo thisDiscard;
            thisDiscard = _ai!.ComputerDiscard();
            await AddToDiscardAsync(thisDiscard.Pile, thisDiscard.Deck);
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
                await Delay!.DelayMilli(250);
            _ai!.MaxPiles = _model.PublicPiles!.MaxPiles();
            if (ComputerList.Count == 0)
            {
                ComputerList = _ai!.ComputerMoves();
                if (ComputerList.Count == 0)
                {
                    if (SaveRoot.GameStatus == EnumStatusList.FirstOne)
                    {
                        if (BasicData!.MultiPlayer == true)
                            await Network!.SendEndTurnAsync();
                        await EndTurnAsync();
                        return;
                    }
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
            LoadControls();
            ComputerList.Clear();
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            SaveRoot!.CardsToShuffle = 0;
            _model.PublicPiles!.ClearBoard();
            SaveRoot.GameStatus = EnumStatusList.FirstOne;
            SaveRoot.LoadMod(_model!); //hopefully this works.
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.DiscardList.Clear(); //hopefully this simple (?)
                //thisPlayer.StockPile!.ClearCards();
                //thisPlayer.DiscardPiles!.ClearBoard();

                thisPlayer.StockList.ReplaceRange(thisPlayer.MainHandList);

                //thisPlayer.MainHandList.ForEach(ThisCard =>
                //{
                //    thisPlayer.StockPile.AddCard(ThisCard);
                //});
                thisPlayer.MainHandList.Clear();
                thisPlayer.StockLeft = thisPlayer.StockList.Count;
                thisPlayer.InStock = thisPlayer.StockList.Last().Display;
                //UpdateStockData(thisPlayer); //i think here is correct.  well see.
                thisPlayer.Discard1 = "0";
                thisPlayer.Discard2 = "0";
                thisPlayer.Discard3 = "0";
                thisPlayer.Discard4 = "0";
                thisPlayer.Discard5 = "0";
            });
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "discardnew":
                    SendDiscard discard = await js.DeserializeObjectAsync<SendDiscard>(content);
                    await AddToDiscardAsync(discard.Pile, discard.Deck);
                    return;
                case "play":
                    SendPlay play = await js.DeserializeObjectAsync<SendPlay>(content);
                    await PlayOnPileAsync(play.Pile, play.Deck, play.WhichType, play.Discard);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (_gameContainer.LoadPlayerPilesAsync != null)
            {
                await _gameContainer.LoadPlayerPilesAsync.Invoke(); //i think.
            }
            await StartDrawingAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if (Test!.ComputerEndsTurn == false && SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
            {
                if (SingleInfo.MainHandList.Count > 4 && (SaveRoot!.GameStatus == EnumStatusList.DiscardOneOnly || SaveRoot.GameStatus == EnumStatusList.Normal))
                    throw new BasicBlankException("Cannot have 5 cards left at the end of this turn");
                else if (SingleInfo.MainHandList.Count > 0 && SaveRoot!.GameStatus == EnumStatusList.DiscardAll)
                    throw new BasicBlankException("Must discard all the cards based on the game status");
            }
            SingleInfo.StockList = _model.StockPile!.GetStockList();
            SingleInfo.DiscardList = _model.DiscardPiles!.PileList!.ToCustomBasicList();

            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            if (SaveRoot!.PlayerFound > 0 && SingleInfo.MainHandList.Count == 5)
                SaveRoot.GameStatus = EnumStatusList.DiscardOneOnly;
            else if ((SaveRoot.PlayerFound == 0) & (SingleInfo.MainHandList.Count == 5))
                SaveRoot.GameStatus = EnumStatusList.DiscardAll;
            else if ((SingleInfo.MainHandList.Count < 5) & (SaveRoot.PlayerFound > 0))
                SaveRoot.GameStatus = EnumStatusList.Normal;
            this.ShowTurn();
            await SaveStateAsync();
            await StartNewTurnAsync();
        }
        public async override Task PopulateSaveRootAsync()
        {
            //PlayerList!.ForEach(thisPlayer =>
            //{
            //    thisPlayer.StockList = thisPlayer.StockPile!.StockFrame.PileList.ToCustomBasicList();
            //    thisPlayer.DiscardList = thisPlayer.DiscardPiles!.PileList.ToCustomBasicList();
            //});
            SaveRoot!.PublicPileList = _model.PublicPiles.PileList!.ToCustomBasicList();
            //hopefully no problem with the player stocks.
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
            //SingleInfo = PlayerList!.GetWhoPlayer();
            _model.StockPile!.UnselectCard();
            _model.PublicPiles!.UnselectAllPiles();
            _model!.PlayerHand1!.UnselectAllObjects();
            _model.DiscardPiles!.UnselectAllCards();
        }
        private void UpdateDiscardData(FlinchPlayerItem thisPlayer, int thisPile)
        {
            string thisNum;
            if (_model.DiscardPiles!.HasCard(thisPile) == false)
                thisNum = "0";
            else
            {
                var card = _model.DiscardPiles.GetLastCard(thisPile);
                thisNum = card.Display; // i think needs to be value.  So can even be W
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
        private bool HasOne()
        {
            if (_model.StockPile.NextCardInStock() == "1")
                return true;
            return SingleInfo!.MainHandList.Any(items => items.Number == 1);
        }
        private void RemoveFromHand(int deck)
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
            if (SingleInfo.MainHandList.Count == 5)
                throw new BasicBlankException($"After removing a card, must have less than 5 cards left for {SingleInfo.NickName}");
        }
        protected override async Task AfterDrawingAsync()
        {
            if (_wasNew == true)
                SingleInfo!.MainHandList.UnhighlightObjects();
            if (SingleInfo!.MainHandList.Any(items => items.IsUnknown))
                throw new BasicBlankException("Should never have unknown cards");
            if (HasOne() == true)
            {
                await AutomatePlayOneAsync();
            }
            await base.AfterDrawingAsync();
        }
        private async Task AutomatePlayOneAsync()
        {
            FlinchCardInformation thisCard;
            thisCard = SingleInfo!.MainHandList.FirstOrDefault(Items => Items.Number == 1);
            EnumCardType thisCat;
            if (thisCard == null)
            {
                thisCard = _model.StockPile.GetCard();
                thisCat = EnumCardType.Stock;
            }
            else
                thisCat = EnumCardType.MyCards;
            await PlayOnPileAsync(-1, thisCard.Deck, thisCat, -1);
        }
        protected override Task AfterReshuffleAsync()
        {
            SaveRoot!.CardsToShuffle = 0; //because reset now.
            return base.AfterReshuffleAsync();
        }
        private void RemoveFromPile(int pile)
        {
            var thisCol = _model.PublicPiles!.EmptyPileList(pile);
            thisCol.ForEach(thisCard =>
            {
                SaveRoot!.CardsToShuffle++;
                _model!.Pile1!.AddCard(thisCard); //for reshuffling eventually.
            });
        }
        public async Task AddToDiscardAsync(int pile, int deck)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(BasicData!) == true)
            {
                SendDiscard thisDiscard = new SendDiscard();
                thisDiscard.Deck = deck;
                thisDiscard.Pile = pile;
                await Network!.SendAllAsync("discardnew", thisDiscard);
            }
            if (SingleInfo.MainHandList.Count > 5)
                throw new BasicBlankException($"The hand must be 5 or less, not {SingleInfo.MainHandList.Count} for {SingleInfo.NickName}");
            RemoveFromHand(deck);
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            if (thisCard.IsUnknown)
                throw new BasicBlankException("Can not be unknown when playing on discard pile.  Rethink");
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            await AnimateDiscardAsync(thisCard, pile, EnumAnimcationDirection.StartUpToCard);
            _model.DiscardPiles!.AddCardToPile(pile, thisCard);
            UpdateDiscardData(SingleInfo, pile);
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            if (SaveRoot!.GameStatus == EnumStatusList.DiscardAll && SingleInfo.MainHandList.Count > 0)
            {
                await ContinueTurnAsync();
                return;
            }
            if (SaveRoot.GameStatus == EnumStatusList.DiscardAll && SingleInfo.MainHandList.Count == 0)
            {
                SaveRoot.GameStatus = EnumStatusList.FirstOne;
                await ContinueTurnAsync(); //since it will automatically draw 5 cards now
                return;
            }
            await EndTurnAsync();
        }
        public async Task AnimateDiscardAsync(FlinchCardInformation thisCard, int pile, EnumAnimcationDirection direction)
        {
            await Aggregator!.AnimateCardAsync(thisCard, direction, $"discard{SingleInfo!.NickName}", _model.DiscardPiles!.PileList![pile]);
        }
        public async Task AnimateStockAsync(FlinchCardInformation thisCard)
        {
            var thisPile = _model.StockPile!.StockFrame.PileList.Single();
            await Aggregator!.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartCardToUp, $"stock{SingleInfo!.NickName}", thisPile);
        }
        public bool IsValidMove(int pile, int deck)
        {
            if (pile == -1)
                throw new BasicBlankException("The pile cannot be -1 for a valid move");
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            int nexts = _model.PublicPiles!.NextNumberNeeded(pile);
            return nexts == thisCard.Number; //i don't we can allow any move because that can hose it up later.
        }
        public int CardSelected(out EnumCardType whatType, out int discardNum)
        {
            int thisNum = _model!.PlayerHand1!.ObjectSelected();
            if (thisNum > 0)
            {
                whatType = EnumCardType.MyCards;
                discardNum = -1;
                return thisNum;
            }
            thisNum = _model!.DiscardPiles!.CardSelected(out int thisPile);
            if (thisNum > 0)
            {
                whatType = EnumCardType.Discard;
                discardNum = thisPile;
                if (discardNum == -1)
                    throw new BasicBlankException("Must be something from the discard pile");
                return thisNum;
            }
            discardNum = -1;
            thisNum = _model.StockPile!.CardSelected();
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
            if (pile == -1 && SaveRoot!.PlayerFound == 0)
                SaveRoot.PlayerFound = WhoTurn;
            if (SingleInfo.CanSendMessage(BasicData!) == true && pile > -1)
            {
                SendPlay thisPlay = new SendPlay();
                thisPlay.Deck = deck;
                thisPlay.Pile = pile;
                thisPlay.WhichType = whichType;
                thisPlay.Discard = discardNum;
                await Network!.SendAllAsync("play", thisPlay);
            }
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            if (thisCard.IsUnknown)
                throw new BasicBlankException("Should not have been unknown.  Rethink");
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            if (whichType == EnumCardType.MyCards)
                RemoveFromHand(deck);
            else if (whichType == EnumCardType.Stock)
            {
                _model.StockPile!.RemoveCard();
                await AnimateStockAsync(thisCard);
                SingleInfo.StockLeft = _model.StockPile.CardsLeft();
                SingleInfo.InStock = _model.StockPile.NextCardInStock();
            }
            else
            {
                if (discardNum == -1)
                    throw new BasicBlankException("The discard number cannot be -1");
                if (_model.DiscardPiles!.PileList![discardNum].ObjectList.Count == 0)
                    throw new BasicBlankException("The discard must have at least one item to play from");
                _model.DiscardPiles!.RemoveCard(discardNum, deck);
                UpdateDiscardData(SingleInfo, discardNum);
                await AnimateDiscardAsync(thisCard, discardNum, EnumAnimcationDirection.StartCardToUp);
                _model.DiscardPiles!.UnselectAllCards();
            }
            if (pile > -1)
            {
                var npile = _model.PublicPiles!.PileList[pile];
                await Aggregator.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartDownToCard, "public", npile);
                _model.PublicPiles.AddCardToPile(pile, thisCard); //hopefully this is still okay.
                if (_model.PublicPiles.NeedToRemovePile(pile))
                {
                    if (Test!.NoAnimations == false)
                        await Delay!.DelaySeconds(1);
                    RemoveFromPile(pile);
                }
            }
            else
            {
                _model.PublicPiles!.CreateNewPile(thisCard);
            }
            if (whichType == EnumCardType.Stock)
            {
                if (_model.StockPile.DidGoOut() == true)
                {
                    await ShowWinAsync();
                    return;
                }
            }
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer && pile > -1)
            {
                if (BasicData!.MultiPlayer == false || BasicData.Client == false)
                    ComputerList.RemoveFirstItem(); //hopefully this is still fine (?)
            }
            if (HasOne() == true)
            {
                await AutomatePlayOneAsync();
                return; //because you have to play if you do have a one.
            }
            await ContinueTurnAsync(); //since this will check first to see if it has to draw 5 more cards.
        }
    }
}
