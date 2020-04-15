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
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using RageCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace RageCardGameXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<RageCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
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
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            //anything that needs to be registered will be here.
            OurContainer.RegisterSingleton<IDeckCount, RageCardGameDeckCount>();
            OurContainer!.RegisterType<StandardWidthHeight>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            return Task.CompletedTask;
        }
    }
}
