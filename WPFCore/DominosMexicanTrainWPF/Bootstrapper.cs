using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using DominosMexicanTrainCP.Data;
using DominosMexicanTrainCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace DominosMexicanTrainWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<DominosMexicanTrainShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task RegisterTestsAsync()
        {
            //TestData!.AlwaysNewGame = true;
            //TestData!.DoubleCheck = true;
            //TestData!.EndRoundEarly = true; //we need to test that part too.
            //TestData!.ComputerEndsTurn = true;
            //TestData.AllowAnyMove = true;
            return base.RegisterTestsAsync();
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<DominosMexicanTrainShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<DominosMexicanTrainPlayerItem>>(true); //had to be set to true after all.
            //anything that needs to be registered will be here.

            OurContainer.RegisterSingleton<IProportionImage, CustomProportions>(ts.TagUsed);
            OurContainer.RegisterSingleton<IProportionImage, CustomProportions>("scatter"); //this is needed so the boneyard part can work.
            OurContainer.RegisterType<DominosBasicShuffler<MexicanDomino>>(true);
            OurContainer.RegisterSingleton<IDeckCount, MexicanDomino>(); //has to do this to stop overflow and duplicates bug.
            OurContainer.RegisterSingleton<IProportionBoard, CustomProportions>();
            OurContainer.RegisterType<BasicGameContainer<DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}