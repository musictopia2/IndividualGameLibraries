using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using MillebournesCP.Cards;
using MillebournesCP.Data;
using MillebournesCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace MillebournesXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<MillebournesShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<MillebournesShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<MillebournesPlayerItem, MillebournesSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<MillebournesPlayerItem, MillebournesSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<MillebournesPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<MillebournesCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<MillebournesCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, MillebournesDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            return Task.CompletedTask;
        }
    }
}
