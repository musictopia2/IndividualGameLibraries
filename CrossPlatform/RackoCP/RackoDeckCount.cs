using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace RackoCP
{
    public class RackoDeckCount : IDeckCount
    {
        public RackoDeckCount(RackoMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        private readonly RackoMainGameClass _mainGame;
        public int GetDeckCount() //taking a risk.  hopefully it pays off.
        {
            if (_mainGame.PlayerList.Count() == 2)
                return 40;
            if (_mainGame.PlayerList.Count() == 3)
                return 50;
            if (_mainGame.PlayerList.Count() == 4)
                return 60;
            throw new BasicBlankException("Only 2 to 4 players are supported");

        }
    }
}