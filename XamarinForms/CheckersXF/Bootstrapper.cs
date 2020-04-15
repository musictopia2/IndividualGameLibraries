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
using CheckersCP.Data;
using CheckersCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;

namespace CheckersXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<CheckersShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<CheckersShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<CheckersPlayerItem, CheckersSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CheckersPlayerItem, CheckersSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CheckersPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, CheckersPlayerItem, CheckersSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, CheckersPlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, CheckersPlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, CheckersPlayerItem>),
                ViewType = typeof(BeginningChooseColorView<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, CheckersPlayerItem, CheckersSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, CheckersPlayerItem>)
                };
                return output;
            });
            return Task.CompletedTask;
        }
    }
}
