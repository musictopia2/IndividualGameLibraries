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
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using ItalianDominosCP.Data;
using ItalianDominosCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace ItalianDominosXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ItalianDominosShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<ItalianDominosShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<ItalianDominosPlayerItem, ItalianDominosSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ItalianDominosPlayerItem, ItalianDominosSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ItalianDominosPlayerItem>>(true); //had to be set to true after all.
            //anything that needs to be registered will be here.

            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("scatter"); //this is needed so the boneyard part can work.
            OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>(); //has to do this to stop overflow and duplicates bug.
            OurContainer.RegisterType<BasicGameContainer<ItalianDominosPlayerItem, ItalianDominosSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}
