using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using LifeCardGameCP.Cards;
using LifeCardGameCP.Data;
using LifeCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeCardGameWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<LifeCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<LifeCardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<LifeCardGamePlayerItem, LifeCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<LifeCardGamePlayerItem, LifeCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<LifeCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<LifeCardGameCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<LifeCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, LifeCardGameDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            return Task.CompletedTask;
        }
    }
}