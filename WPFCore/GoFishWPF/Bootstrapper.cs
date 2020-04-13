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
using GoFishCP.Data;
using GoFishCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace GoFishWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<GoFishShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<GoFishShellViewModel, RegularSimpleCard>(registerCommonProportions: false);
            OurContainer!.RegisterType<BasicGameLoader<GoFishPlayerItem, GoFishSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<GoFishPlayerItem, GoFishSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<GoFishPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IProportionImage, LargeDrawableProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}