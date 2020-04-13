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
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using OpetongCP.Data;
using OpetongCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace OpetongWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<OpetongShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            //TestData!.ImmediatelyEndGame = true;
            return base.RegisterTestsAsync();
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

            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }

    internal class DeckViewModel<T>
    {
    }
}