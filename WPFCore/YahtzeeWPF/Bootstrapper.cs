using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIWPFLibrary.Bootstrappers;
using BasicGamingUIWPFLibrary.GameGraphics.Dice;
using YahtzeeCP.Data;
using YahtzeeCP.Logic;

namespace YahtzeeWPF
{
    public class Bootstrapper : BasicYahtzeeBootstrapper<SimpleDice>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override void RegisterDiceProportions()
        {
            OurContainer!.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
        }

        protected override void RegisterNonSavedClasses()
        {
            OurContainer!.RegisterNonSavedClasses<YahtzeeShellViewModel<SimpleDice>>();
            OurContainer!.RegisterType<YahtzeeDetailClass>();
            OurContainer.RegisterType<YahtzeeScoreProcesses>();
        }

    }
}