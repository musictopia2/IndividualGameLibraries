using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using CousinRummyCP.Data;
using CousinRummyCP.Logic;
using CousinRummyCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CousinRummyWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<CousinRummyShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            TestData!.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.RestoreOnly;
            TestData.StatePosition = 1; //to backtrack.
            return Task.CompletedTask;
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<CousinRummyShellViewModel, RegularRummyCard>(registerCommonProportions: false, customDeck: true);
            OurContainer!.RegisterType<BasicGameLoader<CousinRummyPlayerItem, CousinRummySaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CousinRummyPlayerItem, CousinRummySaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CousinRummyPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}