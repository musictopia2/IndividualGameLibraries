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
using RollEmCP.Data;
using RollEmCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace RollEmWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<RollEmShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.ImmediatelyEndGame = true;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<RollEmShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<RollEmPlayerItem, RollEmSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<RollEmPlayerItem, RollEmSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<RollEmPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, RollEmPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton<IProportionBoard, StandardProportion>(""); //here too.
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}