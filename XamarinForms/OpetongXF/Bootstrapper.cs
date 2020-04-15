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
using OpetongCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using OpetongCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace OpetongXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<OpetongShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterType<BasicGameLoader<OpetongPlayerItem, OpetongSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<OpetongPlayerItem, OpetongSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<OpetongPlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterNonSavedClasses<OpetongMainViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
            OurContainer.RegisterType<RegularCardsBasicShuffler<RegularRummyCard>>(true);
            OurContainer.RegisterType<DeckObservablePile<RegularRummyCard>>(true); //i think
            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<RegularRummyCard> ThisSort = new SortSimpleCards<RegularRummyCard>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceLowSimpleDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
            return Task.CompletedTask;
        }
    }
}
