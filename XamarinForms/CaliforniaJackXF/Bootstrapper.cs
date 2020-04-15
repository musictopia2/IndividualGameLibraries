using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using CaliforniaJackCP.Cards;
using CaliforniaJackCP.Data;
using CaliforniaJackCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace CaliforniaJackXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<CaliforniaJackShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<CaliforniaJackShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CaliforniaJackPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<CaliforniaJackCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<CaliforniaJackCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<CaliforniaJackCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.

            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory cat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<CaliforniaJackCardInformation> sort = new SortSimpleCards<CaliforniaJackCardInformation>();
                sort.SuitForSorting = cat.SortCategory;
                OurContainer.RegisterSingleton(sort); //if we have a custom one, will already be picked up.
            }
            //change view model for area if not using 2 player.
            OurContainer.RegisterType<TwoPlayerTrickObservable<EnumSuitList, CaliforniaJackCardInformation, CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}
