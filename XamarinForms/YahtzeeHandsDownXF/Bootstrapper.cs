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
using YahtzeeHandsDownCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using YahtzeeHandsDownCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using YahtzeeHandsDownCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;

namespace YahtzeeHandsDownXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<YahtzeeHandsDownShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<YahtzeeHandsDownShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<YahtzeeHandsDownPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<YahtzeeHandsDownCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<YahtzeeHandsDownCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, YahtzeeHandsDownDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            OurContainer.RegisterSingleton<IProportionImage, ComboProportion>("combo");
            return Task.CompletedTask;
        }
    }
}
