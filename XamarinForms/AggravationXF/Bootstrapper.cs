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
using AggravationCP.Data;
using AggravationCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGamingUIXFLibrary.GameGraphics.Dice;
using BasicGameFrameworkLibrary.Dice;

namespace AggravationXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<AggravationShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<AggravationShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<AggravationPlayerItem, AggravationSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<AggravationPlayerItem, AggravationSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<AggravationPlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, AggravationPlayerItem, AggravationSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, AggravationPlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, AggravationPlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, AggravationPlayerItem>),
                ViewType = typeof(BeginningChooseColorView<MarblePiecesCP<EnumColorChoice>, MarblePiecesXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, AggravationPlayerItem, AggravationSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, AggravationPlayerItem>)
                };
                return output;
            });
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, AggravationPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterType<BoardPosition>(); //to register the gameboard position.
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
