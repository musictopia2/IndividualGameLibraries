using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGamingUIXFLibrary.GameGraphics.Dice;
using SnakesAndLaddersCP.Data;
using SnakesAndLaddersCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace SnakesAndLaddersXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<SnakesAndLaddersShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<SnakesAndLaddersShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<SnakesAndLaddersPlayerItem>>(true); //had to be set to true after all.

            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, SnakesAndLaddersPlayerItem>>(); //iffy.
            OurContainer.RegisterType<BasicGameContainer<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}
