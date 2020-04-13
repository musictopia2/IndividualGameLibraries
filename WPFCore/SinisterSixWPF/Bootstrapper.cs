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
using SinisterSixCP.Data;
using SinisterSixCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace SinisterSixWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<SinisterSixShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<SinisterSixShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<SinisterSixPlayerItem, SinisterSixSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<SinisterSixPlayerItem, SinisterSixSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<SinisterSixPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<EightSidedDice, SinisterSixPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, EightSidedDice>();

            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}