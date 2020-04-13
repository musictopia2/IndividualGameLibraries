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
using HitTheDeckCP.Cards;
using HitTheDeckCP.Data;
using HitTheDeckCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HitTheDeckWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<HitTheDeckShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<HitTheDeckShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<HitTheDeckPlayerItem, HitTheDeckSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<HitTheDeckPlayerItem, HitTheDeckSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<HitTheDeckPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<HitTheDeckCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<HitTheDeckCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, HitTheDeckDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            //anything that needs to be registered will be here.



            return Task.CompletedTask;
        }
    }
}