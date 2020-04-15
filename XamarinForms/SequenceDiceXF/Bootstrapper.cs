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
using SequenceDiceCP.Data;
using SequenceDiceCP.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGamingUIXFLibrary.GameGraphics.Dice;
using BasicGameFrameworkLibrary.Dice;

namespace SequenceDiceXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<SequenceDiceShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
			
			OurContainer!.RegisterType<StandardPickerSizeClass>(); //needs to keep no matter what since this is extra for xamarin forms

            OurContainer!.RegisterNonSavedClasses<SequenceDiceShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<SequenceDicePlayerItem, SequenceDiceSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<SequenceDicePlayerItem, SequenceDiceSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<SequenceDicePlayerItem>>(true); //had to be set to true after all.

            OurContainer.RegisterType<BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, SequenceDicePlayerItem, SequenceDiceSaveInfo>>();
            OurContainer.RegisterType<BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, SequenceDicePlayerItem>>(false); //did have to replace though.
            OurContainer.RegisterType<BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, SequenceDicePlayerItem>>(true);
            //all piece choices should be here.
            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, SequenceDicePlayerItem>),
                ViewType = typeof(BeginningChooseColorView<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);
            MiscDelegates.GetMiscObjectsToReplace = (() =>
            {
                CustomBasicList<Type> output = new CustomBasicList<Type>()
                {
                    typeof(BeginningColorProcessorClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, SequenceDicePlayerItem, SequenceDiceSaveInfo>),
                    typeof(BeginningColorModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, SequenceDicePlayerItem>)
                };
                return output;
            });
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, SequenceDicePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            return Task.CompletedTask;
        }
    }
}
