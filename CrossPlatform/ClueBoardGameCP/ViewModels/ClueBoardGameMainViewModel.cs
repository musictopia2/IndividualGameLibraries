using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using ClueBoardGameCP.Cards;
using ClueBoardGameCP.Data;
using ClueBoardGameCP.Logic;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ClueBoardGameCP.ViewModels
{
    [InstanceGame]
    public class ClueBoardGameMainViewModel : BoardDiceGameVM
    {

        private int _leftToMove;
        [VM]
        public int LeftToMove
        {
            get { return _leftToMove; }
            set
            {
                if (SetProperty(ref _leftToMove, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _currentRoomName = "";
        [VM]
        public string CurrentRoomName
        {
            get { return _currentRoomName; }
            set
            {
                if (SetProperty(ref _currentRoomName, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _currentCharacterName = "";
        [VM]
        public string CurrentCharacterName
        {
            get { return _currentCharacterName; }
            set
            {
                if (SetProperty(ref _currentCharacterName, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _currentWeaponName = "";
        [VM]
        public string CurrentWeaponName
        {
            get { return _currentWeaponName; }
            set
            {
                if (SetProperty(ref _currentWeaponName, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        private readonly ClueBoardGameMainGameClass _mainGame; //if we don't need, delete.
        private readonly ClueBoardGameVMData _model;
        private readonly BasicData _basicData;
        private readonly ClueBoardGameGameContainer _gameContainer;
        private readonly GameBoardProcesses _gameBoard;
        
        public ClueBoardGameMainViewModel(CommandContainer commandContainer,
            ClueBoardGameMainGameClass mainGame,
            ClueBoardGameVMData model,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller,
            ClueBoardGameGameContainer gameContainer,
            GameBoardProcesses gameBoard
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
            _model = model;
            _basicData = basicData;
            _gameContainer = gameContainer;
            _gameBoard = gameBoard;
            _gameContainer.SpaceClickedAsync = MoveSpaceAsync;
            _gameContainer.RoomClickedAsync = MoveRoomAsync;
            _model.Pile.SendEnableProcesses(this, () => false);
            _model.HandList.ObjectClickedAsync += HandList_ObjectClickedAsync;
            _model.HandList.SendEnableProcesses(this, () => _mainGame.SaveRoot.GameStatus == EnumClueStatusList.FindClues);
        }
        private async Task MoveSpaceAsync(int space)
        {
            if (_gameContainer.Test.DoubleCheck)
            {
                _gameContainer.TempClicked = space;
                _gameBoard.RepaintBoard();
                return;
            }
            if (_gameBoard.CanMoveToSpace(space) == false)
            {
                if (_basicData.IsXamarinForms)
                {
                    return;
                }
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            if (_mainGame.SaveRoot.MovesLeft == 0)
            {
                if (_basicData.IsXamarinForms)
                {
                    return;
                }
                await UIPlatform.ShowMessageAsync("No Moves Left.  Try Rolling The Dice");
                return;
            }
            if (_mainGame.BasicData.MultiPlayer)
            {
                await _mainGame.Network!.SendAllAsync("space", space);
            }
            _gameBoard.MoveToSpace(space);
            await _mainGame.ContinueMoveAsync();
        }

        private async Task MoveRoomAsync(int room)
        {
            if (_gameBoard.CanMoveToRoom(room) == false)
            {
                if (_basicData.IsXamarinForms)
                {
                    return;
                }
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            if (_mainGame!.SaveRoot!.GameStatus == EnumClueStatusList.MoveSpaces && _gameContainer.CurrentCharacter!.PreviousRoom > 0)
            {
                if (_basicData.IsXamarinForms)
                {
                    return;
                }
                await UIPlatform.ShowMessageAsync("Sorry, since you rolled, you cannot go into a room");
                return;
            }
            if (_gameContainer.BasicData!.MultiPlayer)
            {
                await _gameContainer.Network!.SendAllAsync("room", room);
            }
            _gameContainer.SaveRoot.MovesLeft = 0;
            _gameBoard.MoveToRoom(room);
            _gameContainer.SaveRoot.GameStatus = EnumClueStatusList.MakePrediction; //hopefully this is it.
        }

        private async Task HandList_ObjectClickedAsync(CardInfo payLoad, int index)
        {
            if (_gameContainer.CanGiveCard(payLoad) == false)
            {
                if (_basicData.IsXamarinForms)
                {
                    return;
                }
                await UIPlatform.ShowMessageAsync("Sorry, the card cannot be given as a clue because its not part of the prediction");
                return;
            }
            payLoad.IsSelected = true;
            if (_gameContainer.Test.NoAnimations == false)
            {
                await _gameContainer.Delay.DelaySeconds(.25);
            }
            var tempPlayer = _gameContainer!.PlayerList!.GetWhoPlayer();
            if (_gameContainer.BasicData!.MultiPlayer)
                await _gameContainer.Network!.SendToParticularPlayerAsync("cluegiven", payLoad.Deck, tempPlayer.NickName);
            CommandContainer!.ManuelFinish = true;
            payLoad.IsSelected = false;
            _gameContainer.SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
            if (_gameContainer.BasicData.MultiPlayer == false)
                throw new BasicBlankException("Computer should have never had this");
            _gameContainer.Check!.IsEnabled = true; //to wait for them to end turn.
        }
        [Command(EnumCommandCategory.Game)]
        public void CurrentRoomClick(string room)
        {
            if (_basicData.IsXamarinForms)
            {
                CurrentRoomName = room;
            }
            _model.CurrentRoomName = room;
        }
        [Command(EnumCommandCategory.Game)]
        public void CurrentCharacterClick(string character)
        {
            if (_basicData.IsXamarinForms)
            {
                CurrentCharacterName = character;
            }
            _model.CurrentCharacterName = character;
        }

        [Command(EnumCommandCategory.Game)]
        public void CurrentWeaponClick(string weapon)
        {
            if (_basicData.IsXamarinForms)
            {
                CurrentWeaponName = weapon; //this too.
            }
            _model.CurrentWeaponName = weapon;
        }
        public bool CanMakePrediction
        {
            get
            {
                if (CurrentWeaponName == "" || CurrentCharacterName == "")
                {
                    return false;
                }
                if (_gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction)
                {
                    return true;
                }
                if (_gameContainer.SaveRoot.GameStatus == EnumClueStatusList.StartTurn)
                {
                    if (_gameContainer.CurrentCharacter!.PreviousRoom != _gameContainer.CurrentCharacter.CurrentRoom)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task MakePredictionAsync()
        {
            _gameContainer!.SaveRoot!.CurrentPrediction!.CharacterName = CurrentCharacterName;
            _gameContainer.SaveRoot.CurrentPrediction.WeaponName = CurrentWeaponName;
            await _mainGame.MakePredictionAsync();
        }

        public bool CanMakeAccusation
        {
            get
            {
                if (_gameContainer.SaveRoot.GameStatus == EnumClueStatusList.FindClues)
                {
                    return false;
                }
                if (CurrentWeaponName == "" || CurrentCharacterName == "" || CurrentRoomName == "")
                {
                    return false;
                }
                return true;
            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task MakeAccusationAsync()
        {
            _gameContainer!.SaveRoot!.CurrentPrediction!.CharacterName = CurrentCharacterName;
            _gameContainer.SaveRoot.CurrentPrediction.WeaponName = CurrentWeaponName;
            _gameContainer.SaveRoot.CurrentPrediction.RoomName = CurrentRoomName;
            await _mainGame.MakeAccusationAsync();
        }

        protected override Task TryCloseAsync()
        {
            _model.HandList.ObjectClickedAsync -= HandList_ObjectClickedAsync;
            return base.TryCloseAsync();
        }
        //anything else needed is here.
        public override bool CanRollDice()
        {
            return _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.StartTurn;
        }
        public override bool CanEndTurn()
        {
            return _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction || _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.EndTurn;
        }
        public override async Task RollDiceAsync() //if any changes, do here.
        {
            await base.RollDiceAsync();
        }
        [Command(EnumCommandCategory.Limited)]
        public void FillInClue(DetectiveInfo detective)
        {
            detective.IsChecked = !detective.IsChecked;
        }

    }
}