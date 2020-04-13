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
using YahtzeeHandsDownCP.Cards;
using YahtzeeHandsDownCP.Data;
using YahtzeeHandsDownCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace YahtzeeHandsDownWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<YahtzeeHandsDownShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<YahtzeeHandsDownShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<YahtzeeHandsDownPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<YahtzeeHandsDownCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<YahtzeeHandsDownCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, YahtzeeHandsDownDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            OurContainer.RegisterSingleton<IProportionImage, ComboProportion>("combo");

            return Task.CompletedTask;
        }
    }
}