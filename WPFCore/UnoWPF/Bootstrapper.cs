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
using UnoCP.Cards;
using UnoCP.Data;
using UnoCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.BaseColorCardsCP;
using BasicGameFrameworkLibrary.ColorCards;

namespace UnoWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<UnoShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            //TestData!.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.RestoreOnly;
            return base.RegisterTestsAsync();
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonMiscCards<UnoMainViewModel, UnoCardInformation>(ts.TagUsed);
            OurContainer!.RegisterType<BasicGameLoader<UnoPlayerItem, UnoSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<UnoPlayerItem, UnoSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<UnoPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<UnoCardInformation>>(true);
            OurContainer.RegisterType<ColorCardsShuffler<UnoCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, Phase10UnoDeck>();


            return Task.CompletedTask;
        }
    }
}