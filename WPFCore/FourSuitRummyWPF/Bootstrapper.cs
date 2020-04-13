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
using FourSuitRummyCP.Data;
using FourSuitRummyCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace FourSuitRummyWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<FourSuitRummyShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.AllowAnyMove = true;
        //    OurContainer!.RegisterType<TestConfig>();
        //    //TestData!.EndRoundEarly = true;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<FourSuitRummyShellViewModel, RegularRummyCard>(registerCommonProportions: false);
            OurContainer!.RegisterType<BasicGameLoader<FourSuitRummyPlayerItem, FourSuitRummySaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<FourSuitRummyPlayerItem, FourSuitRummySaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<FourSuitRummyPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}