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
using HorseshoeCardGameCP.Cards;
using HorseshoeCardGameCP.Data;
using HorseshoeCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace HorseshoeCardGameXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<HorseshoeCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<HorseshoeCardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<HorseshoeCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<HorseshoeCardGameCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<HorseshoeCardGameCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<HorseshoeCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.

            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory cat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<HorseshoeCardGameCardInformation> sort = new SortSimpleCards<HorseshoeCardGameCardInformation>();
                sort.SuitForSorting = cat.SortCategory;
                OurContainer.RegisterSingleton(sort); //if we have a custom one, will already be picked up.
            }
            //OurContainer.RegisterType<HorseshoeTrickAreaCP>();
            return Task.CompletedTask;
        }
    }
}
