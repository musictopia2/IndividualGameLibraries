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
using ChessCP.Data;
using ChessCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;

namespace ChessXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ChessShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<ChessShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<ChessPlayerItem, ChessSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ChessPlayerItem, ChessSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ChessPlayerItem>>(true); //had to be set to true after all.
            OurContainer!.RegisterSingleton<IProportionBoard, CustomProportion>("main");
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ChessPlayerItem, ChessSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ChessPlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ChessPlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ChessPlayerItem>),
                ViewType = typeof(BeginningChooseColorView<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ChessPlayerItem, ChessSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ChessPlayerItem>)
                };
                return output;
            });
            return Task.CompletedTask;
        }
    }
}
