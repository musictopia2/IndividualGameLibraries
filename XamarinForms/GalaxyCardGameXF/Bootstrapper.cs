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
using GalaxyCardGameCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using GalaxyCardGameCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using GalaxyCardGameCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace GalaxyCardGameXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<GalaxyCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<GalaxyCardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<GalaxyCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<GalaxyCardGameCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<GalaxyCardGameCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<GalaxyCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, CustomSize>(ts.TagUsed);

            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory cat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<GalaxyCardGameCardInformation> sort = new SortSimpleCards<GalaxyCardGameCardInformation>();
                sort.SuitForSorting = cat.SortCategory;
                OurContainer.RegisterSingleton(sort); //if we have a custom one, will already be picked up.
            }
            //change view model for area if not using 2 player.
            OurContainer.RegisterType<TwoPlayerTrickObservable<EnumSuitList, GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}
