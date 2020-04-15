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
using RackoCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using RackoCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using RackoCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;

namespace RackoXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<RackoShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<RackoShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<RackoPlayerItem, RackoSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<RackoPlayerItem, RackoSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<RackoPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<RackoCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<RackoCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, RackoDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            return Task.CompletedTask;
        }
    }
}
