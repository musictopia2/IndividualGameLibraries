using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using PassOutDiceGameCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace PassOutDiceGameCP.Logic
{
    [SingletonGame]
    public class PassOutDiceGameMainGameClass
        : BoardDiceGameClass<PassOutDiceGamePlayerItem, PassOutDiceGameSaveInfo, EnumColorChoice, int>
    {
        public PassOutDiceGameMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            PassOutDiceGameVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            PassOutDiceGameGameContainer container,
            StandardRollProcesses<SimpleDice, PassOutDiceGamePlayerItem> roller,
            GameBoardProcesses gameBoard
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller)
        {
            _model = model;
            _command = command;
            _gameBoard = gameBoard;
        }

        private readonly PassOutDiceGameVMData _model;
        private readonly CommandContainer _command;
        private readonly GameBoardProcesses _gameBoard;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            SaveRoot.LoadMod(_model); //we usually need this.
            _gameBoard.LoadSavedGame();
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
        public override Task PopulateSaveRootAsync()
        {
            if (PlayerList.DidChooseColors() == true)
            {
                _gameBoard.SaveGame();
            }
            return base.PopulateSaveRootAsync();
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
                SaveRoot.DidRoll = false;
            }


            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors() == false)
            {
                await base.ContinueTurnAsync();
            }
            if (SaveRoot.DidRoll == false)
            {
                if (BasicData.MultiPlayer && SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                {
                    Check!.IsEnabled = true;
                    return;
                }
                SaveRoot.DidRoll = true;
                await Roller.RollDiceAsync();
                return;
            }
            await base.ContinueTurnAsync();
        }
        public override async Task MakeMoveAsync(int space)
        {
            _gameBoard.MakeMove(space);
            int wons = _gameBoard.WhoWon;
            if (wons > 0)
            {
                SingleInfo = PlayerList[wons]; //i think should be whoever actually won.
                await ShowWinAsync();
                return;
            }
            await EndTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            _command.ManuelFinish = true;
            //if anything else is needed, do here.
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon ending turn.  many board games require other things. only do if the player actually chose colors.

            }
            await StartNewTurnAsync();
        }

        public override async Task AfterChoosingColorsAsync()
        {
            //anything else that is needed after they finished choosing colors.

            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            _gameBoard.ClearBoard();
            await EndTurnAsync();
        }
        public override async Task AfterRollingAsync()
        {
            SaveRoot.DidRoll = true;
            //anything needed after rolling is here.
            await ContinueTurnAsync();
        }
        //may be no need for computer part because i show that computer won't even play this one either.
    }
}