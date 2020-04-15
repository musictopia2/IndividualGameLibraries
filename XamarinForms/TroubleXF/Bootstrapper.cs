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
using TroubleCP.Data;
using TroubleCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGamingUIXFLibrary.GameGraphics.Dice;
using BasicGameFrameworkLibrary.Dice;

namespace TroubleXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<TroubleShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<TroubleShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<TroublePlayerItem, TroubleSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<TroublePlayerItem, TroubleSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<TroublePlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, TroublePlayerItem, TroubleSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, TroublePlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, TroublePlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, TroublePlayerItem>),
                ViewType = typeof(BeginningChooseColorView<MarblePiecesCP<EnumColorChoice>, MarblePiecesXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, TroublePlayerItem, TroubleSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, TroublePlayerItem>)
                };
                return output;
            });
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, TroublePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterType<BoardPosition>(); //to register the gameboard position.
            return Task.CompletedTask;
        }
    }
}
