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
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TeeItUpCP.Cards;
using TeeItUpCP.Data;
using TeeItUpCP.ViewModels;
namespace TeeItUpWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<TeeItUpShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            TestData!.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.RestoreOnly;
            TestData.StatePosition = 4;
            return base.RegisterTestsAsync();
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<TeeItUpShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<TeeItUpPlayerItem, TeeItUpSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<TeeItUpPlayerItem, TeeItUpSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<TeeItUpPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<TeeItUpCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<TeeItUpCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, TeeItUpDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            return Task.CompletedTask;
        }
    }
}