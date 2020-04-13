using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using RackoCP.Logic;

namespace RackoCP.Cards
{
    public class RackoDeckCount : IDeckCount
    {
        private readonly RackoDelegates _delegates;

        public RackoDeckCount(RackoDelegates delegates)
        {
            _delegates = delegates;
        }
        public int GetDeckCount()
        {
            int count = _delegates.PlayerCount!.Invoke();
            if (count == 2)
                return 40;
            if (count == 3)
                return 50;
            if (count == 4)
                return 60;
            throw new BasicBlankException("Only 2 to 4 players are supported");
        }
    }
}
