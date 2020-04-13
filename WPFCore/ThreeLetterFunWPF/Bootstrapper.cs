using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
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
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.GraphicsCP;
using ThreeLetterFunCP.ViewModels;
namespace ThreeLetterFunWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ThreeLetterFunShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    //ThreeLetterFunShellViewModel.TestMode = EnumTestCategory.CardsPlayer;
        //    //TestData!.AlwaysNewGame = true; //needs to allow new game to start with as well.
        //    TestData!.ImmediatelyEndGame = true; //try this as well.
        //    TestData.ShowErrorMessageBoxes = false;
        //    //TestData!.ShowNickNameOnShell = true;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<ThreeLetterFunShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ThreeLetterFunPlayerItem>>(true); //had to be set to true after all.

            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(TileCP.TagUsed);
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ThreeLetterFunCardGraphicsCP.TagUsed);
            OurContainer.RegisterType<GenericCardShuffler<ThreeLetterFunCardData>>();
            OurContainer.RegisterSingleton<IDeckCount, ThreeLetterFunDeckInfo>();
            OurContainer.RegisterType<BasicGameContainer<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}