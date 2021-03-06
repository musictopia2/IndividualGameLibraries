using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using BowlingDiceGameCP.Data;
using BowlingDiceGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BowlingDiceGameWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<BowlingDiceGameShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<BowlingDiceGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<BowlingDiceGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>();
            OurContainer.RegisterType<BasicGameContainer<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}