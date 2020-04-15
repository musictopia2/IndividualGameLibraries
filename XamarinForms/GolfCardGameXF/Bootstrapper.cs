using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using GolfCardGameCP.Data;
using GolfCardGameCP.Logic;
using GolfCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace GolfCardGameXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<GolfCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
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
            return Task.CompletedTask;
        }
    }
}
