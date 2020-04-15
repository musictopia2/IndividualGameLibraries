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
using ShipCaptainCrewCP.Data;
using ShipCaptainCrewCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace ShipCaptainCrewXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ShipCaptainCrewShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<ShipCaptainCrewShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ShipCaptainCrewPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, ShipCaptainCrewPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
