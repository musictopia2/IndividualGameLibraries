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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommonInterfaces;
using ChineseCheckersCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;

namespace ChineseCheckersCP.Logic
{
    [SingletonGame]
    public class ChineseCheckersMainGameClass
        : SimpleBoardGameClass<ChineseCheckersPlayerItem, ChineseCheckersSaveInfo, EnumColorChoice, int>, IMiscDataNM
    {
        public ChineseCheckersMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            ChineseCheckersVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            ChineseCheckersGameContainer container,
            GameBoardProcesses gameBoard
            ) : base(resolver, aggregator, basic, test, model, state,delay, command, container)
        {
            _gameContainer = container;
            _gameBoard = gameBoard;
            _gameContainer.Model = model;
            _gameContainer.CanMove = (() => !command.IsExecuting);
            _gameContainer.MakeMoveAsync = PrivateMoveAsync;
            SaveRoot.Init(_gameContainer);
        }

        private async Task PrivateMoveAsync(int space)
        {
            if (SaveRoot!.PreviousSpace == space && _gameBoard.WillContinueTurn() == false)
            {
                SaveRoot.Instructions = "Choose a piece to move";
                if (BasicData!.MultiPlayer == true)
                    await Network!.SendAllAsync("undomove");
                await _gameBoard.UnselectPieceAsync();
                return;
            }
            else if (SaveRoot.PreviousSpace == space)
                return;
            if (_gameBoard!.IsValidMove(space) == false)
                return;
            if (SaveRoot.PreviousSpace == 0)
            {
                if (SingleInfo!.PieceList.Any(Items => Items == space) == false)
                    return;
                SaveRoot.Instructions = "Select where to move to";
                if (BasicData!.MultiPlayer == true)
                    await Network!.SendAllAsync("pieceselected", space);
                await _gameBoard.HighlightItemAsync(space);
                return;
            }
            SaveRoot.Instructions = "Making Move";
            if (BasicData!.MultiPlayer == true)
                await Network!.SendMoveAsync(space);
            _gameContainer.Command.StartExecuting(); //i think.
            await MakeMoveAsync(space);
        }

        private readonly ChineseCheckersGameContainer _gameContainer;
        private readonly GameBoardProcesses _gameBoard;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            SaveRoot.Init(_gameContainer); //hopefully this simple.
            _gameBoard.LoadSavedGame(); //i think.
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
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            //hopefully the erasing of colors is already handled.
            await FinishUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "undomove":
                    await _gameBoard.UnselectPieceAsync();
                    return;
                case "pieceselected":
                    await _gameBoard.HighlightItemAsync(int.Parse(content));
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                PrepStartTurn(); //if you did not choose colors, no need to prepstart because something else will do it.
                //code to run but only if you actually chose color.
                _gameBoard.StartTurn();
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                {
                    SaveRoot!.Instructions = "Choose a piece to move";
                }
                else
                {
                    SaveRoot!.Instructions = $"Waitng for {SingleInfo.NickName} to take their turn";
                }
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
                //can do extra things upon ending turn.  many board games require other things. only do if the player actually chose colors.
                _gameContainer.Command.ManuelFinish = true; //i think in this case, yes.

            }
            await StartNewTurnAsync();
        }

        public override async Task AfterChoosingColorsAsync()
        {
            //anything else that is needed after they finished choosing colors.

            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            _gameBoard.ClearBoard(); //i think here.
            await EndTurnAsync();
        }
    }
}
