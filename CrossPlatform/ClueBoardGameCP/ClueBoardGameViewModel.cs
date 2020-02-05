using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ClueBoardGameCP
{
    public class ClueBoardGameViewModel : BoardDiceGameVM<EnumColorChoice, PawnPiecesCP<EnumColorChoice>,
        ClueBoardGamePlayerItem, ClueBoardGameMainGameClass, int>
    {
        private bool _DetectiveVisible;
        public bool DetectiveVisible
        {
            get { return _DetectiveVisible; }
            set
            {
                if (SetProperty(ref _DetectiveVisible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumClueStatusList _GameStatus;
        public EnumClueStatusList GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _CurrentRoomName = "";
        public string CurrentRoomName
        {
            get { return _CurrentRoomName; }
            set
            {
                if (SetProperty(ref _CurrentRoomName, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _CurrentCharacterName = "";
        public string CurrentCharacterName
        {
            get { return _CurrentCharacterName; }
            set
            {
                if (SetProperty(ref _CurrentCharacterName, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _CurrentWeaponName = "";
        public string CurrentWeaponName
        {
            get { return _CurrentWeaponName; }
            set
            {
                if (SetProperty(ref _CurrentWeaponName, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _LeftToMove;
        public int LeftToMove
        {
            get { return _LeftToMove; }
            set
            {
                if (SetProperty(ref _LeftToMove, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool CanEnablePart => MainGame!.DidChooseColors;
        public HandViewModel<CardInfo>? HandList; //this time, needs the hand
        public PileViewModel<CardInfo>? ThisPile;
        public ClueBoardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDice()
        {
            return false;
        }
        protected override bool CanRollDice()
        {
            if (CanEnablePart == false)
                return false;
            return GameStatus == EnumClueStatusList.StartTurn;
        }
        public override bool CanEndTurn()
        {
            if (CanEnablePart == false)
                return false;
            return GameStatus == EnumClueStatusList.MakePrediction || GameStatus == EnumClueStatusList.EndTurn;
        }
        private bool CanEnableSpace
        {
            get
            {
                if (MainGame!.SaveRoot!.GameStatus == EnumClueStatusList.FindClues)
                    return false;
                if (MainGame.SaveRoot.GameStatus == EnumClueStatusList.StartTurn)
                {
                    return MainGame.ThisGlobal!.CurrentCharacter!.CurrentRoom > 0;
                }
                return MainGame.SaveRoot.GameStatus == EnumClueStatusList.MoveSpaces;
            }
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 1; //most board games are only one dice.  can increase if necessary
            ThisCup.Visible = true;
        }
        public GameBoardProcesses? GameBoard1;
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public BasicGameCommand<int>? RoomCommand { get; set; }
        public BasicGameCommand? ToggleDetectiveCommand { get; set; }
        public BasicGameCommand<string>? CurrentRoomCommand { get; set; }
        public BasicGameCommand<string>? CurrentCharacterCommand { get; set; }
        public BasicGameCommand<string>? CurrentWeaponCommand { get; set; }
        public BasicGameCommand<DetectiveInfo>? FillClueCommand { get; set; }
        public BasicGameCommand? MakePredictionCommand { get; set; }
        public BasicGameCommand? MakeAccusationCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<int>(this, async space =>
            {
                if (ThisTest!.DoubleCheck)
                {
                    MainGame!.TempClicked = space;
                    GameBoard1!.RepaintBoard(); //i think.
                    return;
                }
                if (GameBoard1!.CanMoveToSpace(space) == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    return;
                }
                if (MainGame!.SaveRoot!.MovesLeft == 0)
                {
                    await ShowGameMessageAsync("No Moves Left.  Try Rolling The Dice");
                    return;
                }
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("space", space);
                GameBoard1.MoveToSpace(space);
                await MainGame.ContinueMoveAsync();
            }, item =>
            {
                if (ThisTest!.DoubleCheck)
                    return true;
                if (CanEnablePart == false)
                    return false;
                return CanEnableSpace;
            }, this, CommandContainer!);
            RoomCommand = new BasicGameCommand<int>(this, async room =>
            {
                if (GameBoard1!.CanMoveToRoom(room) == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    return;
                }
                if (MainGame!.SaveRoot!.GameStatus == EnumClueStatusList.MoveSpaces && MainGame.ThisGlobal!.CurrentCharacter!.PreviousRoom > 0)
                {
                    await ShowGameMessageAsync("Sorry, since you rolled, you cannot go into a room");
                    return;
                }
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("room", room);
                MainGame.SaveRoot.MovesLeft = 0;
                GameBoard1.MoveToRoom(room);
                MainGame.SaveRoot.GameStatus = EnumClueStatusList.MakePrediction; //hopefully this is it.
            }, items =>
            {
                if (CanEnablePart == false)
                    return false;
                return CanEnableSpace;
            }, this, CommandContainer!);
            FillClueCommand = new BasicGameCommand<DetectiveInfo>(this, thisD =>
            {
                thisD.IsChecked = !thisD.IsChecked;
            }, items => true, this, CommandContainer!);
            ToggleDetectiveCommand = new BasicGameCommand(this, items =>
            {
                DetectiveVisible = !DetectiveVisible;
                OnPropertyChanged(nameof(GameStatus));
            }, items => CanEnablePart, this, CommandContainer!);

            CurrentRoomCommand = new BasicGameCommand<string>(this, chosenRoom =>
            {
                CurrentRoomName = chosenRoom; //hopefully does not need notify because of the new format.
            }, items => CanEnablePart, this, CommandContainer!);
            CurrentCharacterCommand = new BasicGameCommand<string>(this, chosenCharacter =>
            {
                CurrentCharacterName = chosenCharacter;
            }, items => CanEnablePart, this, CommandContainer!);
            CurrentWeaponCommand = new BasicGameCommand<string>(this, chosenWeapon =>
            {
                CurrentWeaponName = chosenWeapon;
            }, items => CanEnablePart, this, CommandContainer!);
            MakePredictionCommand = new BasicGameCommand(this, async items =>
            {
                MainGame!.SaveRoot!.CurrentPrediction!.CharacterName = CurrentCharacterName;
                MainGame.SaveRoot.CurrentPrediction.WeaponName = CurrentWeaponName;
                await MainGame.MakePredictionAsync();
            }, items =>
            {
                if (CanEnablePart == false)
                    return false;
                if (CurrentWeaponName == "" || CurrentCharacterName == "")
                    return false;
                if (GameStatus == EnumClueStatusList.MakePrediction)
                    return true;
                if (GameStatus == EnumClueStatusList.StartTurn)
                {
                    if (MainGame!.ThisGlobal!.CurrentCharacter!.PreviousRoom != MainGame.ThisGlobal.CurrentCharacter.CurrentRoom)
                        return true;
                }
                return false;
            }, this, CommandContainer!);
            MakeAccusationCommand = new BasicGameCommand(this, async items =>
            {
                MainGame!.SaveRoot!.CurrentPrediction!.CharacterName = CurrentCharacterName;
                MainGame.SaveRoot.CurrentPrediction.WeaponName = CurrentWeaponName;
                MainGame.SaveRoot.CurrentPrediction.RoomName = CurrentRoomName;
                await MainGame.MakeAccusationAsync();
            }, items =>
            {
                if (CanEnablePart == false)
                    return false;
                if (GameStatus == EnumClueStatusList.FindClues)
                    return false;
                if (CurrentWeaponName == "" || CurrentCharacterName == "" || CurrentRoomName == "")
                    return false;
                return true;
            }, this, CommandContainer!);
            FillClueCommand.BusyCategory = EnumCommandBusyCategory.Limited;
            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>();
            HandList = new HandViewModel<CardInfo>(this);
            HandList.AutoSelect = HandViewModel<CardInfo>.EnumAutoType.None;
            HandList.Maximum = 3;
            HandList.Text = "Your Cards";
            HandList.SendEnableProcesses(this, () => GameStatus == EnumClueStatusList.FindClues);
            ThisPile = new PileViewModel<CardInfo>(ThisE!, this); ;
            ThisPile.CurrentOnly = true;
            ThisPile.SendEnableProcesses(this, () => false);
            HandList.ObjectClickedAsync += HandList_ObjectClickedAsync;
        }
        private async Task HandList_ObjectClickedAsync(CardInfo payLoad, int index)
        {
            if (MainGame!.ThisGlobal!.CanGiveCard(payLoad) == false)
            {
                await ShowGameMessageAsync("Sorry, the card cannot be given as a clue because its not part of the prediction");
                return;
            }
            payLoad.IsSelected = true;
            if (ThisTest!.NoAnimations == false)
                await MainGame!.Delay!.DelaySeconds(.25);
            var tempPlayer = MainGame!.PlayerList!.GetWhoPlayer();
            if (ThisData!.MultiPlayer)
                await ThisNet!.SendToParticularPlayerAsync("cluegiven", payLoad.Deck, tempPlayer.NickName);
            CommandContainer!.ManuelFinish = true;
            payLoad.IsSelected = false;
            MainGame.SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
            if (ThisData.MultiPlayer == false)
                throw new BasicBlankException("Computer should have never had this");
            MainGame.ThisCheck!.IsEnabled = true; //to wait for them to end turn.
        }
    }
}