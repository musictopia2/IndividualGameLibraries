using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using MilkRunCP.Cards;
using MilkRunCP.Data;
using MilkRunCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace MilkRunXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<MilkRunShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<MilkRunShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<MilkRunPlayerItem, MilkRunSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<MilkRunPlayerItem, MilkRunSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<MilkRunPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<MilkRunCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<MilkRunCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, MilkRunDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            return Task.CompletedTask;
        }
    }
}
