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
using DeadDie96CP.Data;
using DeadDie96CP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace DeadDie96XF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<DeadDie96ShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<DeadDie96ShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<DeadDie96PlayerItem, DeadDie96SaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<DeadDie96PlayerItem, DeadDie96SaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<DeadDie96PlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<TenSidedDice, DeadDie96PlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, TenSidedDice>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
