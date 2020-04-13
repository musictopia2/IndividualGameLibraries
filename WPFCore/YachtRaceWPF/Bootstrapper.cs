using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using BasicGamingUIWPFLibrary.GameGraphics.Dice;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using YachtRaceCP.Data;
using YachtRaceCP.ViewModels;
//i think this is the most common things i like to do
namespace YachtRaceWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<YachtRaceShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.AllowAnyMove = true;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<YachtRaceShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<YachtRacePlayerItem, YachtRaceSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<YachtRacePlayerItem, YachtRaceSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<YachtRacePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, YachtRacePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            return Task.CompletedTask;
        }
    }
}