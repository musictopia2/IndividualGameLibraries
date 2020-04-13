using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using OldMaidCP.Data;
using OldMaidCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace OldMaidWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<OldMaidShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<OldMaidShellViewModel, RegularSimpleCard>(customDeck: true);
            OurContainer!.RegisterType<BasicGameLoader<OldMaidPlayerItem, OldMaidSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<OldMaidPlayerItem, OldMaidSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<OldMaidPlayerItem>>(true); //had to be set to true after all.
            //anything that needs to be registered will be here.
            OurContainer.RegisterSingleton<IDeckCount, OldMaidDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
            return Task.CompletedTask;
        }
    }
}