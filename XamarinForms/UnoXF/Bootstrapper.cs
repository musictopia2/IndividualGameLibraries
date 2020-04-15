using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using UnoCP.Cards;
using UnoCP.Data;
using UnoCP.ViewModels;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.BaseColorCardsCP;

namespace UnoXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<UnoShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
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
            OurContainer.RegisterType<StandardPickerSizeClass>();
            return Task.CompletedTask;
        }
    }
}
