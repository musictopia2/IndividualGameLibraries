using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using BattleshipCP.Data;
using BattleshipCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace BattleshipXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<BattleshipShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<BattleshipShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<BattleshipPlayerItem, BattleshipSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<BattleshipPlayerItem, BattleshipSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<BattleshipPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>();
            OurContainer.RegisterType<BasicGameContainer<BattleshipPlayerItem, BattleshipSaveInfo>>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
