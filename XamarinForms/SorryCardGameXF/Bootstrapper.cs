using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicGamingUIXFLibrary.Views;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.CollectionClasses;
using SorryCardGameCP.Cards;
using SorryCardGameCP.Data;
using SorryCardGameCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace SorryCardGameXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<SorryCardGameShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<SorryCardGameShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<SorryCardGamePlayerItem, SorryCardGameSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<SorryCardGamePlayerItem, SorryCardGameSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<SorryCardGamePlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<SorryCardGameCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<SorryCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, SorryCardGameDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            //anything that needs to be registered will be here.

            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoices, PawnPiecesCP<EnumColorChoices>, SorryCardGamePlayerItem, SorryCardGameSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoices, PawnPiecesCP<EnumColorChoices>, SorryCardGamePlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoices, PawnPiecesCP<EnumColorChoices>, SorryCardGamePlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoices, PawnPiecesCP<EnumColorChoices>, SorryCardGamePlayerItem>),
                ViewType = typeof(BeginningChooseColorView<PawnPiecesCP<EnumColorChoices>, PawnPiecesXF<EnumColorChoices>, EnumColorChoices>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoices, PawnPiecesCP<EnumColorChoices>, SorryCardGamePlayerItem, SorryCardGameSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoices, PawnPiecesCP<EnumColorChoices>, SorryCardGamePlayerItem>)
                };
                return output;
            });
            OurContainer.RegisterType<StandardPickerSizeClass>();
            return Task.CompletedTask;
        }
    }
}
