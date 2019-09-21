using BasicGameFramework.Attributes;
using BasicGameFramework.ColorCards;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using mm = BasicGameFramework.Extensions.CommonMessageStrings;
using nn = BasicGameFramework.MultiplayerClasses.InterfaceMessages;
namespace UnoCP
{
    [SingletonGame]
    public class UnoMainGameClass : CardGameClass<UnoCardInformation, UnoPlayerItem, UnoSaveInfo>, IFinishStart, nn.IChoosePieceNM, nn.IEndTurnNM, nn.IMiscDataNM, IStartNewGame
    {
        private readonly UnoComputerAI ai = new UnoComputerAI();
        private bool _alreadyUno;
        private bool _playerDrew;
        private bool _finishPlay;
        internal UnoCardInformation CurrentObject => _thisMod!.Pile1!.CurrentCard;
        public UnoMainGameClass(IGamePackageResolver container) : base(container) { }

        private UnoViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<UnoViewModel>();
            ai.ThisGame = this;
        }

        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            await base.FinishGetSavedAsync();
            int newTurn = await PlayerList!.CalculateWhoTurnAsync();
            UnoPlayerItem ThisPlayer = PlayerList[newTurn];
            _thisMod!.NextPlayer = ThisPlayer.NickName;
        }
        private bool _wasStart;
        private bool _wasNew;

        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            SaveRoot!.HasDrawn = false;
            SaveRoot.HasSkipped = false;
            _wasNew = true;
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }

        private void LoadControls()
        {
            if (IsLoaded == false)
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
                await ColorChosenAsync(ai.ColorChosen());
                return;
            }
            if (SaveRoot.GameStatus == EnumGameStatus.WaitingForUno)
            {
                await Delay!.DelaySeconds(.5);
                await ProcessUnoAsync(true); //computer will always say uno
                return;
            }
            await Delay!.DelaySeconds(1);
            UnoCardInformation thisCard;
            int deck;
            if (AlreadyDrew == true)
            {
                thisCard = SingleInfo.MainHandList.GetLastObjectDrawn();
                deck = thisCard.Deck;
                if (CanPlay(deck) == false)
                {
                    if (ThisData!.MultiPlayer == true)
                        await ThisNet!.SendEndTurnAsync();
                    await EndTurnAsync();
                    return;
                }
                await ProcessPlayAsync(deck);
                return;
            }
            deck = ai.ComputerMove();
            if (deck > 0)
            {
                await ProcessPlayAsync(deck);
                return;
            }
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("drawcard");
            await DrawAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning) //hopefully still okay (?)
        {
            LoadControls();
            return base.StartSetUpAsync(isBeginning);
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            _alreadyUno = false;
            _playerDrew = false;
            _finishPlay = false;
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
                _thisMod!.NextPlayer = "";
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
            int NewTurn = await PlayerList!.CalculateWhoTurnAsync();
            UnoPlayerItem newPlayer = PlayerList[NewTurn];
            _thisMod!.NextPlayer = newPlayer.NickName;
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync(); //hopefully this still works (?)
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
            if (PlayerWentOut() == -1)
                return false;
            return true;
        }
        public bool CanPlay(int deck)
        {
            if (SaveRoot!.CurrentColor == EnumColorTypes.ZOther)
                throw new BasicBlankException("Color cannot be other");
            if (SaveRoot.CurrentColor == EnumColorTypes.None)
                throw new BasicBlankException("Color cannot be none");
            if (ThisTest!.AllowAnyMove == true)
                return true; //because its testing.
            UnoCardInformation thisCard = DeckList!.GetSpecificItem(deck);
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
            if (SingleInfo.MainHandList.Count == 1 && _alreadyUno == false)
            {
                SaveRoot!.GameStatus = EnumGameStatus.WaitingForUno;
                await ContinueTurnAsync();
                return;
            }
            UnoCardInformation thisCard = CurrentObject;
            if (thisCard.Color == EnumColorTypes.ZOther)
            {
                SaveRoot!.GameStatus = EnumGameStatus.ChooseColors;
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
            UnoCardInformation ThisCard = DeckList!.GetSpecificItem(deck); //i think
            PlaceDiscard(ThisCard);
            if (SingleInfo.CanSendMessage(ThisData!) == true)
            {
                await ThisNet!.SendAllAsync("play", deck); //try this way
            }
            _finishPlay = true;
            await ProcessPlayAsync(ThisCard, deck);
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
            if (_wasNew == true)
            {
                _wasNew = false;
                if (CurrentObject.WhichType == EnumCardTypeList.Reverse && PlayerList.Count() > 2)
                {
                    SaveRoot.PlayOrder.IsReversed = true;
                    WhoTurn = await PlayerList!.CalculateWhoTurnAsync(); //i think
                }
                else if (CurrentObject.WhichType == EnumCardTypeList.Wild && CurrentObject.Color == EnumColorTypes.ZOther)
                    SaveRoot.GameStatus = EnumGameStatus.ChooseColors; //i think
                else if (CurrentObject.WhichType == EnumCardTypeList.Draw2 || CurrentObject.WhichType == EnumCardTypeList.Skip)
                {
                    _wasStart = true;
                    if (CurrentObject.WhichType == EnumCardTypeList.Skip)
                    {
                        await GoToNextPlayerAsync();
                        return;
                    }
                    _thisMod!.MainOptionsVisible = true; //has to be done early.
                    _thisMod.NextPlayer = "";
                    _playerDrew = true;
                    PlayerDraws = WhoTurn;
                    LeftToDraw = 2;
                    await DrawAsync();
                    return;
                }
            }
            int newTurn = await PlayerList!.CalculateWhoTurnAsync();
            UnoPlayerItem thisPlayer = PlayerList[newTurn];
            _thisMod!.NextPlayer = thisPlayer.NickName; //i think this part was missing.
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
            _thisMod!.NextPlayer = "None";
            int ThisNum = PlayerWentOut();
            UpdateScores(ThisNum);
            if (CanEndGame() == true)
            {
                WhoTurn = ThisNum;
                SingleInfo = PlayerList!.GetWhoPlayer();
                await ShowWinAsync();
                return; //hopefully this simple.
            }
            this.RoundOverNext();
        }
        private void UpdateScores(int thisNum)
        {
            var thisList = GetPlayerCards();
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
        public async Task ColorChosenAsync(EnumColorTypes whichColor)
        {
            SaveRoot!.GameStatus = EnumGameStatus.NormalPlay;
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync(mm.ChosenPiece, whichColor);
            if (whichColor == EnumColorTypes.ZOther)
                throw new BasicBlankException("Cannot choose wild as a color");
            SaveRoot.CurrentColor = whichColor;
            UnoCardInformation thisCard = CurrentObject;
            thisCard.Color = whichColor;
            await EndTurnAsync();
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
            return PlayerList.Any(Items => Items.TotalPoints >= minScore);
        }

        public async Task ProcessUnoAsync(bool saidUno)
        {
            _alreadyUno = true;
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("uno", saidUno);
            if (saidUno == false)
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    await _thisMod!.ShowGameMessageAsync("You had one card left.  However, you did not say uno.  Therefore, you have to draw 2 cards");
                LeftToDraw = 2;
                await DrawAsync();
                return;
            }
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                await _thisMod!.ShowGameMessageAsync($"Uno From {SingleInfo.NickName}");
            await DoFinishAsync();
        }

        async Task nn.IChoosePieceNM.ChoosePieceReceivedAsync(string data)
        {
            EnumColorTypes ThisColor = await js.DeserializeObjectAsync<EnumColorTypes>(data);
            await ColorChosenAsync(ThisColor);
        }

        async Task nn.IEndTurnNM.EndTurnReceivedAsync(string data)
        {
            await EndTurnAsync();
        }

        async Task nn.IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "play":
                    await ProcessPlayAsync(int.Parse(content));
                    break;
                case "uno":
                    await ProcessUnoAsync(bool.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing For Status {status} with the message of {content}");
            }
        }

        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(Items =>
            {
                Items.TotalPoints = 0;
                Items.PreviousPoints = 0; //i think this simple.
            });
            return Task.CompletedTask;
        }
    }

    //most did not even use this one. FinishStartAsync
}