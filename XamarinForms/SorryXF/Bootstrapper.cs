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
using SorryCP.Data;
using SorryCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.MiscClasses;
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;

namespace SorryXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<SorryShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<SorryShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<SorryPlayerItem, SorrySaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<SorryPlayerItem, SorrySaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<SorryPlayerItem>>(true); //had to be set to true after all.

            OurContainer!.RegisterSingleton<IProportionBoard, BoardProportion>(""); //here too.
            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, SorryPlayerItem, SorrySaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, SorryPlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, SorryPlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, SorryPlayerItem>),
                ViewType = typeof(BeginningChooseColorView<MarblePiecesCP<EnumColorChoice>, MarblePiecesXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, SorryPlayerItem, SorrySaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, SorryPlayerItem>)
                };
                return output;
            });
            //anything that needs to be registered will be here.
            OurContainer.RegisterType<DrawShuffleClass<CardInfo, SorryPlayerItem>>();
            OurContainer.RegisterType<GenericCardShuffler<CardInfo>>();
            OurContainer.RegisterSingleton<IDeckCount, DeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(""); //i think.
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }

}
