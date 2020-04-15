using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TileRummyCP.Data;
using TileRummyCP.Logic;
using TileRummyCP.ViewModels;

namespace TileRummyXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<TileRummyShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.RestoreOnly;
        //    TestData.StatePosition = 3;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<TileRummyShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<TileRummyPlayerItem, TileRummySaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<TileRummyPlayerItem, TileRummySaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<TileRummyPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<TileShuffler>();
            OurContainer.RegisterSingleton<IDeckCount, TileCountClass>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("scatter");
            OurContainer.RegisterType<BasicGameContainer<TileRummyPlayerItem, TileRummySaveInfo>>();
            return Task.CompletedTask;
        }
    }
}
