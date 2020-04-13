using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CheckersCP.Data;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace CheckersCP.Logic
{
    [SingletonGame]
    public class CheckersMainGameClass
        : SimpleBoardGameClass<CheckersPlayerItem, CheckersSaveInfo, EnumColorChoice, int>, IMiscDataNM, IFinishStart
    {
        public CheckersMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            CheckersVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            CheckersGameContainer container,
            GameBoardProcesses gameBoard
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, container)
        {
            _model = model;
            _container = container;
            _gameBoard = gameBoard;
            BasicGameBoardDelegates.AfterPaintAsync = FinishAfterPaintingAsync;
            CheckersChessDelegates.CanMove = CanMove;
            CheckersChessDelegates.MakeMoveAsync = PrivateMoveAsync;
        }
        private bool CanMove()
        {
            return SaveRoot.GameStatus == EnumGameStatus.None; //i think this simple this time.
        }
        private async Task PrivateMoveAsync(int space)
        {
            if (_gameBoard.IsValidMove(space) == false)
            {
                return;
            }
            if (BasicData.MultiPlayer)
            {
                await Network!.SendMoveAsync(GameBoardGraphicsCP.GetRealIndex(space, true));
            }
            _container.Command.ManuelFinish = true;
            await _gameBoard.MakeMoveAsync(space);
        }
        private readonly CheckersVMData _model;
        private readonly CheckersGameContainer _container;
        private readonly GameBoardProcesses _gameBoard;
        private bool _autoResume;
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
                //put in cases here.
                case "possibletie":
                    await ProcessTieAsync();
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
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            //if anything else is needed, do here.
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon ending turn.  many board games require other things. only do if the player actually chose colors.

            }
            await StartNewTurnAsync();
        }
        async Task IFinishStart.FinishStartAsync()
        {
            //await UIPlatform.ShowMessageAsync("Trying to finish");
            if (_autoResume && PlayerList.DidChooseColors() == true)
            {

                await _gameBoard.ReloadSavedGameAsync(); //hopefully this simple.
            }
        }
        public override async Task AfterChoosingColorsAsync()
        {
            //anything else that is needed after they finished choosing colors.
            SaveRoot!.GameStatus = EnumGameStatus.None;

            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data
            //hopefully the part below will run once it can run.
        }

        private async Task FinishAfterPaintingAsync()
        {
            _gameBoard.ClearBoard();
            await EndTurnAsync();
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