using BasicGameFramework.Dice;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace CountdownCP
{
    public class CountdownDice : SimpleDice
    {
        public override void Populate(int chosen)
        {
            base.Populate(chosen);
            DotColor = cs.Red;
            FillColor = cs.White;
        }
    }
}