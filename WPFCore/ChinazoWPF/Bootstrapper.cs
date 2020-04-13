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
using ChinazoCP.Data;
using ChinazoCP.Logic;
using ChinazoCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace ChinazoWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ChinazoShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<ChinazoShellViewModel, ChinazoCard>(registerCommonProportions: false, customDeck: true);
            OurContainer!.RegisterType<BasicGameLoader<ChinazoPlayerItem, ChinazoSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ChinazoPlayerItem, ChinazoSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ChinazoPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
            return Task.CompletedTask;
        }
    }
}