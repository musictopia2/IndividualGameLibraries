using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using MonasteryCardGameCP.Data;
using MonasteryCardGameCP.Logic;
using MonasteryCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace MonasteryCardGameWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<MonasteryCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<MonasteryCardGameShellViewModel, MonasteryCardInfo>(customDeck: true, registerCommonProportions: false);
            OurContainer!.RegisterType<BasicGameLoader<MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<MonasteryCardGamePlayerItem>>(true); //had to be set to true after all.
            //anything that needs to be registered will be here.
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
            return Task.CompletedTask;
        }
    }
}