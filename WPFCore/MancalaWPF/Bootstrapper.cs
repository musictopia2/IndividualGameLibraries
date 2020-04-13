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
using MancalaCP.Data;
using MancalaCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MancalaWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<MancalaShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<MancalaShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<MancalaPlayerItem, MancalaSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<MancalaPlayerItem, MancalaSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<MancalaPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IProportionBoard, StandardProportion>(""); //here too.
            OurContainer.RegisterType<BasicGameContainer<MancalaPlayerItem, MancalaSaveInfo>>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}