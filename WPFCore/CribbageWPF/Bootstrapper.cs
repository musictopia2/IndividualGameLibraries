using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using CribbageCP.Data;
using CribbageCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CribbageWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<CribbageShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.EndRoundEarly = true;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<CribbageShellViewModel, CribbageCard>();
            OurContainer!.RegisterType<BasicGameLoader<CribbagePlayerItem, CribbageSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CribbagePlayerItem, CribbageSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CribbagePlayerItem>>(true); //had to be set to true after all.
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}