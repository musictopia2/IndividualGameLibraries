using BasicGameFrameworkLibrary.Dice;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;

namespace KismetCP.Data
{
    public class KismetDice : SimpleDice
    {
        public override void Populate(int chosen)
        {
            base.Populate(chosen);
            //do other things.
            if (chosen == 1 || chosen == 6)
                DotColor = cs.Black;
            else if (chosen == 2 || chosen == 5)
                DotColor = cs.DarkOrange;
            else
                DotColor = cs.DarkGreen;
        }
    }
}