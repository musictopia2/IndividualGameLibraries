using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using GolfCardGameCP.Data;
using GolfCardGameCP.Logic;
using GolfCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GolfCardGameWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<GolfCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<GolfCardGameShellViewModel, RegularSimpleCard>(customDeck: true);
            OurContainer!.RegisterType<BasicGameLoader<GolfCardGamePlayerItem, GolfCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<GolfCardGamePlayerItem, GolfCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<GolfCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
            OurContainer.RegisterSingleton<IDeckCount, GolfDeck>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}