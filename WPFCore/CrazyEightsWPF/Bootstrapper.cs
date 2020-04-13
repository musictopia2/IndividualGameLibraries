using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using CrazyEightsCP.Data;
using CrazyEightsCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CrazyEightsWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<CrazyEightsShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<CrazyEightsShellViewModel, RegularSimpleCard>();
            OurContainer!.RegisterType<BasicGameLoader<CrazyEightsPlayerItem, CrazyEightsSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CrazyEightsPlayerItem, CrazyEightsSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CrazyEightsPlayerItem>>(true); //had to be set to true after all.
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}