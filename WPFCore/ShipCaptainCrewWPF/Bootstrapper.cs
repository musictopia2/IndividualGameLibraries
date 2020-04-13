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
using ShipCaptainCrewCP.Data;
using ShipCaptainCrewCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace ShipCaptainCrewWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ShipCaptainCrewShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<ShipCaptainCrewShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ShipCaptainCrewPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, ShipCaptainCrewPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();

            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}