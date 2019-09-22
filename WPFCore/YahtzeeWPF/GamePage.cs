using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.YahtzeeStyleHelpers;
using YahtzeeCP;
namespace YahtzeeWPF
{
    public class GamePage : YahtzeeWindow<SimpleDice>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        protected override void RegisterDiceProportions()
        {
            OurContainer!.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
        }
        protected override void RegisterInterfaces()
        {
            base.RegisterInterfaces();
            OurContainer!.RegisterType<YahtzeeDetailClass>();
            OurContainer.RegisterType<YahtzeeScoreProcesses>();
        }
        protected override void RegisterNonSavedClasses()
        {
            OurContainer!.RegisterNonSavedClasses<YahtzeeViewModel<SimpleDice>>();
        }
    }
}