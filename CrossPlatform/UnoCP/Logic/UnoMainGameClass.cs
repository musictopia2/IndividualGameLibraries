using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using UnoCP.Cards;
using UnoCP.Data;

namespace UnoCP.Logic
{
    [SingletonGame]
    public class UnoMainGameClass : CardGameClass<UnoCardInformation, UnoPlayerItem, UnoSaveInfo>, IMiscDataNM, IFinishStart, IStartNewGame
    {


        private readonly UnoVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly UnoGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly UnoComputerAI _ai;
        private readonly IChooseColorProcesses _colorProcesses;
        private readonly ISayUnoProcesses _sayUnoProcesses;
        private readonly UnoColorsDelegates _delegates;
        private bool _playerDrew;
        private bool _finishPlay;
        private UnoCardInformation CurrentObject => _model!.Pile1!.CurrentCard;
        public UnoMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            UnoVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<UnoCardInformation> cardInfo,
            CommandContainer command,
            UnoGameContainer gameContainer,
            UnoComputerAI ai,
            IChooseColorProcesses colorProcesses,
            ISayUnoProcesses sayUnoProcesses,
            UnoColorsDelegates delegates
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _ai = ai;
            _colorProcesses = colorProcesses;
            _sayUnoProcesses = sayUnoProcesses;
            _delegates = delegates;
            _gameContainer.CanPlay = CanPlay;
            _gameContainer.DoFinishAsync = DoFinishAsync;
        }
        //public override bool CanMakeMainOptionsVisibleAtBeginning => SaveRoot.GameStatus != EnumGameStatus.ChooseColors;

        public override bool CanMakeMainOptionsVisibleAtBeginning
        {
            get
            {
                if (SaveRoot.GameStatus == EnumGameStatus.ChooseColors || SaveRoot.CurrentColor == EnumColorTypes.ZOther)
                {
                    return false;
                }
                return true;
            }
        }

        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            await base.FinishGetSavedAsync();
            int newTurn = await PlayerList!.CalculateWhoTurnAsync();
            UnoPlayerItem player = PlayerList[newTurn];
            _model!.NextPlayer = player.NickName;
        }
        private bool _wasStart;
        //private bool _wasNew;
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                throw new BasicBlankException("Can't play for self");
            if (SaveRoot!.GameStatus == EnumGameStatus.ChooseColors)
            {
                await Delay!.DelaySeconds(.5);
                await _colorProcesses.ColorChosenAsync(_ai.ColorChosen());
                return;
            }
            if (SaveRoot.GameStatus == EnumGameStatus.WaitingForUno)
            {
                await Delay!.DelaySeconds(.5);
                await _sayUnoProcesses.ProcessUnoAsync(true); //computer will always say uno
                return;
            }
            await Delay!.DelaySeconds(1);
            UnoCardInformation thisCard;
            int deck;
            if (_gameContainer.AlreadyDrew == true)
            {
                thisCard = SingleInfo.MainHandList.GetLastObjectDrawn();
                deck = thisCard.Deck;
                if (CanPlay(deck) == false)
                {
                    if (BasicData!.MultiPlayer == true)
                        await Network!.SendEndTurnAsync();
                    await EndTurnAsync();
                    return;
                }
                await ProcessPlayAsync(deck);
                return;
            }
            deck = _ai.ComputerMove();
            if (deck > 0)
            {
                await ProcessPlayAsync(deck);
                return;
            }
            if (BasicData!.MultiPlayer == true)
                await Network!.SendAllAsync("drawcard");
            await DrawAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            return base.StartSetUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "play":
                    await ProcessPlayAsync(int.Parse(content));
                    break;
                case "uno":
                    await _sayUnoProcesses.ProcessUnoAsync(bool.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            _gameContainer.AlreadyUno = false;
            _playerDrew = false;
            _finishPlay = false;
            if (SaveRoot.GameStatus == EnumGameStatus.WaitingForUno)
            {
                if (_gameContainer.CloseSaidUnoAsync == null)
                {
                    throw new BasicBlankException("Nobody is closing the uno screen.  Rethink");
                }
                await _gameContainer.CloseSaidUnoAsync.Invoke();
            }
            SaveRoot!.GameStatus = EnumGameStatus.NormalPlay;
            UnoCardInformation thisCard = CurrentObject;
            if (thisCard.WhichType == EnumCardTypeList.Skip && SaveRoot.HasSkipped == false ||
                thisCard.Draw > 0 && SaveRoot.HasDrawn == false)
            {
                if (thisCard.Draw == 0)
                {
                    await GoToNextPlayerAsync();
                    return;
                }
                _model!.NextPlayer = "";
                _playerDrew = true;
                PlayerDraws = WhoTurn;
                LeftToDraw = thisCard.Draw;
                await DrawAsync();
                return;
            }

            if (CanEndRound() == true)
            {
                await EndRoundAsync();
                return;
            }
            if (SaveRoot.CurrentColor == EnumColorTypes.ZOther)
            {
                throw new BasicBlankException("The current color can't be other");
            }
            int newTurn = await PlayerList!.CalculateWhoTurnAsync();
            UnoPlayerItem newPlayer = PlayerList[newTurn];
            _model!.NextPlayer = newPlayer.NickName;
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.

            //anything else is here.  varies by game.


            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await StartNewTurnAsync();
        }

        private async Task GoToNextPlayerAsync()
        {
            SaveRoot!.HasDrawn = true;
            SaveRoot.HasSkipped = true;
            _playerDrew = false;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }

        private bool CanEndRound()
        {
            // if true, then will end round
            if (_gameContainer.PlayerWentOut() == -1)
                return false;
            return true;
        }
        public bool CanPlay(int deck)
        {
            if (SaveRoot!.CurrentColor == EnumColorTypes.ZOther)
                return true; //hopefully means you can play because its other.
                //throw new BasicBlankException("Color cannot be other");
            if (SaveRoot.CurrentColor == EnumColorTypes.None)
                throw new BasicBlankException("Color cannot be none");
            if (Test!.AllowAnyMove == true)
                return true; //because its testing.
            UnoCardInformation thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            //don't have to worry about too many cards because you are always forced to play now anyways.
            if (thisCard.WhichType.Equals(EnumCardTypeList.Wild) && thisCard.Draw == 0)
                return true; //because you can always play a plain wild.
            if (thisCard.Color == SaveRoot.CurrentColor && thisCard.Color != EnumColorTypes.ZOther)
                return true;
            UnoCardInformation newCard = CurrentObject;
            if (newCard.Number == thisCard.Number && thisCard.Color != EnumColorTypes.ZOther)
                return true;
            if (thisCard.Draw == 4)
            {
                bool rets = SingleInfo!.MainHandList.Any(Items => Items.Color == SaveRoot.CurrentColor);
                if (rets == true)
                    return false;
                return true;
            }
            return false;

        }

        private async Task DoFinishAsync()
        {
            if (SingleInfo!.MainHandList.Count == 0)
            {
                await EndTurnAsync();
                return;
            }
            if (SingleInfo.MainHandList.Count == 1 && _gameContainer.AlreadyUno == false)
            {
                SaveRoot!.GameStatus = EnumGameStatus.WaitingForUno;
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                {
                    if (_gameContainer.OpenSaidUnoAsync == null)
                    {
                        throw new BasicBlankException("Nobody is handling the opening of saying uno screen.  Rethink");
                    }
                    await _gameContainer.OpenSaidUnoAsync.Invoke();
                }
                await ContinueTurnAsync();
                return;
            }
            UnoCardInformation thisCard = CurrentObject;
            if (thisCard.Color == EnumColorTypes.ZOther)
            {
                SaveRoot!.GameStatus = EnumGameStatus.ChooseColors;
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                {
                    if (_delegates.OpenColorAsync == null)
                    {
                        throw new BasicBlankException("Nobody is handling the opening of the color chooser screen.  Rethink");
                    }
                    await _delegates.OpenColorAsync.Invoke();
                }
                await ContinueTurnAsync();
                return;
            }
            SaveRoot!.CurrentColor = thisCard.Color;
            if (SaveRoot.CurrentColor == EnumColorTypes.ZOther)
                throw new BasicBlankException("Color can't be other");
            await EndTurnAsync();
        }

        public async Task ProcessPlayAsync(int deck)
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
            UnoCardInformation thisCard = _gameContainer.DeckList!.GetSpecificItem(deck); //i think
            PlaceDiscard(thisCard);
            if (SingleInfo.CanSendMessage(BasicData!) == true)
            {
                await Network!.SendAllAsync("play", deck); //try this way
            }
            _finishPlay = true;
            await ProcessPlayAsync(thisCard, deck);
        }
        private async Task ProcessPlayAsync(UnoCardInformation thisCard, int deck)
        {
            await AnimatePlayAsync(thisCard);
            SaveRoot!.HasSkipped = false;
            if (CurrentObject.Deck != deck)
                throw new BasicBlankException("This is not the card played.  That means there is a problem (possibly with the discard)");
            await DoFinishAsync();
        }
        private void PlaceDiscard(UnoCardInformation thisCard)
        {
            thisCard.Drew = false;
            if (thisCard.WhichType == EnumCardTypeList.Reverse)
                PlayerList!.ChangeReverse(); //decided to be part of playerlist now.
            SaveRoot!.HasDrawn = false;
            SaveRoot.HasSkipped = false;
        }
        protected async override Task AfterDrawingAsync()
        {
            if (_finishPlay == true)
            {
                await DoFinishAsync();
                return;
            }
            if (_playerDrew == true)
            {
                await GoToNextPlayerAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        public async Task FinishStartAsync()
        {
            SaveRoot!.CurrentColor = CurrentObject.Color;

            if (CurrentObject.WhichType == EnumCardTypeList.Reverse && PlayerList.Count() > 2)
            {
                SaveRoot.PlayOrder.IsReversed = true;
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync(); //i think
            }
            else if (CurrentObject.WhichType == EnumCardTypeList.Wild && CurrentObject.Color == EnumColorTypes.ZOther)
            {
                SaveRoot.GameStatus = EnumGameStatus.ChooseColors; //i think
                if (_delegates.OpenColorAsync == null)
                {
                    throw new BasicBlankException("Nobody is handing the choose colors.  Rethink");
                }
                await _delegates.OpenColorAsync.Invoke();
            }
            else if (CurrentObject.WhichType == EnumCardTypeList.Draw2 || CurrentObject.WhichType == EnumCardTypeList.Skip)
            {
                _wasStart = true;
                if (CurrentObject.WhichType == EnumCardTypeList.Skip)
                {
                    await GoToNextPlayerAsync();
                    return;
                }
                _model.NextPlayer = "";
                _playerDrew = true;
                PlayerDraws = WhoTurn;
                LeftToDraw = 2;
                await DrawAsync(); //could be iffy.
                return;
            }

            //if (_wasNew == true)
            //{
            //    //_wasNew = false;
                
            //}
            int newTurn = await PlayerList!.CalculateWhoTurnAsync();
            UnoPlayerItem thisPlayer = PlayerList[newTurn];
            _model!.NextPlayer = thisPlayer.NickName; //i think this part was missing.
        }

        public override Task ContinueTurnAsync()
        {
            if (_wasStart == true)
            {
                _wasStart = false;
                return Task.CompletedTask;
            }
            return base.ContinueTurnAsync();
        }

        public override async Task EndRoundAsync()
        {
            SaveRoot!.HasDrawn = false;
            SaveRoot.HasSkipped = false;
            _model!.NextPlayer = "None";
            int thisNum = _gameContainer.PlayerWentOut();
            UpdateScores(thisNum);
            if (CanEndGame() == true)
            {
                WhoTurn = thisNum;
                SingleInfo = PlayerList!.GetWhoPlayer();
                await ShowWinAsync();
                return; //hopefully this simple.
            }
            await this.RoundOverNextAsync();
        }
        private void UpdateScores(int thisNum)
        {
            var thisList = _gameContainer.GetPlayerCards();
            int points = thisList.Sum(items => items.Points);
            PlayerList!.ForEach(items =>
            {
                if (items.Id != thisNum)
                    items.PreviousPoints = 0;
                else
                {
                    items.PreviousPoints = points;
                    items.TotalPoints += points;
                }
            });
        }
        private bool CanEndGame()
        {
            int minScore;
            if (PlayerList.Count() == 2)
                minScore = 250;
            else if (PlayerList.Count() == 3)
                minScore = 400;
            else
                minScore = 500;
            return PlayerList.Any(x => x.TotalPoints >= minScore);
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(x =>
            {
                x.TotalPoints = 0;
                x.PreviousPoints = 0; //i think this simple.
            });
            return Task.CompletedTask;
        }
    }
}
