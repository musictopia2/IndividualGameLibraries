using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TicTacToeCP.Data;
using TicTacToeCP.ViewModels;
namespace TicTacToeWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<TicTacToeShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<TicTacToeShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<TicTacToePlayerItem, TicTacToeSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<TicTacToePlayerItem, TicTacToeSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<TicTacToePlayerItem>>(true); //did not pay off since it reopens it again for the tcp events.
            OurContainer.RegisterType<BasicGameContainer<TicTacToePlayerItem, TicTacToeSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}
