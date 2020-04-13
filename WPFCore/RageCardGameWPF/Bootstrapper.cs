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
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.Bootstrappers;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using RageCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace RageCardGameWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<RageCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<RageCardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<RageCardGamePlayerItem, RageCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<RageCardGamePlayerItem, RageCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<RageCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<RageCardGameCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<RageCardGameCardInformation>>();
            //OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            //OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            //anything that needs to be registered will be here.
            OurContainer.RegisterSingleton<IDeckCount, RageCardGameDeckCount>();
            OurContainer!.RegisterType<StandardWidthHeight>();

            //change view model for area if not using 2 player.
            return Task.CompletedTask;
        }
    }
}