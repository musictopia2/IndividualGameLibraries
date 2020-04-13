using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using ConnectTheDotsCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConnectTheDotsCP.Logic
{
    [SingletonGame]
    public class ConnectTheDotsMainGameClass
        : SimpleBoardGameClass<ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo, EnumColorChoice, int>
    {
        public ConnectTheDotsMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            ConnectTheDotsVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            ConnectTheDotsGameContainer container,
            GameBoardProcesses gameBoard
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, container)
        {
            _command = command;
            _gameBoard = gameBoard;
            container.MakeMoveAsync = PrivateMoveAsync;
        }

        private async Task PrivateMoveAsync(int dot)
        {
            if (_gameBoard.IsValidMove(dot) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            if (BasicData.MultiPlayer)
            {
                await Network!.SendMoveAsync(dot);
            }
            _command.StartExecuting();
            await _gameBoard.MakeMoveAsync(dot);
        }

        private readonly CommandContainer _command;
        private readonly GameBoardProcesses _gameBoard;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            if (PlayerList.DidChooseColors())
            {
                _gameBoard.LoadGame();
            }
            //anything else needed is here.
            return Task.CompletedTask;
        }
        public override Task PopulateSaveRootAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                _gameBoard.SaveGame();
            }
            return base.PopulateSaveRootAsync();
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

            }


            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon continue turn.  many board games require other things.

            }
            return base.ContinueTurnAsync();
        }
        public override async Task MakeMoveAsync(int space)
        {
            await _gameBoard.MakeMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            //if anything else is needed, do here.
            if (PlayerList.DidChooseColors())
            {
                _command.ManuelFinish = true;
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
    }
}