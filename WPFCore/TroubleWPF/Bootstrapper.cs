using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGamingUIWPFLibrary.Bootstrappers;
using TroubleCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using TroubleCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicControlsAndWindowsCore.MVVMFramework.ViewLinkersPlusBinders;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIWPFLibrary.Views;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.Dice;
using BasicGamingUIWPFLibrary.GameGraphics.Dice;
//i think this is the most common things i like to do
namespace TroubleWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper <TroubleShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        
        protected override Task ConfigureAsync()
        {
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
                ViewType = typeof(BeginningChooseColorView<MarblePiecesCP<EnumColorChoice>, MarblePiecesWPF<EnumColorChoice>, EnumColorChoice>)
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
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();


            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}