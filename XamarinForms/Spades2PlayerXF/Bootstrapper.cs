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
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using Spades2PlayerCP.Cards;
using Spades2PlayerCP.Data;
using Spades2PlayerCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace Spades2PlayerXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<Spades2PlayerShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<Spades2PlayerShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<Spades2PlayerPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<Spades2PlayerCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<Spades2PlayerCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<Spades2PlayerCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.

            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory cat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<Spades2PlayerCardInformation> sort = new SortSimpleCards<Spades2PlayerCardInformation>();
                sort.SuitForSorting = cat.SortCategory;
                OurContainer.RegisterSingleton(sort); //if we have a custom one, will already be picked up.
            }
            //change view model for area if not using 2 player.
            OurContainer.RegisterType<TwoPlayerTrickObservable<EnumSuitList, Spades2PlayerCardInformation, Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>>();
            OurContainer.RegisterType<StandardWidthHeight>();
            return Task.CompletedTask;
        }
    }
}
