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
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using FiveCrownsCP.Cards;
using FiveCrownsCP.Data;
using FiveCrownsCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FiveCrownsWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<FiveCrownsShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<FiveCrownsShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<FiveCrownsPlayerItem, FiveCrownsSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<FiveCrownsPlayerItem, FiveCrownsSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<FiveCrownsPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<FiveCrownsCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<FiveCrownsCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FiveCrownsDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>("");

            return Task.CompletedTask;
        }
    }
}