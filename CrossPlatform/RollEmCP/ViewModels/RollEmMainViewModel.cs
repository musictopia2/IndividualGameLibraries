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
using RollEmCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.Dice;
using RollEmCP.Data;
using BasicGameFrameworkLibrary.BasicEventModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
//i think this is the most common things i like to do
namespace RollEmCP.ViewModels
{
    [InstanceGame]
    public class RollEmMainViewModel : DiceGamesVM<SimpleDice>
    {
        private readonly RollEmMainGameClass _mainGame; //if we don't need, delete.
        private readonly BasicData _basicData;
        private readonly IEventAggregator _aggregator;
        private readonly GameBoardProcesses _gameBoard;

        public RollEmMainViewModel(CommandContainer commandContainer,
            RollEmMainGameClass mainGame,
            RollEmVMData viewModel, 
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller,
            IEventAggregator aggregator,
            RollEmGameContainer gameContainer,
            GameBoardProcesses gameBoard
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
            _basicData = basicData;
            _aggregator = aggregator;
            _gameBoard = gameBoard;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
            gameContainer.MakeMoveAsync = MakeMoveAsync;
        }

        private async Task MakeMoveAsync(int space)
        {
            if (_gameBoard.CanMakeMove(space) == false)
            {
                if (_gameBoard.HadRecent)
                {
                    if (_basicData.MultiPlayer)
                    {
                        await _mainGame.Network!.SendAllAsync("clearrecent");
                    }
                    await UIPlatform.ShowMessageAsync("Illegal Move");
                    _gameBoard.ClearRecent(true);
                    await _mainGame.ContinueTurnAsync();
                }
                return;
            }
            await _mainGame.MakeMoveAsync(space);
        }

        private void CommandContainer_ExecutingChanged()
        {
            _aggregator.RepaintBoard();
        }
        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }
        //anything else needed is here.
        private int _round;
        [VM]
        public int Round
        {
            get { return _round; }
            set
            {
                if (SetProperty(ref _round, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        protected override bool CanEnableDice()
        {
            return false; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return _mainGame!.SaveRoot!.GameStatus != EnumStatusList.NeedRoll;
        }
        public override bool CanRollDice()
        {
            return _mainGame!.SaveRoot!.GameStatus == EnumStatusList.NeedRoll;
        }

    }
}