using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using RummyDiceCP.Data;
using RummyDiceCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace RummyDiceXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<RummyDiceShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
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
