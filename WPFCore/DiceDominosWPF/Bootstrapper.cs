using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using BasicGamingUIWPFLibrary.GameGraphics.Dice;
using DiceDominosCP.Data;
using DiceDominosCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;
namespace DiceDominosWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<DiceDominosShellViewModel, GamePage>
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
            OurContainer!.RegisterNonSavedClasses<DiceDominosShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<DiceDominosPlayerItem, DiceDominosSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<DiceDominosPlayerItem, DiceDominosSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<DiceDominosPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, DiceDominosPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();

            OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>();
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}