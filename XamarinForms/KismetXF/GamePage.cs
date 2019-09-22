using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.YahtzeeStyleHelpers;
using KismetCP;
namespace KismetXF
{
    public class GamePage : YahtzeePage<KismetDice>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        protected override void RegisterInterfaces()
        {
            base.RegisterInterfaces();
            OurContainer!.RegisterType<KismetDetailClass>();
            OurContainer.RegisterType<KismetScoreProcesses>();
            OurContainer.RegisterType<KismetMissTurn>();
            OurContainer.RegisterType<YahtzeeLayout>(); //hopefully this is all that was missing.
        }
        protected override void RegisterNonSavedClasses()
        {
            OurContainer!.RegisterNonSavedClasses<YahtzeeViewModel<KismetDice>>();
        }
        protected override void RegisterDiceProportions(string tag)
        {
            OurContainer!.RegisterSingleton<IProportionImage, StandardProportion>(tag);
        }
    }
}