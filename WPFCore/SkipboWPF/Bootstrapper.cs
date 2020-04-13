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
using SkipboCP.Cards;
using SkipboCP.Data;
using SkipboCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SkipboWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<SkipboShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            //TestData!.PlayCategory = BasicGameFrameworkLibrary.TestUtilities.EnumPlayCategory.NoShuffle;
            //TestData.WhoStarts = 2;
            return base.RegisterTestsAsync();
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<SkipboShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<SkipboPlayerItem, SkipboSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<SkipboPlayerItem, SkipboSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<SkipboPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<SkipboCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<SkipboCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, SkipboDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            //anything that needs to be registered will be here.



            return Task.CompletedTask;
        }
    }
}