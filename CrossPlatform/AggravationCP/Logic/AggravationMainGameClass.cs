using AggravationCP.Data;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace AggravationCP.Logic
{
    [SingletonGame]
    public class AggravationMainGameClass
        : BoardDiceGameClass<AggravationPlayerItem, AggravationSaveInfo, EnumColorChoice, int>
    {
        public AggravationMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            AggravationVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            AggravationGameContainer container,
            StandardRollProcesses<SimpleDice, AggravationPlayerItem> roller,
            GameBoardProcesses gameBoard
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller)
        {
            _model = model;
            _command = command;
            _gameBoard = gameBoard;
            container.MakeMoveAsync = HumanMoveAsync;
        }

        private readonly AggravationVMData _model;
        private readonly CommandContainer _command;
        private readonly GameBoardProcesses _gameBoard;
        private async Task HumanMoveAsync(int space)
        {
            if (BasicData.MultiPlayer)
            {
                await Network!.SendMoveAsync(space);
            }
            await MakeMoveAsync(space);
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            _gameBoard.LoadSavedGame();
            BoardGameSaved(); //i think.
            SaveRoot.LoadMod(_model); //we usually need this.
            //can't send turn because view model not created yet at this point.
            _model.Cup!.CanShowDice = SaveRoot.DiceNumber > 0;
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            //if there is nothing, then just won't do anything.
            await Task.CompletedTask;
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SetUpDice();
            SaveRoot.LoadMod(_model); //we usually need this.
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            //hopefully the erasing of colors is already handled.
            await FinishUpAsync(isBeginning);
        }

        public override async Task StartNewTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                PrepStartTurn(); //if you did not choose colors, no need to prepstart because something else will do it.
                //code to run but only if you actually chose color.
                _gameBoard.StartTurn(); //this could be what was missing.
            }


            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon continue turn.  many board games require other things.
                if (Test!.DoubleCheck)
                {
                    Test.DoubleCheck = false;
                    await _gameBoard.GetValidMovesAsync(); //so i can see what is wrong.
                    return;
                }

            }
            await base.ContinueTurnAsync();
        }
        public override async Task MakeMoveAsync(int space)
        {
            await _gameBoard.MakeMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            _command.ManuelFinish = true;
            //if anything else is needed, do here.
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon ending turn.  many board games require other things. only do if the player actually chose colors.
                _gameBoard.StartTurn();
            }
            await StartNewTurnAsync();
        }

        public override async Task AfterChoosingColorsAsync()
        {
            //anything else that is needed after they finished choosing colors.
            _gameBoard.ClearBoard();
            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            await EndTurnAsync();
        }
        public override async Task AfterRollingAsync()
        {
            SaveRoot.DiceNumber = _model.Cup!.ValueOfOnlyDice;
            //anything needed after rolling is here.
            await _gameBoard.GetValidMovesAsync();
        }
    }
}
