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
using ChineseCheckersCP.Data;
using ChineseCheckersCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;

namespace ChineseCheckersXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<ChineseCheckersShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<ChineseCheckersShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<ChineseCheckersPlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, ChineseCheckersPlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, ChineseCheckersPlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, ChineseCheckersPlayerItem>),
                ViewType = typeof(BeginningChooseColorView<MarblePiecesCP<EnumColorChoice>, MarblePiecesXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, ChineseCheckersPlayerItem>)
                };
                return output;
            });
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
