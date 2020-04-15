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
using HitTheDeckCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using HitTheDeckCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using HitTheDeckCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;

namespace HitTheDeckXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<HitTheDeckShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<HitTheDeckShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<HitTheDeckPlayerItem, HitTheDeckSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<HitTheDeckPlayerItem, HitTheDeckSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<HitTheDeckPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<HitTheDeckCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<HitTheDeckCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, HitTheDeckDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
