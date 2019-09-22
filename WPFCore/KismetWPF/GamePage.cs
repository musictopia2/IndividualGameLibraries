using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.YahtzeeStyleHelpers;
using KismetCP;
namespace KismetWPF
{
    public class GamePage : YahtzeeWindow<KismetDice>
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
            OurContainer!.RegisterType<KismetDetailClass>();
            OurContainer.RegisterType<KismetScoreProcesses>();
            OurContainer.RegisterType<KismetMissTurn>();
        }

        protected override void RegisterNonSavedClasses()
        {
            OurContainer!.RegisterNonSavedClasses<YahtzeeViewModel<KismetDice>>();
        }
    }
}