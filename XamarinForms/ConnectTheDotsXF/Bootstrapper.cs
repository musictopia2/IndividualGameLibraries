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
using ConnectTheDotsCP.Data;
using ConnectTheDotsCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;

namespace ConnectTheDotsXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ConnectTheDotsShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<ConnectTheDotsShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ConnectTheDotsPlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>(""); //here too.
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectTheDotsPlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectTheDotsPlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectTheDotsPlayerItem>),
                ViewType = typeof(BeginningChooseColorView<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectTheDotsPlayerItem>)
                };
                return output;
            });
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
