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
using SkuckCardGameCP.Cards;
using SkuckCardGameCP.Data;
using SkuckCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace SkuckCardGameWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<SkuckCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<SkuckCardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<SkuckCardGamePlayerItem, SkuckCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<SkuckCardGamePlayerItem, SkuckCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<SkuckCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<SkuckCardGameCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<SkuckCardGameCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<SkuckCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.
            OurContainer.RegisterType<StandardWidthHeight>();
            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory cat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<SkuckCardGameCardInformation> sort = new SortSimpleCards<SkuckCardGameCardInformation>();
                sort.SuitForSorting = cat.SortCategory;
                OurContainer.RegisterSingleton(sort); //if we have a custom one, will already be picked up.
            }
            //change view model for area if not using 2 player.
            OurContainer.RegisterType<TwoPlayerTrickObservable<EnumSuitList, SkuckCardGameCardInformation, SkuckCardGamePlayerItem, SkuckCardGameSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}