using BaseMahjongTilesCP;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Bootstrappers;
using MahJongSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace MahJongSolitaireXF
{
    public class Bootstrapper : SinglePlayerBootstrapper<MahJongSolitaireShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<MahJongSolitaireShellViewModel>();
            OurContainer!.RegisterType<BaseMahjongGlobals>(true);
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            OurContainer.RegisterType<MahjongShuffler>(true);
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
