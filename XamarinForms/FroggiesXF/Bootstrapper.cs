using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Bootstrappers;
using FroggiesCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;

namespace FroggiesXF
{
    public class Bootstrapper : SinglePlayerBootstrapper<FroggiesShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            TestData!.ShowErrorMessageBoxes = false;
            return base.RegisterTestsAsync();
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<FroggiesShellViewModel>();
            OurContainer!.RegisterType<CustomSize>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
    public class CustomSize : IWidthHeight
    {
        int IWidthHeight.GetWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 30;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 60;
                return 80;
            }
        }
    }
}
