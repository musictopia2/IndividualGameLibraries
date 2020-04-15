using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGamingUIXFLibrary.GameGraphics.Dice;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThinkTwiceCP.Data;
using ThinkTwiceCP.ViewModels;

namespace ThinkTwiceXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ThinkTwiceShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<ThinkTwiceShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<ThinkTwicePlayerItem, ThinkTwiceSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ThinkTwicePlayerItem, ThinkTwiceSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ThinkTwicePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, ThinkTwicePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            return Task.CompletedTask;
        }
    }
}
