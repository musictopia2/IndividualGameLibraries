using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP
{
    public static class SpinnerAnimationClass
    {
        private static int iPhase;
        private static int iPhaseHI;
        private static int ChangePositions;
        private static int iPosition = 0;
        private static bool CanBetween;
        private static int MaxHighSpeed;
        private static GlobalVariableClass? _thisGlobal;
        public static async Task AnimateSpinAsync(SpinnerPositionData thisSpin, LifeBoardGameMainGameClass mainGame)
        {
            if (_thisGlobal == null)
                _thisGlobal = mainGame.MainContainer.Resolve<GlobalVariableClass>();
            CanBetween = thisSpin.CanBetween;
            ChangePositions = thisSpin.ChangePositions;
            MaxHighSpeed = thisSpin.HighSpeedUpTo;
            iPhase = 0;
            iPhaseHI = 0;
            do
            {
                if (iPhaseHI < MaxHighSpeed)
                {
                    iPhaseHI += 1;
                    _thisGlobal.SpinnerCanvas!.HighSpeedPhase = iPhaseHI;
                    if (mainGame.ThisTest!.NoAnimations == false)
                    {
                        _thisGlobal.SpinnerCanvas.Repaint();
                        await Task.Delay(10);
                    }
                }
                else
                {
                    _thisGlobal.SpinnerCanvas!.HighSpeedPhase = 0;
                    iPhase += 1;
                    iPosition = AddPosition(CalculateSkips());
                    _thisGlobal.SpinnerCanvas.Position = iPosition;
                    if (mainGame.ThisTest!.NoAnimations == false)
                    {
                        _thisGlobal.SpinnerCanvas.Repaint();
                        await Task.Delay(10);
                    }
                }
                if (iPhase >= ChangePositions)
                {
                    if (CanBetween == true)
                        return;
                    if (_thisGlobal.GetNumberSpun(iPosition) > 0)
                        return;
                }
                if (mainGame.ThisTest.NoAnimations == true)
                    await Task.Delay(2);
            }
            while (true);// will let the other classes know its finished if necessary
        }
        private static int CalculateSkips()
        {
            int x;
            x = ChangePositions;
            if (iPhase < (x * 0.25))
                return 10;
            if (iPhase < (x * 0.4))
                return 8;
            if (iPhase < (x * 0.65))
                return 6;
            if (iPhase < (x * 0.8))
                return 3;
            if (iPhase < (x * 0.9))
                return 2;
            return 1;
        }
        private static int AddPosition(int v)
        {
            int output;
            int t;
            t = iPosition + v;
            while (t > 360)
                t -= 360;
            while (t < 1)
                t += 360;
            output = t;
            return output;
        }
    }
}