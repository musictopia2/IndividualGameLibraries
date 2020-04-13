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
using ChessCP.Data;
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
using BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;

namespace ChessCP.Logic
{
    [SingletonGame]
    public class ChessMainGameClass
        : SimpleBoardGameClass<ChessPlayerItem, ChessSaveInfo, EnumColorChoice, int>, IMiscDataNM, IFinishStart
    {
        public ChessMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            ChessVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            ChessGameContainer container,
            GameBoardProcesses gameBoard
            ) : base(resolver, aggregator, basic, test, model, state,delay, command, container)
        {
            _model = model;
            _gameBoard = gameBoard;
            _gameContainer = container;
            CheckersChessDelegates.CanMove = (() => SaveRoot.GameStatus == EnumGameStatus.None);
            CheckersChessDelegates.MakeMoveAsync = PrivateMakeMoveAsync;
            BasicGameBoardDelegates.AfterPaintAsync = FinishAfterPaintingAsync;
        }
        private readonly ChessGameContainer _gameContainer;
        private async Task FinishAfterPaintingAsync()
        {
            _gameBoard.ClearBoard();
            _gameContainer.CurrentMoveList.Clear();
            SaveRoot.PreviousMove = new PreviousMove();
            SaveRoot.PossibleMove = new PreviousMove(); //try this too.
            await EndTurnAsync();
        }

        private async Task PrivateMakeMoveAsync(int space)
        {
            if (_gameBoard.IsValidMove(space) == false)
            {
                return;
            }
            if (BasicData.MultiPlayer)
            {
                await Network!.SendMoveAsync(GameBoardGraphicsCP.GetRealIndex(space, true));
            }
            await _gameBoard.MakeMoveAsync(space);
        }

        private readonly ChessVMData _model;
        private readonly GameBoardProcesses _gameBoard;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            _autoResume = true;
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
        private bool _autoResume;
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            _autoResume = false;
            //hopefully the erasing of colors is already handled.
            await FinishUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "possibletie":
                    await ProcessTieAsync();
                    return;
                case "undomove":
                    await _gameBoard.UndoAllMovesAsync();
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
                await _gameBoard.StartNewTurnAsync();
                return;
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon continue turn.  many board games require other things.
                if (SaveRoot.GameStatus == EnumGameStatus.PossibleTie)
                {
                    _model.Instructions = "Either Agree To Tie Or End Turn";
                }
                else if (SaveRoot.SpaceHighlighted == 0)
                {
                    _model.Instructions = "Make Move Or Initiate Tie";
                }
                else
                {
                    _model.Instructions = "Finish Move";
                }
            }
            return base.ContinueTurnAsync();
        }
        public override async Task MakeMoveAsync(int space)
        {
            await _gameBoard.MakeMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.PossibleTie = false);
            if (PlayerList.DidChooseColors() && SaveRoot.PossibleMove != null)
            {
                SaveRoot.PreviousMove = SaveRoot.PossibleMove; //otherwise, sets to null. wrong.
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
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
            SaveRoot!.GameStatus = EnumGameStatus.None;
            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            
        }
        
        async Task IFinishStart.FinishStartAsync()
        {
            if (_autoResume && PlayerList.DidChooseColors() == true)
            {
                await _gameBoard.ReloadSavedGameAsync(); //hopefully this simple.
            }
        }

        public async Task ProcessTieAsync()
        {
            SingleInfo!.PossibleTie = true;
            if (PlayerList.Any(items => items.PossibleTie == false))
            {
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                SaveRoot!.GameStatus = EnumGameStatus.PossibleTie;
                SingleInfo = PlayerList.GetWhoPlayer();
                PrepStartTurn();
                await ContinueTurnAsync();
                return;
            }
            await ShowTieAsync(); //hopefully this simple.
        }
        
    }
}
