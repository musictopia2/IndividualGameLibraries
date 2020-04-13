using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIWPFLibrary.Bootstrappers;
using BasicGamingUIWPFLibrary.GameGraphics.Dice;
using KismetCP.Data;
using KismetCP.Logic;
namespace KismetWPF
{
    public class Bootstrapper : BasicYahtzeeBootstrapper<KismetDice>
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
            OurContainer!.RegisterNonSavedClasses<YahtzeeShellViewModel<KismetDice>>();
            OurContainer!.RegisterType<KismetDetailClass>();
            OurContainer.RegisterType<KismetScoreProcesses>();
            OurContainer.RegisterType<KismetMissTurn>();

        }
    }
}