using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using ConcentrationCP.Data;
using ConcentrationCP.Logic;
using ConcentrationCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConcentrationWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ConcentrationShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            //TestData!.PlayCategory = BasicGameFrameworkLibrary.TestUtilities.EnumPlayCategory.NoShuffle;
            //TestData.AllowAnyMove = true;
            //TestData!.ImmediatelyEndGame = true;
            return base.RegisterTestsAsync();
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<ConcentrationShellViewModel, RegularSimpleCard>(customDeck: true);
            OurContainer!.RegisterType<BasicGameLoader<ConcentrationPlayerItem, ConcentrationSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ConcentrationPlayerItem, ConcentrationSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ConcentrationPlayerItem>>(true); //had to be set to true after all.
            //anything that needs to be registered will be here.
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
            return Task.CompletedTask;
        }
    }
}