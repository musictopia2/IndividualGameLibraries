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
using FluxxCP.Cards;
using FluxxCP.Data;
using FluxxCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace FluxxXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<FluxxShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<FluxxShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<FluxxPlayerItem, FluxxSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<FluxxPlayerItem, FluxxSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<FluxxPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<FluxxCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<FluxxCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FluxxDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            return Task.CompletedTask;
        }
    }
}
