using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using FluxxCP.Cards;
using FluxxCP.Data;
using FluxxCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FluxxWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<FluxxShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            //TestData!.StatePosition = 3;
            //OurContainer!.RegisterType<TestConfig>();
            //TestData!.ComputerEndsTurn = true; //the computer has to end turn to get better testing.
            //TestData.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.RestoreOnly; //restore so i can see about use what you take.
            //we may need to register lots of tests to start with.
            return base.RegisterTestsAsync();
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<FluxxShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<FluxxPlayerItem, FluxxSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<FluxxPlayerItem, FluxxSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<FluxxPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<FluxxCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<FluxxCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FluxxDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            //anything that needs to be registered will be here.



            return Task.CompletedTask;
        }
    }
}