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
using Rummy500CP.Data;
using Rummy500CP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace Rummy500WPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<Rummy500ShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<Rummy500ShellViewModel, RegularRummyCard>(aceLow: false, registerCommonProportions: false);
            OurContainer!.RegisterType<BasicGameLoader<Rummy500PlayerItem, Rummy500SaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<Rummy500PlayerItem, Rummy500SaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<Rummy500PlayerItem>>(true); //had to be set to true after all.

            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);

            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}