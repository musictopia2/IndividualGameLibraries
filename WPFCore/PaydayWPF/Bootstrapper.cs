using BasicControlsAndWindowsCore.MVVMFramework.ViewLinkersPlusBinders;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using BasicGamingUIWPFLibrary.GameGraphics.Dice;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using BasicGamingUIWPFLibrary.Views;
using CommonBasicStandardLibraries.CollectionClasses;
using PaydayCP.Data;
using PaydayCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace PaydayWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<PaydayShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            //TestData!.PlayCategory = BasicGameFrameworkLibrary.TestUtilities.EnumPlayCategory.NoShuffle;
            //TestData!.ImmediatelyEndGame = true; //to test end game.
            //TestData!.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.RestoreOnly;
            return base.RegisterTestsAsync();
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<PaydayShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<PaydayPlayerItem, PaydaySaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<PaydayPlayerItem, PaydaySaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<PaydayPlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem, PaydaySaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem>),
                ViewType = typeof(BeginningChooseColorView<PawnPiecesCP<EnumColorChoice>, PawnPiecesWPF<EnumColorChoice>, EnumColorChoice>)
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
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportionWPF>(); //i think



            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}