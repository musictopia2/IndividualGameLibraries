using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using LottoDominosCP.Data;
using LottoDominosCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace LottoDominosXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<LottoDominosShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<LottoDominosShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<LottoDominosPlayerItem, LottoDominosSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<LottoDominosPlayerItem, LottoDominosSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<LottoDominosPlayerItem>>(true); //had to be set to true after all.

            OurContainer.RegisterType<StandardWidthHeight>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>();
            OurContainer.RegisterType<BasicGameContainer<LottoDominosPlayerItem, LottoDominosSaveInfo>>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
