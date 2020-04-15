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
using PaydayCP.Data;
using PaydayCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGamingUIXFLibrary.GameGraphics.Dice;
using BasicGameFrameworkLibrary.Dice;

namespace PaydayXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<PaydayShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<PaydayShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<PaydayPlayerItem, PaydaySaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<PaydayPlayerItem, PaydaySaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<PaydayPlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, CustomProportionXF>("main"); //here too.
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem, PaydaySaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem>),
                ViewType = typeof(BeginningChooseColorView<PawnPiecesCP<EnumColorChoice>, PawnPiecesXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem, PaydaySaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem>)
                };
                return output;
            });
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, PaydayPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportionXF>(); //i think
            return Task.CompletedTask;
        }
    }
}
