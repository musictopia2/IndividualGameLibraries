using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using DummyRummyCP.Data;
using DummyRummyCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace DummyRummyXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<DummyRummyShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<DummyRummyShellViewModel, RegularRummyCard>(registerCommonProportions: false);
            OurContainer!.RegisterType<BasicGameLoader<DummyRummyPlayerItem, DummyRummySaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<DummyRummyPlayerItem, DummyRummySaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<DummyRummyPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
            return Task.CompletedTask;
        }
    }
}
