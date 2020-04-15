using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using KismetCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
using KismetCP.Logic;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGamingUIXFLibrary.GameGraphics.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;

namespace KismetXF
{
    public class Bootstrapper : BasicYahtzeeBootstrapper<KismetDice>
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
            OurContainer!.RegisterNonSavedClasses<YahtzeeShellViewModel<KismetDice>>();
            OurContainer!.RegisterType<KismetDetailClass>();
            OurContainer.RegisterType<KismetScoreProcesses>();
            OurContainer.RegisterType<KismetMissTurn>();
            OurContainer.RegisterType<YahtzeeLayout>(); //hopefully this is all that was missing.

        }
    }
}
