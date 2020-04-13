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
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.Bootstrappers;
using PickelCardGameCP.Cards;
using PickelCardGameCP.Data;
using PickelCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace PickelCardGameWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<PickelCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<PickelCardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<PickelCardGamePlayerItem, PickelCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<PickelCardGamePlayerItem, PickelCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<PickelCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<PickelCardGameCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<PickelCardGameCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<PickelCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //i think this is best this time.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<SeveralPlayersTrickObservable<EnumSuitList, PickelCardGameCardInformation, PickelCardGamePlayerItem, PickelCardGameSaveInfo>>();
            OurContainer.RegisterType<StandardWidthHeight>();
            return Task.CompletedTask;
        }
    }
}