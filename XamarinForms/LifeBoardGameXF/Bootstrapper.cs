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
using LifeBoardGameCP.Data;
using LifeBoardGameCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using LifeBoardGameCP.Graphics;

namespace LifeBoardGameXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<LifeBoardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<LifeBoardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<LifeBoardGamePlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, BoardProportion>(""); //here too.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>();
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem>),
                ViewType = typeof(BeginningChooseColorView<CarPieceCP, CarPieceXF, EnumColorChoice>)
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

            BeginningColorDimensions.GraphicsHeight = 248 * .8f;
            BeginningColorDimensions.GraphicsWidth = 136 * .8f;
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
