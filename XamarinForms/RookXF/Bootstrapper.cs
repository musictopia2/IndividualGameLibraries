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
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using RookCP.Cards;
using RookCP.Data;
using RookCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace RookXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<RookShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<RookShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<RookPlayerItem, RookSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<RookPlayerItem, RookSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<RookPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<RookCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<RookCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, RookDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            OurContainer.RegisterType<StandardWidthHeight>(); //hopefully this simple this time.
            OurContainer.RegisterType<StandardPickerSizeClass>();
            return Task.CompletedTask;
        }
    }
}
