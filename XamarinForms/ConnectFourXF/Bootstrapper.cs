using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicGamingUIXFLibrary.Views;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.CollectionClasses;
using ConnectFourCP.Data;
using ConnectFourCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;

namespace ConnectFourXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ConnectFourShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<ConnectFourShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<ConnectFourPlayerItem, ConnectFourSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ConnectFourPlayerItem, ConnectFourSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ConnectFourPlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectFourPlayerItem, ConnectFourSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectFourPlayerItem>>(false); //since this can be used more than once, go ahead and do false.  hopefully won't regret this.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectFourPlayerItem>>(true); //may need replacements (?)
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectFourPlayerItem>),
                ViewType = typeof(BeginningChooseColorView<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectFourPlayerItem, ConnectFourSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectFourPlayerItem>)
                };
                return output;
            });
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
