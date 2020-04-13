using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using RummyDiceCP.Data;
using RummyDiceCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace RummyDiceWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<RummyDiceShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<RummyDiceShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<RummyDicePlayerItem, RummyDiceSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<RummyDicePlayerItem, RummyDiceSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<RummyDicePlayerItem>>(true);
            OurContainer.RegisterSingleton<IGenerateDice<int>, RummyDiceInfo>();
            OurContainer.RegisterType<BasicGameContainer<RummyDicePlayerItem, RummyDiceSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}