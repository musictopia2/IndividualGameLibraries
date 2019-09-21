using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using AndyCristinaGamePackageXF;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.YahtzeeStyleHelpers;
using YahtzeeCP;
namespace YahtzeeXF
{
    public class GamePage : YahtzeePage<SimpleDice>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        protected override void RegisterInterfaces()
        {
            base.RegisterInterfaces();
            OurContainer!.RegisterType<YahtzeeDetailClass>();
            OurContainer.RegisterType<YahtzeeScoreProcesses>();
            OurContainer.RegisterType<YahtzeeLayout>(); //hopefully this is all that was missing.
        }
        protected override void RegisterNonSavedClasses()
        {
            OurContainer!.RegisterNonSavedClasses<YahtzeeViewModel<SimpleDice>>();
        }
        protected override void RegisterDiceProportions(string tag)
        {
            OurContainer!.RegisterSingleton<IProportionImage, StandardProportion>(tag);
        }
    }
}