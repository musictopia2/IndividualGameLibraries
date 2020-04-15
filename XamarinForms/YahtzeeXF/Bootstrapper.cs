using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGamingUIXFLibrary.GameGraphics.Dice;
using YahtzeeCP.Data;
using YahtzeeCP.Logic;

namespace YahtzeeXF
{
    public class Bootstrapper : BasicYahtzeeBootstrapper<SimpleDice>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        
        protected override void RegisterDiceProportions()
        {
            OurContainer!.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
        }

        protected override void RegisterNonSavedClasses()
        {
            OurContainer!.RegisterNonSavedClasses<YahtzeeShellViewModel<SimpleDice>>();
            OurContainer!.RegisterType<YahtzeeDetailClass>();
            OurContainer.RegisterType<YahtzeeScoreProcesses>();
            OurContainer.RegisterType<YahtzeeLayout>();
        }

    }
}
