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
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using DominosRegularCP.Data;
using DominosRegularCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace DominosRegularWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<DominosRegularShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<DominosRegularShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<DominosRegularPlayerItem, DominosRegularSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<DominosRegularPlayerItem, DominosRegularSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<DominosRegularPlayerItem>>(true); //had to be set to true after all.
            //anything that needs to be registered will be here.

            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("scatter"); //this is needed so the boneyard part can work.
            OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>(); //has to do this to stop overflow and duplicates bug.
            OurContainer.RegisterType<BasicGameContainer<DominosRegularPlayerItem, DominosRegularSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}