using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using RackoCP.Cards;
using RackoCP.Data;
using RackoCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RackoWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<RackoShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            //TestData.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.RestoreOnly;
            //TestData!.ImmediatelyEndGame = true;
            return base.RegisterTestsAsync();
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<RackoShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<RackoPlayerItem, RackoSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<RackoPlayerItem, RackoSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<RackoPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<RackoCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<RackoCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, RackoDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            //anything that needs to be registered will be here.



            return Task.CompletedTask;
        }
    }
}