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
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using FlinchCP.Cards;
using FlinchCP.Data;
using FlinchCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace FlinchXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<FlinchShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<FlinchShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<FlinchPlayerItem, FlinchSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<FlinchPlayerItem, FlinchSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<FlinchPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<FlinchCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<FlinchCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FlinchDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            return Task.CompletedTask;
        }
    }
}
