using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using SnagCardGameCP.Cards;
using SnagCardGameCP.Data;
using SnagCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace SnagCardGameXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<SnagCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<SnagCardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<SnagCardGamePlayerItem, SnagCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<SnagCardGamePlayerItem, SnagCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<SnagCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<SnagCardGameCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<SnagCardGameCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<SnagCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.

            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory cat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<SnagCardGameCardInformation> sort = new SortSimpleCards<SnagCardGameCardInformation>();
                sort.SuitForSorting = cat.SortCategory;
                OurContainer.RegisterSingleton(sort); //if we have a custom one, will already be picked up.
            }
            //change view model for area if not using 2 player.
            //OurContainer.RegisterType<SnagTrickObservable>();
            return Task.CompletedTask;
        }
    }
}
