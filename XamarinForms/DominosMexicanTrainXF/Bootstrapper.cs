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
using DominosMexicanTrainCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using DominosMexicanTrainCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace DominosMexicanTrainXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<DominosMexicanTrainShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<DominosMexicanTrainShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<DominosMexicanTrainPlayerItem>>(true); //had to be set to true after all.
            //anything that needs to be registered will be here.

            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("scatter"); //this is needed so the boneyard part can work.
            OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>(); //has to do this to stop overflow and duplicates bug.
            return Task.CompletedTask;
        }
    }
}
