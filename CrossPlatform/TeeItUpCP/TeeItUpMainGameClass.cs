using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace TeeItUpCP
{
    [SingletonGame]
    public class TeeItUpMainGameClass : CardGameClass<TeeItUpCardInformation, TeeItUpPlayerItem, TeeItUpSaveInfo>, IMiscDataNM, IStartNewGame
    {
        public TeeItUpMainGameClass(IGamePackageResolver container) : base(container) { }

        private TeeItUpViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<TeeItUpViewModel>();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            SaveRoot!.LoadMod(_thisMod!);
            LoadPlayerBoards();
            _thisMod!.Instructions = "Please choose the 2 cards to turn face up";
            PlayerList!.ForEach(thisPlayer => thisPlayer.PlayerBoard!.ClearBoard(thisPlayer.MainHandList));
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            SaveRoot!.Round++;
            LoadPlayerBoards();
            SaveRoot.LoadMod(_thisMod!);
            SaveRoot.ImmediatelyStartTurn = true;
            SaveRoot.Begins = WhoTurn;
            SaveRoot.GameStatus = EnumStatusType.Beginning;
            _thisMod!.Instructions = "Please choose the 2 cards to turn face up";
            return base.StartSetUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "firstcard":
                    var thisList = await GetSentListAsync(content);
                    if (thisList.Count != 2)
                        throw new BasicBlankException("Needs 2 cards");
                    await thisList.ForEachAsync(async thisSend =>
                    {
                        await BeginningCardSelectedAsync(thisSend.Deck, thisSend.Player);
                    });
                    return; //should be smart enough after number 2 to do what is needed.
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
            this.RoundOverNext();
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
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if (SaveRoot!.GameStatus == EnumStatusType.Normal)
            {
                if (HasGoneOut == true)
                {
                    await _thisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} has gone out.  Therefore, everyone gets one last turn");
                    SingleInfo.WentOut = true;
                    _thisMod.Instructions = "Last Turn";
                    SaveRoot.Begins = WhoTurn;
                    SaveRoot.GameStatus = EnumStatusType.Finished;
                }
            }
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
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
        public async override Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.FirstMulligan = false;
            PreviousCard = 0;
            if (SaveRoot.GameStatus == EnumStatusType.Beginning)
            {
                _thisMod!.NormalTurn = "None";
                _thisMod.firstList.Clear();
                if (ThisData!.MultiPlayer == true)
                    ThisCheck!.IsEnabled = false; //just in case.
                await ShowHumanCanPlayAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PlayerBoard!.ClearBoard(thisPlayer.MainHandList);
                thisPlayer.FinishedChoosing = false;
                thisPlayer.WentOut = false;

            });
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        private void LoadPlayerBoards()
        {
            if (PlayerList.First().PlayerBoard != null)
                return;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.LoadPlayerBoard(_thisMod!);
                thisPlayer.PlayerBoard!.SendEnableProcesses(_thisMod!, () =>
                {
                    if (ThisData!.MultiPlayer == false)
                        return true;
                    if (SaveRoot!.GameStatus != EnumStatusType.Beginning)
                        return true;
                    return thisPlayer.PlayerCategory == EnumPlayerCategory.Self;
                });
            });
        }
        internal async Task BoardClicked(TeeItUpPlayerItem thisPlayer, TeeItUpCardInformation thisCard)
        {
            int playerID = thisPlayer.Id;
            int deck = thisCard.Deck;
            if (SaveRoot!.GameStatus == EnumStatusType.Beginning)
            {
                if (thisPlayer.PlayerBoard!.CanStart)
                {
                    await _thisMod!.ShowGameMessageAsync("You already selected the 2 cards");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                {
                    SingleInfo = PlayerList!.GetSelf();
                    playerID = SingleInfo.Id;
                    AddFirstPlay(deck, playerID);
                }
                await BeginningCardSelectedAsync(deck, playerID);
                return;
            }
            bool knowns = thisPlayer.PlayerBoard!.IsCardKnown(deck);
            if (thisCard.IsMulligan == true && knowns == true)
            {
                if (SaveRoot.GameStatus != EnumStatusType.FirstTurn)
                {
                    await _thisMod!.ShowGameMessageAsync("This is not the first turn.  Therefore, the Mulligan card is not valid");
                    return;
                }
                if (_thisMod!.Pile2!.PileEmpty() == false)
                {
                    await _thisMod.ShowGameMessageAsync("Sorry, since you already drew, cannot use the Mulligan");
                    return;
                }
                if (thisPlayer.PlayerBoard.IsSelf == false)
                {
                    await _thisMod.ShowGameMessageAsync("Sorry, cannot use someone else's Mulligan");
                    return;
                }
                if (thisPlayer.PlayerBoard.IsMulliganValid(deck) == false)
                {
                    await _thisMod.ShowGameMessageAsync("Sorry, the mulligan cannot be used because someone traded with you");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("firstmulliganchosen", deck);
                await UseMulliganFirstTurnAsync(deck);
                return;
            }
            bool selfs = thisPlayer.PlayerBoard.IsSelf;
            if (SaveRoot.FirstMulligan)
            {
                if (selfs)
                {
                    await _thisMod!.ShowGameMessageAsync("Since you are using a Mulligan, you must trade with another player");
                    return;
                }
                if (thisPlayer.PlayerBoard.CanStealCard(deck) == false)
                {
                    await _thisMod!.ShowGameMessageAsync("The card chosen cannot be stolen");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await SendPlayAsync("firstmulliganplayed", deck, playerID);
                await FirstMulliganTradeAsync(playerID, deck);
                return;
            }
            int oldDeck;
            if (_thisMod!.Pile2!.PileEmpty() == true)
                oldDeck = 0;
            else
                oldDeck = _thisMod.Pile2.GetCardInfo().Deck;
            if (thisCard.IsMulligan && knowns)
            {
                await _thisMod.ShowGameMessageAsync("A mulligan card cannot be used for trading");
                return;
            }
            if (oldDeck == 0 && selfs == false)
            {
                await _thisMod.ShowGameMessageAsync("Cannot steal any card because no card is chosen");
                return;
            }
            if (selfs == false)
            {
                var previousCard = _thisMod.Pile2.GetCardInfo();
                if (previousCard.IsMulligan == false)
                {
                    await _thisMod.ShowGameMessageAsync("Since the card is not a mulligan, cannot steal another player's card");
                    return;
                }
                if (thisPlayer.PlayerBoard.CanStealCard(deck) == false)
                {
                    await _thisMod.ShowGameMessageAsync("The card cannot be stolen from another player");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await SendPlayAsync("stealcard", deck, playerID);
                await StealCardAsync(playerID, deck);
                return;
            }
            if (oldDeck == 0)
            {
                await _thisMod.ShowGameMessageAsync("Sorry, you must choose to pickup from discard or the deck first");
                return;
            }
            if (thisPlayer.PlayerBoard.IsPartFrozen(deck))
            {
                await _thisMod.ShowGameMessageAsync("Since the column is frozen, cannot trade it anymore for the round");
                return;
            }
            if (HasMatch(thisPlayer.PlayerBoard, deck, knowns) == false) //hopefully this is still correct.
            {
                await _thisMod.ShowGameMessageAsync("There is a match.  Therefore, must return a match");
                return;
            }
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("tradecard", deck);
            await TradeCardAsync(deck);
        }
        public async Task SendPlayAsync(string status, int deck, int player)
        {
            SendPlay thisSend = new SendPlay();
            thisSend.Deck = deck;
            thisSend.Player = player;
            await ThisNet!.SendAllAsync(status, thisSend);
        }
        public async Task SendFirstMoveAsync()
        {
            if (_thisMod!.firstList.Count != 2)
                throw new BasicBlankException("There must be 2 cards for your first move.");
            await ThisNet!.SendAllAsync("firstcard", _thisMod.firstList);
        }
        public void AddFirstPlay(int deck, int player)
        {
            SendPlay thisSend = new SendPlay();
            thisSend.Deck = deck;
            thisSend.Player = player;
            _thisMod!.firstList.Add(thisSend);
        }
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
            if (SingleInfo.PlayerBoard.CanStart == false)
            {
                return; //hopefully this simple.
            }
            SingleInfo.FinishedChoosing = true;
            if (PlayerList.Any(items => items.FinishedChoosing == false))
            {
                if (ThisData!.MultiPlayer == true)
                {
                    _thisMod!.CommandContainer!.ManuelFinish = true; //can't show finished anymore because has to wait for other players.
                    _thisMod.Instructions = "Waiting for the other players to finish choosing the 2 cards";
                    await SendFirstMoveAsync();
                    ThisCheck!.IsEnabled = true;
                    return;
                }
                _thisMod!.Instructions = "You must finish selecting the 2 cards for the other players";
                return; //i think
            }
            _thisMod!.CommandContainer!.ManuelFinish = true;
            SaveRoot!.GameStatus = EnumStatusType.FirstTurn;
            _thisMod.NormalTurn = "None";
            _thisMod.Instructions = "None";
            await StartNewTurnAsync();
        }
        public async Task FirstMulliganTradeAsync(int player, int deck)
        {
            if (SaveRoot!.FirstMulligan == false)
                throw new BasicBlankException("There was no first mulligan");
            TeeItUpPlayerItem thisPlayer = PlayerList![player];
            thisPlayer.PlayerBoard!.TradeCard(deck, _thisMod!.Pile2!.GetCardInfo().Deck);
            SingleInfo = PlayerList.GetWhoPlayer();
            SingleInfo.PlayerBoard!.ReplaceFirstWith(deck);
            _thisMod.Pile2.ClearCards();
            await EndTurnAsync();
        }
        public async Task UseMulliganFirstTurnAsync(int deck)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.PlayerBoard!.UseFirstMulligan(deck);
            TeeItUpCardInformation thisCard = new TeeItUpCardInformation();
            thisCard.Populate(deck);
            _thisMod!.Pile2!.AddCard(thisCard);
            SaveRoot!.FirstMulligan = true;
            await ContinueTurnAsync();
        }
        public async Task StealCardAsync(int player, int deck)
        {
            TeeItUpPlayerItem thisPlayer = PlayerList![player];
            thisPlayer.PlayerBoard!.TradeCard(deck, _thisMod!.Pile2!.GetCardInfo().Deck);
            var thisCard = DeckList!.GetSpecificItem(deck);
            _thisMod.Pile2.AddCard(thisCard);
            await ContinueTurnAsync();
        }
        public bool HasMatch(TeeItUpPlayerBoardCP thisBoard, int decks, bool isKnowns)
        {
            int matches = thisBoard.ColumnMatched(_thisMod!.Pile2!.GetCardInfo().Deck);
            if (matches == 0)
                return true;
            if (isKnowns == false)
                return false;
            var thisCard = _thisMod.Pile2.GetCardInfo();
            var newCard = DeckList!.GetSpecificItem(decks);
            return thisCard.Points == newCard.Points;
        }
        public async Task TradeCardAsync(int deck)
        {
            int matches = SingleInfo!.PlayerBoard!.ColumnMatched(_thisMod!.Pile2!.GetCardInfo().Deck);
            if (HasMatch(SingleInfo.PlayerBoard, deck, true) == false || matches == 0)
            {
                SingleInfo.PlayerBoard.TradeCard(deck, _thisMod.Pile2.GetCardInfo().Deck);
                TeeItUpCardInformation tempCard = new TeeItUpCardInformation();
                tempCard.Populate(deck);
                await DiscardAsync(tempCard);
                return;
            }
            SingleInfo.PlayerBoard.MatchCard(deck, out int newDeck);
            if (newDeck == 0)
                throw new BasicBlankException("When trading card, newdeck cannot be 0");
            TeeItUpCardInformation finCard = new TeeItUpCardInformation();
            finCard.Populate(newDeck);
            await DiscardAsync(finCard);
        }
    }
}