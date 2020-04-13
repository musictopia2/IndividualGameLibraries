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
using BackgammonCP.Data;
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
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;

namespace BackgammonCP.Logic
{
    [SingletonGame]
    public class BackgammonMainGameClass
        : BoardDiceGameClass<BackgammonPlayerItem, BackgammonSaveInfo, EnumColorChoice, int>, IMiscDataNM
    {
        public BackgammonMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            BackgammonVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            BackgammonGameContainer container,
            StandardRollProcesses<SimpleDice, BackgammonPlayerItem> roller,
            GameBoardProcesses gameBoard
            ) : base(resolver, aggregator, basic, test, model, state,delay, command, container, roller)
        {
            _model = model;
            _command = command;
            _gameBoard = gameBoard;
            _gameContainer = container;
            _gameContainer.DiceVisibleProcesses = DiceVisibleProcesses;
        }
        //no need for finish because no autoresume.

        private readonly BackgammonVMData _model;
        private readonly CommandContainer _command;
        private readonly GameBoardProcesses _gameBoard;
        private readonly BackgammonGameContainer _gameContainer;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            _model.Cup!.ShowDiceListAlways = true;
            _model.Cup.Visible = true;
            _model.Cup.CanShowDice = true;
            SaveRoot.LoadMod(_model); //we usually need this.
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override Task ComputerTurnAsync()
        {
            throw new BasicBlankException("Computer should not go because it had too many problems");
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SetUpDice();
            //_autoResume = false;
            SaveRoot.LoadMod(_model); //we usually need this.
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            //hopefully the erasing of colors is already handled.
            await FinishUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
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
                Cup!.DiceList.ForEach(thisDice => thisDice.Visible = true);
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
                {
                    Check!.IsEnabled = true;
                    return; //because waiting for other player to roll dice.
                }
                await Roller!.RollDiceAsync(); //hopefully this simple.
                return;
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors() == false)
            {
                _model.LastStatus = "";
                await base.ContinueTurnAsync();
                return;
            }
            if (SaveRoot!.GameStatus == EnumGameStatus.EndingTurn)
            {
                _model.LastStatus = "Finished Moves";
                SaveRoot.Instructions = "Either End Turn Or Undo All Moves";
            }
            if (SaveRoot.MovesMade == 4)
            {
                _model.LastStatus = "Made Moves With Doubles.";
            }
            else if (SaveRoot.MovesMade > 0 && SaveRoot.SpaceHighlighted == -1)
            {
                _model.LastStatus = "Made At Least One Move";
            }
            else if (SaveRoot.SpaceHighlighted > -1)
            {
                if (SaveRoot.GameStatus == EnumGameStatus.EndingTurn)
                {
                    throw new BasicBlankException("It can't be ending turn if a space is highlighted");
                }
                _model.LastStatus = "";
                SaveRoot.Instructions = "Either Unhighlight space or finish move";
            }
            else
            {
                _model!.LastStatus = "";
                SaveRoot.Instructions = "Make Moves";
            }
            await base.ContinueTurnAsync();
        }
        public override async Task MakeMoveAsync(int space)
        {
            await _gameBoard.MakeMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            _command.ManuelFinish = true;
            //maybe not needed because no computer play.
            //SaveRoot!.ComputerSpaceTo = -1;
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
            SaveRoot!.GameStatus = EnumGameStatus.MakingMoves; //i think.  hopefully this simple.
            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.

            await Task.Delay(300); //hopefully enough time this time.
            _gameBoard.ClearBoard();
            
            await EndTurnAsync();
        }
        public override async Task AfterRollingAsync()
        {
            await _gameBoard.StartNewTurnAsync();
            //anything needed after rolling is here.
            await ContinueTurnAsync();
        }
        public void DiceVisibleProcesses()
        {
            var thisList = Cup!.DiceList;
            if (SaveRoot!.NumberUsed == 0 && SaveRoot.MovesMade == 0 && SaveRoot.MadeAtLeastOneMove == false)
            {
                thisList.ForEach(thisDice => thisDice.Visible = true);
                return;
            }
            if (_gameContainer!.HadDoubles())
            {
                if (SaveRoot.MovesMade < 2)
                {
                    thisList.ForEach(thisDice => thisDice.Visible = true);
                    return;
                }
                if (SaveRoot.MovesMade == 4)
                {
                    thisList.ForEach(thisDice => thisDice.Visible = false);
                    return;
                }
                thisList.First().Visible = false;
                return;
            }
            if (SaveRoot.MovesMade == 2)
            {
                thisList.ForEach(thisDice => thisDice.Visible = false);
                return;
            }
            if (SaveRoot.NumberUsed == _gameContainer.FirstDiceValue)
                thisList.First().Visible = false;
            else if (SaveRoot.NumberUsed == _gameContainer.SecondDiceValue)
                thisList.Last().Visible = false;
            else
                throw new BasicBlankException("Not Sure");
        }
        //async Task IFinishStart.FinishStartAsync()
        //{
        //    if (_autoResume && DidChooseColors == true)
        //        await ThisMod!.GameBoard1!.ReloadSavedGameAsync(); //hopefully this simple.
        //}
    }
}
