using BasicControlsAndWindowsCore.MVVMFramework.ViewLinkersPlusBinders;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using BasicGamingUIWPFLibrary.Views;
using CommonBasicStandardLibraries.CollectionClasses;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Graphics;
using LifeBoardGameCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<LifeBoardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override Task RegisterTestsAsync()
        {
            //TestData!.PlayCategory = BasicGameFrameworkLibrary.TestUtilities.EnumPlayCategory.NoShuffle;

            //LifeBoardGameGameContainer.StartCollegeCareer = true;
            //TestData!.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.RestoreOnly;
            //TestData.NoAnimations = true;
            return base.RegisterTestsAsync();
        }
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<LifeBoardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<LifeBoardGamePlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, BoardProportion>(""); //here too.
            OurContainer.RegisterSingleton<IProportionImage, CardProportion>();
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem>),
                ViewType = typeof(BeginningChooseColorView<CarPieceCP, CarPieceWPF, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem>)
                };
                return output;
            });
            BeginningColorDimensions.GraphicsHeight = 248;
            BeginningColorDimensions.GraphicsWidth = 136;
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}