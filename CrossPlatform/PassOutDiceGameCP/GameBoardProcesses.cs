using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
namespace PassOutDiceGameCP
{
    [SingletonGame]
    public class GameBoardProcesses
    {
        private readonly PassOutDiceGameMainGameClass _mainGame;
        public GameBoardProcesses(PassOutDiceGameMainGameClass MainGame)
        {
            this._mainGame = MainGame;
        }
        public void ClearBoard()
        {
            foreach (var thisSpace in _mainGame.SpaceList!.Values)
            {
                if (thisSpace.IsEnabled == true)
                {
                    thisSpace.Player = 0;
                    thisSpace.Color = EnumColorChoice.None;
                }
            }
            _mainGame.SaveRoot!.PreviousSpace = 0;
        }
        public void SaveGame()
        {
            _mainGame.SaveRoot!.SpacePlayers = _mainGame.SpaceList!.Values.Select(items => items.Player).ToCustomBasicList();
        }
        public void LoadSavedGame()
        {
            int x = 0;
            _mainGame.SaveRoot!.SpacePlayers.ForEach(thisPlayer =>
            {
                x++;
                var thisSpace = _mainGame.SpaceList![x];
                thisSpace.Player = thisPlayer;
                if (thisSpace.Player == 0)
                    thisSpace.Color = EnumColorChoice.None;
                else
                {
                    var TempPlayer = _mainGame.PlayerList![thisSpace.Player]; //its one based now.
                    thisSpace.Color = TempPlayer.Color;
                }
            });
            _mainGame.RepaintBoard();
        }
        public int GetDiceTotal => _mainGame.ThisCup!.TotalDiceValue;
        public bool IsValidMove(int index)
        {
            if (_mainGame.ThisTest!.AllowAnyMove == true)
                return true;
            var thisSpace = _mainGame.SpaceList![index];
            int diceValue = GetDiceTotal;
            return thisSpace.FirstValue + thisSpace.SecondValue == diceValue;
        }
        public void MakeMove(int index)
        {
            var thisSpace = _mainGame.SpaceList![index];
            thisSpace.Color = _mainGame.SingleInfo!.Color;
            thisSpace.Player = _mainGame.WhoTurn;
            _mainGame.SaveRoot!.PreviousSpace = index; //looks like i forgot this.
            _mainGame.RepaintBoard();
        }
        public int WhoWon
        {
            get
            {
                if (_mainGame.SpaceList!.Values.Count(items => items.Player == 1) >= 11)
                    return 1;
                if (_mainGame.SpaceList.Values.Count(items => items.Player == 2) >= 11)
                    return 2;
                return 0;
            }
        }
    }
}