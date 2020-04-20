using BasicGameFrameworkLibrary.Attributes;
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
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TeeItUpCP.Cards;
using TeeItUpCP.Data;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace TeeItUpCP.Logic
{
    [SingletonGame]
    public class TeeItUpMainGameClass : CardGameClass<TeeItUpCardInformation, TeeItUpPlayerItem, TeeItUpSaveInfo>, IMiscDataNM, IStartNewGame
    {


        private readonly TeeItUpVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly TeeItUpGameContainer _gameContainer; //if we don't need it, take it out.

        public TeeItUpMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            TeeItUpVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<TeeItUpCardInformation> cardInfo,
            CommandContainer command,
            TeeItUpGameContainer gameContainer)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _gameContainer.BoardClickedAsync = BoardClicked;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            SaveRoot!.LoadMod(_model!);

            //_model!.Instructions = "Please choose the 2 cards to turn face up";
            
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            //if there is nothing, then just won't do anything.
            await Task.CompletedTask;
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            SaveRoot!.Round++;
            SaveRoot.LoadMod(_model!);
            SaveRoot.ImmediatelyStartTurn = true;
            SaveRoot.Begins = WhoTurn;
            SaveRoot.GameStatus = EnumStatusType.Beginning;
            //_model!.Instructions = "Please choose the 2 cards to turn face up";
            //when you choose one, it has to send information to the other player.
            //so when you go out and go back in, can have autoresume and no cheating.

            return base.StartSetUpAsync(isBeginning);
        }
        public override Task ContinueTurnAsync()
        {
            if (SaveRoot.GameStatus == EnumStatusType.Beginning)
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                {
                    _model.Instructions = "Please choose the 2 cards to turn face up";
                }
                else
                {
                    _model.Instructions = $"Waiting for {SingleInfo.NickName} to choose the 2 cards to turn face up";
                }
            }
            return base.ContinueTurnAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "firstcard":

                    //very iffy

                    SendPlay send = await GetSentPlayAsync(content);
                    await BeginningCardSelectedAsync(send.Deck, send.Player);
                    return; //hopefully this simple.
                            //var thisList = await GetSentListAsync(content);
                            //if (thisList.Count != 2)
                            //    throw new BasicBlankException("Needs 2 cards");
                            //await thisList.ForEachAsync(async thisSend =>
                            //{
                            //    await BeginningCardSelectedAsync(thisSend.Deck, thisSend.Player);
                            //});
                            //return; //should be smart enough after number 2 to do what is needed.
                case "firstmulliganchosen":
                    await UseMulliganFirstTurnAsync(int.Parse(content));
                    return;
                case "firstmulliganplayed":
                    var thisSend2 = await GetSentPlayAsync(content);
                    await FirstMulliganTradeAsync(thisSend2.Player, thisSend2.Deck);
                    return;
                case "stealcard":
                    var thisSend3 = await GetSentPlayAsync(content);
                    await StealCardAsync(thisSend3.Player, thisSend3.Deck);
                    return;
                case "tradecard":
                    await TradeCardAsync(int.Parse(content));
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        private bool HasGoneOut
        {
            get
            {
                return SingleInfo!.PlayerBoard!.IsFinished;
            }
        }
        public override async Task EndRoundAsync()
        {
            CalculateScore();
            if (SaveRoot!.Round == 9)
            {
                SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
                await ShowWinAsync();
                return;
            }
            await this.RoundOverNextAsync();
        }
        private void CalculateScore()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                var finalList = thisPlayer.PlayerBoard!.GetFinalCards();
                thisPlayer.PreviousScore = finalList.Sum(items => items.Points);
                thisPlayer.TotalScore += thisPlayer.PreviousScore;
            });
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.FirstMulligan = false;
            _gameContainer.PreviousCard = 0;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            PlayerList!.ForEach(tempPlayer =>
            {
                tempPlayer.PlayerBoard!.DoubleCheck();
            });
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if (SaveRoot!.GameStatus == EnumStatusType.Normal)
            {
                if (HasGoneOut == true)
                {
                    await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} has gone out.  Therefore, everyone gets one last turn");
                    SingleInfo.WentOut = true;
                    _model.Instructions = "Last Turn";
                    SaveRoot.Begins = WhoTurn;
                    SaveRoot.GameStatus = EnumStatusType.Finished;
                }
            }
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            if (SaveRoot.GameStatus == EnumStatusType.Finished && WhoTurn == SaveRoot.Begins)
            {
                await EndRoundAsync();
                return;
            }
            if (SaveRoot.GameStatus == EnumStatusType.FirstTurn && WhoTurn == SaveRoot.Begins)
                SaveRoot.GameStatus = EnumStatusType.Normal;
            await StartNewTurnAsync();
        }

        //iffy is continueturn maybe.


        public override Task PopulateSaveRootAsync()
        {
            PlayerList.ForEach(player =>
            {
                if (player.PlayerBoard != null)
                {
                    player.MainHandList = player.PlayerBoard!.ObjectList.ToObservableDeckDict();
                }
            });
            return base.PopulateSaveRootAsync();
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MainHandList.ForEach(card =>
                {
                    card.IsUnknown = true;
                    card.IsSelected = false;
                    card.Visible = true;
                    card.MulliganUsed = false;
                });
                //thisPlayer.PlayerBoard!.ClearBoard(thisPlayer.MainHandList);
                thisPlayer.FinishedChoosing = false;
                thisPlayer.WentOut = false;
            });
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        internal async Task BoardClicked(TeeItUpPlayerItem thisPlayer, TeeItUpCardInformation thisCard)
        {
            int playerID = thisPlayer.Id;
            int deck = thisCard.Deck;
            try
            {
                if (SaveRoot!.GameStatus == EnumStatusType.Beginning)
                {
                    if (thisPlayer.PlayerBoard!.CanStart)
                    {
                        throw new BasicBlankException("Should have not allowed this since 2 cards was already selected");
                        //await _thisMod!.ShowGameMessageAsync("You already selected the 2 cards");
                        //return
                    }
                    //if (BasicData!.MultiPlayer == true)
                    //{
                    //    SingleInfo = PlayerList!.GetSelf();
                    //    playerID = SingleInfo.Id;
                    //    AddFirstPlay(deck, playerID);
                    //}
                    await BeginningCardSelectedAsync(deck, playerID);
                    return;
                }
                bool knowns = thisPlayer.PlayerBoard!.IsCardKnown(deck);
                if (thisCard.IsMulligan == true && knowns == true)
                {
                    if (SaveRoot.GameStatus != EnumStatusType.FirstTurn)
                    {
                        await UIPlatform.ShowMessageAsync("This is not the first turn.  Therefore, the Mulligan card is not valid");
                        return;
                    }
                    if (_model!.OtherPile!.PileEmpty() == false)
                    {
                        await UIPlatform.ShowMessageAsync("Sorry, since you already drew, cannot use the Mulligan");
                        return;
                    }
                    if (thisPlayer.PlayerBoard.IsSelf == false)
                    {
                        await UIPlatform.ShowMessageAsync("Sorry, cannot use someone else's Mulligan");
                        return;
                    }
                    if (thisPlayer.PlayerBoard.IsMulliganValid(deck) == false)
                    {
                        await UIPlatform.ShowMessageAsync("Sorry, the mulligan cannot be used because someone traded with you");
                        return;
                    }
                    if (BasicData!.MultiPlayer == true)
                        await Network!.SendAllAsync("firstmulliganchosen", deck);
                    await UseMulliganFirstTurnAsync(deck);
                    return;
                }
                bool selfs = thisPlayer.PlayerBoard.IsSelf;
                if (SaveRoot.FirstMulligan)
                {
                    if (selfs)
                    {
                        await UIPlatform.ShowMessageAsync("Since you are using a Mulligan, you must trade with another player");
                        return;
                    }
                    if (thisPlayer.PlayerBoard.CanStealCard(deck) == false)
                    {
                        await UIPlatform.ShowMessageAsync("The card chosen cannot be stolen");
                        return;
                    }
                    if (BasicData!.MultiPlayer == true)
                        await SendPlayAsync("firstmulliganplayed", deck, playerID);
                    await FirstMulliganTradeAsync(playerID, deck);
                    return;
                }
                int oldDeck;
                if (_model!.OtherPile!.PileEmpty() == true)
                    oldDeck = 0;
                else
                    oldDeck = _model.OtherPile.GetCardInfo().Deck;
                if (thisCard.IsMulligan && knowns)
                {
                    await UIPlatform.ShowMessageAsync("A mulligan card cannot be used for trading");
                    return;
                }
                if (oldDeck == 0 && selfs == false)
                {
                    await UIPlatform.ShowMessageAsync("Cannot steal any card because no card is chosen");
                    return;
                }
                if (selfs == false)
                {
                    var previousCard = _model.OtherPile.GetCardInfo();
                    if (previousCard.IsMulligan == false)
                    {
                        await UIPlatform.ShowMessageAsync("Since the card is not a mulligan, cannot steal another player's card");
                        return;
                    }
                    if (thisPlayer.PlayerBoard.CanStealCard(deck) == false)
                    {
                        await UIPlatform.ShowMessageAsync("The card cannot be stolen from another player");
                        return;
                    }
                    if (BasicData!.MultiPlayer == true)
                        await SendPlayAsync("stealcard", deck, playerID);
                    await StealCardAsync(playerID, deck);
                    return;
                }
                if (oldDeck == 0)
                {
                    await UIPlatform.ShowMessageAsync("Sorry, you must choose to pickup from discard or the deck first");
                    return;
                }
                if (thisPlayer.PlayerBoard.IsPartFrozen(deck))
                {
                    await UIPlatform.ShowMessageAsync("Since the column is frozen, cannot trade it anymore for the round");
                    return;
                }
                if (HasMatch(thisPlayer.PlayerBoard, deck, knowns) == false) //hopefully this is still correct.
                {
                    await UIPlatform.ShowMessageAsync("There is a match.  Therefore, must return a match");
                    return;
                }
                if (BasicData!.MultiPlayer == true)
                    await Network!.SendAllAsync("tradecard", deck);
                await TradeCardAsync(deck);
            }
            catch (Exception ex)
            {
                UIPlatform.ShowError(ex.Message);
            }
            
        }
        public async Task SendPlayAsync(string status, int deck, int player)
        {
            SendPlay thisSend = new SendPlay();
            thisSend.Deck = deck;
            thisSend.Player = player;
            await Network!.SendAllAsync(status, thisSend);
        }
        //public async Task SendFirstMoveAsync()
        //{
        //    if (_thisMod!.firstList.Count != 2)
        //        throw new BasicBlankException("There must be 2 cards for your first move.");
        //    await ThisNet!.SendAllAsync("firstcard", _thisMod.firstList);
        //}
        //public void AddFirstPlay(int deck, int player)
        //{
        //    SendPlay thisSend = new SendPlay();
        //    thisSend.Deck = deck;
        //    thisSend.Player = player;
        //    _thisMod!.firstList.Add(thisSend);
        //}
        public async Task<CustomBasicList<SendPlay>> GetSentListAsync(string message)
        {
            return await js.DeserializeObjectAsync<CustomBasicList<SendPlay>>(message);
        }
        public async Task<SendPlay> GetSentPlayAsync(string message)
        {
            return await js.DeserializeObjectAsync<SendPlay>(message);
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PreviousScore = 0;
                thisPlayer.TotalScore = 0;
            });
            SaveRoot!.Round = 0;
            return Task.CompletedTask;
        }
        public override Task DiscardAsync(TeeItUpCardInformation ThisCard)
        {
            ThisCard.IsUnknown = false;
            return base.DiscardAsync(ThisCard);
        }
        public async Task BeginningCardSelectedAsync(int deck, int player)
        {
            SingleInfo = PlayerList![player];
            SingleInfo.PlayerBoard!.ChooseCard(deck);
            if (SingleInfo.CanSendMessage(BasicData))
            {
                SendPlay send = new SendPlay()
                {
                    Deck = deck,
                    Player = player
                };
                await Network!.SendAllAsync("firstcard", send);
            }
            if (SingleInfo.PlayerBoard.CanStart == false)
            {
                await ContinueTurnAsync();
                return; //hopefully this simple.
            }
            SingleInfo.FinishedChoosing = true;
            


            if (PlayerList.Any(items => items.FinishedChoosing == false))
            {
                await EndTurnAsync(); //try this way.
                return;
                //if (ThisData!.MultiPlayer == true)
                //{
                //    _thisMod!.CommandContainer!.ManuelFinish = true; //can't show finished anymore because has to wait for other players.
                //    _thisMod.Instructions = "Waiting for the other players to finish choosing the 2 cards";
                //    await SendFirstMoveAsync();
                //    ThisCheck!.IsEnabled = true;
                //    return;
                //}
                //_thisMod!.Instructions = "You must finish selecting the 2 cards for the other players";
                //return; //i think
            }
            _command.ManuelFinish = true;
            SaveRoot!.GameStatus = EnumStatusType.FirstTurn;
            _model.NormalTurn = "None";
            _model.Instructions = "None";
            await EndTurnAsync();
        }
        public async Task FirstMulliganTradeAsync(int player, int deck)
        {
            if (SaveRoot!.FirstMulligan == false)
                throw new BasicBlankException("There was no first mulligan");
            TeeItUpPlayerItem thisPlayer = PlayerList![player];
            thisPlayer.PlayerBoard!.TradeCard(deck, _model!.OtherPile!.GetCardInfo().Deck);
            SingleInfo = PlayerList.GetWhoPlayer();
            SingleInfo.PlayerBoard!.ReplaceFirstWith(deck);
            _model.OtherPile.ClearCards();
            await EndTurnAsync();
        }
        public async Task UseMulliganFirstTurnAsync(int deck)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.PlayerBoard!.UseFirstMulligan(deck);
            TeeItUpCardInformation thisCard = new TeeItUpCardInformation();
            thisCard.Populate(deck);
            _model!.OtherPile!.AddCard(thisCard);
            SaveRoot!.FirstMulligan = true;
            await ContinueTurnAsync();
        }
        public async Task StealCardAsync(int player, int deck)
        {
            TeeItUpPlayerItem thisPlayer = PlayerList![player];
            thisPlayer.PlayerBoard!.TradeCard(deck, _model!.OtherPile!.GetCardInfo().Deck);
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            _model.OtherPile.AddCard(thisCard);
            await ContinueTurnAsync();
        }
        public bool HasMatch(TeeItUpPlayerBoardCP thisBoard, int decks, bool isKnowns)
        {
            int matches = thisBoard.ColumnMatched(_model!.OtherPile!.GetCardInfo().Deck);
            if (matches == 0)
                return true;
            if (isKnowns == false)
                return false;
            var thisCard = _model.OtherPile.GetCardInfo();
            var newCard = _gameContainer.DeckList!.GetSpecificItem(decks);
            return thisCard.Points == newCard.Points;
        }
        public async Task TradeCardAsync(int deck)
        {
            try
            {
                int matches = SingleInfo!.PlayerBoard!.ColumnMatched(_model!.OtherPile!.GetCardInfo().Deck);
                if (HasMatch(SingleInfo.PlayerBoard, deck, true) == false || matches == 0)
                {
                    SingleInfo.PlayerBoard.TradeCard(deck, _model.OtherPile.GetCardInfo().Deck);
                    TeeItUpCardInformation tempCard = new TeeItUpCardInformation();
                    tempCard.Populate(deck);
                    await DiscardAsync(tempCard);
                    return;
                }
                SingleInfo.PlayerBoard.MatchCard(deck, out int newDeck);
                if (newDeck == 0)
                    UIPlatform.ShowError("When trading card, newdeck cannot be 0");
                TeeItUpCardInformation finCard = new TeeItUpCardInformation();
                finCard.Populate(newDeck);
                await DiscardAsync(finCard);
            }
            catch (Exception ex)
            {
                UIPlatform.ShowError(ex.Message);
            }
            
        }

    }
}
