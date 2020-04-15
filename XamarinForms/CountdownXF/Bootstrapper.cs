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
using CountdownCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using CountdownCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGamingUIXFLibrary.GameGraphics.Dice;

namespace CountdownXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<CountdownShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<CountdownShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<CountdownPlayerItem, CountdownSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CountdownPlayerItem, CountdownSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CountdownPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<CountdownDice, CountdownPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, CountdownDice>();
            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("");
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
