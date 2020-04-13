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
using FluxxCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using FluxxCP.Containers;
//i think this is the most common things i like to do
namespace FluxxCP.ViewModels
{
    public class FluxxShellViewModel : BasicMultiplayerShellViewModel<FluxxPlayerItem>
    {
        private readonly FluxxDelegates _delegates;

        public FluxxShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container, 
            IGameInfo gameData, 
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test,
            FluxxDelegates delegates
            )
            : base(mainContainer, container, gameData, basicData, save, test)
        {
            delegates.LoadMainScreenAsync = LoadMainScreenAsync;
            delegates.LoadProperActionScreenAsync = LoadActionScreenAsync;
            delegates.LoadKeeperScreenAsync = LoadKeeperScreenAsync;
            delegates.CurrentScreen = GetCurrentScreenCategory;
            _delegates = delegates;
        }

        //internal Func<FluxxGameContainer, Task>? LoadProperActionScreenAsync { get; set; }
        //internal Func<Task>? LoadMainScreenAsync { get; set; }
        ////we may need the container for the keepers (?)
        //internal Func<FluxxGameContainer, Task>? LoadKeeperScreenAsync { get; set; }

        private async Task LoadMainScreenAsync()
        {
            ClearSubscriptions();
            //CommandContainer.ClearLists(); //try this too.  taking some risks.
            await StartNewGameAsync();
            RefreshEnables();
            //CommandContainer.ManuelFinish = false; //try this too.
            //CommandContainer.StopExecuting();//try this
        }
        private EnumActionScreen GetCurrentScreenCategory()
        {
            if (MainVM != null)
            {
                return EnumActionScreen.None;
            }
            if (KeeperScreen != null)
            {
                return EnumActionScreen.KeeperScreen;
            }
            if (ActionScreen != null)
            {
                return EnumActionScreen.ActionScreen;
            }
            throw new BasicBlankException("Cannot find the screen used.  Rethink");
        }

        protected override async Task StartNewGameAsync()
        {
            await CloseActionScreenAsync();
            await CloseKeeperScreenAsync();
            
            await base.StartNewGameAsync();
        }

        private async Task LoadActionScreenAsync(ActionContainer actionContainer)
        {
            if (ActionScreen != null)
            {
                throw new BasicBlankException("Previous action was not loaded.  Rethink");
            }
            await CloseMainAsync();
            await CloseKeeperScreenAsync();
            //much harder to figure out which screen to load.
            switch (actionContainer.ActionCategory)
            {
                case EnumActionCategory.None:
                    break;
                case EnumActionCategory.Rules:
                    break;
                case EnumActionCategory.Directions:
                    break;
                case EnumActionCategory.DoAgain:
                    break;
                case EnumActionCategory.TradeHands:
                    break;
                case EnumActionCategory.UseTake:
                    break;
                case EnumActionCategory.Everybody1:
                    break;
                case EnumActionCategory.DrawUse:
                    break;
                case EnumActionCategory.FirstRandom:
                    break;
                default:
                    break;
            }

            ActionScreen = actionContainer.ActionCategory switch
            {
                EnumActionCategory.Rules => MainContainer.Resolve<ActionDiscardRulesViewModel>(),
                EnumActionCategory.Directions => MainContainer.Resolve<ActionDirectionViewModel>(),
                EnumActionCategory.DoAgain => MainContainer.Resolve<ActionDoAgainViewModel>(),
                EnumActionCategory.DrawUse => MainContainer.Resolve<ActionDrawUseViewModel>(),
                EnumActionCategory.Everybody1 => MainContainer.Resolve<ActionEverybodyGetsOneViewModel>(),
                EnumActionCategory.FirstRandom => MainContainer.Resolve<ActionFirstCardRandomViewModel>(),
                EnumActionCategory.TradeHands => MainContainer.Resolve<ActionTradeHandsViewModel>(),
                EnumActionCategory.UseTake => MainContainer.Resolve<ActionTakeUseViewModel>(),
                _ => throw new BasicBlankException("Cannot figure out action screen.  Rethink")
            };
            await LoadScreenAsync(ActionScreen);
            RefreshEnables();
        }

        private void RefreshEnables()
        {
            if (_delegates.RefreshEnables == null)
            {
                throw new BasicBlankException("Nobody is refreshing enables.  Rethink");
            }
            _delegates.RefreshEnables.Invoke();
        }

        private async Task LoadKeeperScreenAsync(KeeperContainer keeperContainer)
        {
            //this is easiest one.
            if (KeeperScreen != null)
            {
                return; //try to ignore because we already have the keeper screen (?)
            }
            await CloseActionScreenAsync();
            await CloseMainAsync();
            KeeperScreen = keeperContainer.Section switch
            {
                EnumKeeperSection.None => MainContainer.Resolve<KeeperShowViewModel>(),
                EnumKeeperSection.Trash => MainContainer.Resolve<KeeperTrashViewModel>(),
                EnumKeeperSection.Steal => MainContainer.Resolve<KeeperStealViewModel>(),
                EnumKeeperSection.Exchange => MainContainer.Resolve<KeeperExchangeViewModel>(),
                _ => throw new BasicBlankException("Not Supported"),
            };
            await LoadScreenAsync(KeeperScreen);

        }
        private async Task CloseMainAsync()
        {
            if (MainVM != null)
            {
                await CloseSpecificChildAsync(MainVM);
                MainVM = null;
            }
        }
        
        private async Task CloseActionScreenAsync()
        {
            if (ActionScreen != null)
            {
                await CloseSpecificChildAsync(ActionScreen);
                ActionScreen = null;
            }
        }
        private async Task CloseKeeperScreenAsync()
        {
            if (KeeperScreen != null)
            {
                await CloseSpecificChildAsync(KeeperScreen);
                KeeperScreen = null;
            }
        }
        public BasicActionScreen? ActionScreen { get; set; }
        public BasicKeeperScreen? KeeperScreen { get; set; }
        

        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<FluxxMainViewModel>();
            return model;
        }
    }
}
