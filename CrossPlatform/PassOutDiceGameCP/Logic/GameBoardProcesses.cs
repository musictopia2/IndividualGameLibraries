using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using PassOutDiceGameCP.Data;
using System.Linq;
namespace PassOutDiceGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardProcesses
    {
        private readonly PassOutDiceGameGameContainer _gameContainer;
        private readonly PassOutDiceGameVMData _model;

        public GameBoardProcesses(PassOutDiceGameGameContainer gameContainer, PassOutDiceGameVMData model, GameBoardGraphicsCP gameBoardGraphics)
        {
            _gameContainer = gameContainer;
            _model = model;
            gameBoardGraphics.CreateSpaceList();
        }
        public void ClearBoard()
        {
            foreach (var thisSpace in _gameContainer.SpaceList!.Values)
            {
                if (thisSpace.IsEnabled == true)
                {
                    thisSpace.Player = 0;
                    thisSpace.Color = EnumColorChoice.None;
                }
            }
            _gameContainer.SaveRoot!.PreviousSpace = 0;
        }
        public void SaveGame()
        {
            _gameContainer.SaveRoot!.SpacePlayers = _gameContainer.SpaceList!.Values.Select(items => items.Player).ToCustomBasicList();
        }
        public void LoadSavedGame()
        {
            int x = 0;
            _gameContainer.SaveRoot!.SpacePlayers.ForEach(thisPlayer =>
            {
                x++;
                var thisSpace = _gameContainer.SpaceList![x];
                thisSpace.Player = thisPlayer;
                if (thisSpace.Player == 0)
                    thisSpace.Color = EnumColorChoice.None;
                else
                {
                    var TempPlayer = _gameContainer.PlayerList![thisSpace.Player]; //its one based now.
                    thisSpace.Color = TempPlayer.Color;
                }
            });
            _gameContainer.RepaintBoard();
        }
        public int GetDiceTotal => _model.Cup!.TotalDiceValue;
        public bool IsValidMove(int index)
        {
            if (_gameContainer.Test!.AllowAnyMove == true)
                return true;
            var thisSpace = _gameContainer.SpaceList![index];
            int diceValue = GetDiceTotal;
            return thisSpace.FirstValue + thisSpace.SecondValue == diceValue;
        }
        public void MakeMove(int index)
        {
            var thisSpace = _gameContainer.SpaceList![index];
            thisSpace.Color = _gameContainer.SingleInfo!.Color;
            thisSpace.Player = _gameContainer.WhoTurn;
            _gameContainer.SaveRoot!.PreviousSpace = index; //looks like i forgot this.
            _gameContainer.RepaintBoard();
        }
        public int WhoWon
        {
            get
            {
                if (_gameContainer.SpaceList!.Values.Count(items => items.Player == 1) >= 11)
                    return 1;
                if (_gameContainer.SpaceList.Values.Count(items => items.Player == 2) >= 11)
                    return 2;
                return 0;
            }
        }
    }
}