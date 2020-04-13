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
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using SixtySix2PlayerCP.Cards;
using SixtySix2PlayerCP.Data;
using SixtySix2PlayerCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace SixtySix2PlayerWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<SixtySix2PlayerShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<SixtySix2PlayerShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<SixtySix2PlayerPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<SixtySix2PlayerCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<SixtySix2PlayerCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<SixtySix2PlayerCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.

            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory cat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<SixtySix2PlayerCardInformation> sort = new SortSimpleCards<SixtySix2PlayerCardInformation>();
                sort.SuitForSorting = cat.SortCategory;
                OurContainer.RegisterSingleton(sort); //if we have a custom one, will already be picked up.
            }
            //change view model for area if not using 2 player.
            OurContainer.RegisterType<TwoPlayerTrickObservable<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}