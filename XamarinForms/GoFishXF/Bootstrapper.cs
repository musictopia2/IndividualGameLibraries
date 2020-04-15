using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using GoFishCP.Data;
using GoFishCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace GoFishXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<GoFishShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterCommonRegularCards<GoFishShellViewModel, RegularSimpleCard>();
            OurContainer!.RegisterType<BasicGameLoader<GoFishPlayerItem, GoFishSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<GoFishPlayerItem, GoFishSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<GoFishPlayerItem>>(true); //had to be set to true after all.
            //OurContainer.RegisterSingleton<IProportionImage, LargeDrawableProportion>(ts.TagUsed);
            OurContainer.RegisterType<StandardPickerSizeClass>();
            return Task.CompletedTask;
        }
    }
}
