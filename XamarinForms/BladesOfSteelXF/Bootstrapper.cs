using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using BladesOfSteelCP.Data;
using BladesOfSteelCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace BladesOfSteelXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<BladesOfSteelShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }
        
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<BladesOfSteelShellViewModel, RegularSimpleCard>(aceLow: false);
            OurContainer!.RegisterType<BasicGameLoader<BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<BladesOfSteelPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            return Task.CompletedTask;
        }
    }
}
