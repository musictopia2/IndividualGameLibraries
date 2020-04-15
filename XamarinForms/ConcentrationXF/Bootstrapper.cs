using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using ConcentrationCP.Data;
using ConcentrationCP.Logic;
using ConcentrationCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace ConcentrationXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ConcentrationShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<ConcentrationShellViewModel, RegularSimpleCard>(customDeck: true);
            OurContainer!.RegisterType<BasicGameLoader<ConcentrationPlayerItem, ConcentrationSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ConcentrationPlayerItem, ConcentrationSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ConcentrationPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
            return Task.CompletedTask;
        }
    }
}
