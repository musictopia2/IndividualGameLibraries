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
using MonopolyCardGameCP.Cards;
using MonopolyCardGameCP.Data;
using MonopolyCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MonopolyCardGameWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<MonopolyCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<MonopolyCardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<MonopolyCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<MonopolyCardGameCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<MonopolyCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, MonopolyCardGameDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            return Task.CompletedTask;
        }
    }
}