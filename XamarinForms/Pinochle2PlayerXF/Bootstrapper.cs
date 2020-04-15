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
using Pinochle2PlayerCP.Cards;
using Pinochle2PlayerCP.Data;
using Pinochle2PlayerCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace Pinochle2PlayerXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<Pinochle2PlayerShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<Pinochle2PlayerShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<Pinochle2PlayerPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<Pinochle2PlayerCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<Pinochle2PlayerCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<Pinochle2PlayerCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);

            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory cat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<Pinochle2PlayerCardInformation> sort = new SortSimpleCards<Pinochle2PlayerCardInformation>();
                sort.SuitForSorting = cat.SortCategory;
                OurContainer.RegisterSingleton(sort); //if we have a custom one, will already be picked up.
            }
            //change view model for area if not using 2 player.
            OurContainer.RegisterType<TwoPlayerTrickObservable<EnumSuitList, Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}
