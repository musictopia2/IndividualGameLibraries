using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using CribbageCP.Data;
using CribbageCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace CribbageXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<CribbageShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<CribbageShellViewModel, CribbageCard>();
            OurContainer!.RegisterType<BasicGameLoader<CribbagePlayerItem, CribbageSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CribbagePlayerItem, CribbageSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CribbagePlayerItem>>(true); //had to be set to true after all.
            return Task.CompletedTask;
        }
    }
}
