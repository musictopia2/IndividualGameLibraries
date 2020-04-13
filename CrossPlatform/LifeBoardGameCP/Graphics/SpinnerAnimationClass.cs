using LifeBoardGameCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.Graphics
{
    public static class SpinnerAnimationClass
    {
        private static int Phase { get; set; }
        private static int PhaseHI { get; set; }
        private static int ChangePositions { get; set; }
        private static int Position { get; set; }
        private static bool CanBetween { get; set; }
        private static int MaxHighSpeed { get; set; }
        private static LifeBoardGameGameContainer? _gameContainer;
        public static async Task AnimateSpinAsync(SpinnerPositionData thisSpin, LifeBoardGameGameContainer gameContainer)
        {
            _gameContainer = gameContainer;
            CanBetween = thisSpin.CanBetween;
            ChangePositions = thisSpin.ChangePositions;
            MaxHighSpeed = thisSpin.HighSpeedUpTo;
            Phase = 0;
            PhaseHI = 0;
            if (_gameContainer.Test.NoAnimations)
            {
                MaxHighSpeed = -1;
                ChangePositions = 30;
            }
            do
            {
                if (PhaseHI < MaxHighSpeed)
                {
                    PhaseHI += 1;
                    _gameContainer.HighSpeedPhase = PhaseHI;
                    if (_gameContainer.Test!.NoAnimations == false)
                    {
                        _gameContainer.SpinnerRepaint();
                        await Task.Delay(10);
                    }
                }
                else
                {
                    _gameContainer.HighSpeedPhase = 0;
                    Phase += 1;
                    Position = AddPosition(CalculateSkips());
                    _gameContainer.SpinnerPosition = Position;
                    if (_gameContainer.Test.NoAnimations == false)
                    {
                        _gameContainer.SpinnerRepaint();
                        await Task.Delay(10);
                    }
                }
                if (Phase >= ChangePositions)
                {
                    if (CanBetween == true)
                        return;
                    if (_gameContainer.GetNumberSpun(Position) > 0)
                        return;
                }
                if (_gameContainer.Test.NoAnimations)
                {
                    await Task.Delay(2);
                    //_gameContainer.SpinnerRepaint();
                    //return;
                }
                //if (_gameContainer.Test.NoAnimations == false)
                //    await Task.Delay(2);
            }
            while (true);// will let the other classes know its finished if necessary
        }
        private static int CalculateSkips()
        {
            int x;
            x = ChangePositions;
            if (Phase < (x * 0.25))
                return 10;
            if (Phase < (x * 0.4))
                return 8;
            if (Phase < (x * 0.65))
                return 6;
            if (Phase < (x * 0.8))
                return 3;
            if (Phase < (x * 0.9))
                return 2;
            return 1;
        }
        private static int AddPosition(int v)
        {
            int output;
            int t;
            t = Position + v;
            while (t > 360)
                t -= 360;
            while (t < 1)
                t += 360;
            output = t;
            return output;
        }
    }
}