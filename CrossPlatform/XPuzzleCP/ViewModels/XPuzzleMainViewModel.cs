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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Messenging;
using XPuzzleCP.Logic;
using XPuzzleCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGameFrameworkLibrary.BasicEventModels;
//i think this is the most common things i like to do
namespace XPuzzleCP.ViewModels
{
    [InstanceGame]
    public class XPuzzleMainViewModel : Screen, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
    {
        private readonly IEventAggregator _aggregator;
        //private readonly XPuzzleMainGameClass _mainGame;
        private readonly XPuzzleGameBoardClass _gameBoard;
        [Command(EnumCommandCategory.Plain)]
        public async Task MakeMoveAsync(XPuzzleSpaceInfo space)
        {
            await _gameBoard!.MakeMoveAsync(space);
            EnumMoveList NextMove = _gameBoard.Results();
            if (NextMove == EnumMoveList.TurnOver)
                return; //will automatically enable it again.
            if (NextMove == EnumMoveList.Won)
            {
                await UIPlatform.ShowMessageAsync("Congratulations, you won");
                await this.SendGameOverAsync(); //only if you won obviously.
            }
        }

        public XPuzzleMainViewModel(IEventAggregator aggregator, CommandContainer commandContainer, XPuzzleGameBoardClass gameBoard )
        {
            _aggregator = aggregator;
            CommandContainer = commandContainer;
            _gameBoard = gameBoard; //hopefully this works.  means you have to really rethink.
        }

        public CommandContainer CommandContainer { get; set; }

        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        public bool CanEnableBasics()
        {
            return true; //because maybe you can't enable it.
        }
        protected override Task ActivateAsync(IUIView view)
        {
            return base.ActivateAsync(view);
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            await _gameBoard.NewGameAsync();
        }
    }
}
