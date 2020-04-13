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
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using ThreeLetterFunCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using ThreeLetterFunCP.Logic;
using CommonBasicStandardLibraries.Messenging;
using ThreeLetterFunCP.EventModels;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
//i think this is the most common things i like to do
namespace ThreeLetterFunCP.ViewModels
{
    public class ThreeLetterFunShellViewModel : BasicMultiplayerShellViewModel<ThreeLetterFunPlayerItem>, IHandleAsync<NextScreenEventModel>
    {
        public static EnumTestCategory TestMode = EnumTestCategory.None; //so if i set it, then this can force one to show up because its testing.
        //this could mean that for test, i can see ui alone.

        public ThreeLetterFunShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container, 
            IGameInfo gameData, 
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test)
            : base(mainContainer, container, gameData, basicData, save, test)
        {
        }
        protected override async Task ActivateAsync()
        {
            if (BasicData.MultiPlayer == true)
            {
                UIPlatform.ShowError("Needs to rethink multiplayer.");
                return;
            }
            await base.ActivateAsync();
            if (TestMode != EnumTestCategory.None && BasicData.GamePackageMode == EnumGamePackageMode.Production)
            {
                throw new BasicBlankException("Cannot have a test mode for screens because its in production");
            }
            if (TestMode != EnumTestCategory.None)
            {
                //needs to do the opening screen part.
                IScreen screen;
                switch (TestMode)
                {
                    case EnumTestCategory.FirstOption:
                        FirstScreen = MainContainer.Resolve<FirstOptionViewModel>();
                        screen = FirstScreen;
                        break;
                    case EnumTestCategory.CardsPlayer:
                        CardsScreen = MainContainer.Resolve<CardsPlayerViewModel>();
                        screen = CardsScreen;
                        break;
                    case EnumTestCategory.Advanced:
                        AdvancedScreen = MainContainer.Resolve<AdvancedOptionsViewModel>();
                        screen = AdvancedScreen;
                        break;
                    default:
                        throw new BasicBlankException("Rethink");
                }
                await LoadScreenAsync(screen);
            }
        }
        protected override bool CanStartWithOpenScreen => TestMode == EnumTestCategory.None;
        
        public FirstOptionViewModel? FirstScreen { get; set; }
        public AdvancedOptionsViewModel? AdvancedScreen { get; set; }
        public CardsPlayerViewModel? CardsScreen { get; set; }
        //for each step, will close the screens it was up to.
        protected override CustomBasicList<Type> GetAdditionalObjectsToReset()
        {
            return new CustomBasicList<Type>()
            {
                typeof(GenericCardShuffler<ThreeLetterFunCardData>)
            };
        }
        private async Task CloseStartingScreensAsync()
        {
            if (FirstScreen != null)
            {
                await CloseSpecificChildAsync(FirstScreen);
                FirstScreen = null;
                return;
            }
            if (AdvancedScreen != null)
            {
                await CloseSpecificChildAsync(AdvancedScreen);
                AdvancedScreen = null;
            }
            if (CardsScreen != null)
            {
                await CloseSpecificChildAsync(CardsScreen);
                CardsScreen = null;
            }
        }
        //the old screens should be closed first before this runs.

        //private async Task GetNextScreenAsync()
        //{
        //    //this should figure out the first screen or future screens.  in this case, this works well.
        //    ThreeLetterFunSaveInfo saveroot = MainContainer.Resolve<ThreeLetterFunSaveInfo>();

        //    if (saveroot.CanStart)
        //    {
        //        //this will load the main screen now.
        //        await StartNewGameAsync();
        //        return; //hopefully other rethinking is not required.  if so, do rethink.
        //    }

        //    if (saveroot.Level == EnumLevel.None)
        //    {
        //        //this means needs to load the first screen.  you have to first choose level.
        //        FirstScreen = MainContainer.Resolve<FirstOptionViewModel>();
        //        await LoadScreenAsync(FirstScreen);
        //        return;
        //    }
        //}


        //decided that to begin with, if you close out before choosing options, you start over again.
        //otherwise, a person would have to remember what they chose previously.
        protected override async Task GetStartingScreenAsync()
        {
            //await GetNextScreenAsync();

            FirstScreen = MainContainer.Resolve<FirstOptionViewModel>();
            await LoadScreenAsync(FirstScreen);

        }


        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<ThreeLetterFunMainViewModel>();
            return model;
        }

        async Task IHandleAsync<NextScreenEventModel>.HandleAsync(NextScreenEventModel message)
        {
            await CloseStartingScreensAsync();
            switch (message.Screen)
            {
                case BeginningClasses.EnumNextScreen.Advanced:
                    AdvancedScreen = MainContainer.Resolve<AdvancedOptionsViewModel>();
                    await LoadScreenAsync(AdvancedScreen);
                    break;
                case BeginningClasses.EnumNextScreen.Cards:
                    CardsScreen = MainContainer.Resolve<CardsPlayerViewModel>();
                    await LoadScreenAsync(CardsScreen);
                    break;
                case BeginningClasses.EnumNextScreen.Finished:
                    await StartNewGameAsync(); //hopefully this simple.
                    //if there is anything else that has to happen at this stage, this can do it.
                    break;
                default:
                    throw new BasicBlankException("Next screen not supported");
            }
        }
    }
}
