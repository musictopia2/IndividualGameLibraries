using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using CrazyEightsCP.Data;
using CrazyEightsCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace CrazyEightsXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<CrazyEightsShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<CrazyEightsShellViewModel, RegularSimpleCard>();
            OurContainer!.RegisterType<BasicGameLoader<CrazyEightsPlayerItem, CrazyEightsSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CrazyEightsPlayerItem, CrazyEightsSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CrazyEightsPlayerItem>>(true);
            return Task.CompletedTask;
        }
    }
}
