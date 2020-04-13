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
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SequenceDiceCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace SequenceDiceCP.Logic
{
    [SingletonGame]
    public class SequenceDiceMainGameClass
        : BoardDiceGameClass<SequenceDicePlayerItem, SequenceDiceSaveInfo, EnumColorChoice, SpaceInfoCP>
    {
        public SequenceDiceMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            SequenceDiceVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            SequenceDiceGameContainer container,
            StandardRollProcesses<SimpleDice, SequenceDicePlayerItem> roller
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller)
        {
            _model = model;
        }

        private readonly SequenceDiceVMData _model;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            if (SaveRoot.GameStatus == EnumGameStatusList.RollDice)
            {
                _model.Cup!.CanShowDice = false;
            }
            else
            {
                _model.Cup!.CanShowDice = true;
            }
            SaveRoot.LoadMod(_model); //we usually need this.
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            SaveRoot.GameBoard.LoadBoard(PlayerList, Test!, _model);
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
            if (WhoTurn == 0)
            {
                throw new BasicBlankException("WhoTurn cannot be 0 at the start of the turn.");
            }
            if (PlayerList.DidChooseColors())
            {
                PrepStartTurn();
                SingleInfo = PlayerList!.GetWhoPlayer();
                SaveRoot!.GameStatus = EnumGameStatusList.RollDice;
                _model!.Cup!.CanShowDice = false;

            }


            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override void PrepStartTurn()
        {
            base.PrepStartTurn();
            SaveRoot!.GameBoard.StartTurn(WhoTurn); //looks like i do have to override the prepstartturn after all.
        }
        public override Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon continue turn.  many board games require other things.
                if (SaveRoot.GameStatus == EnumGameStatusList.RollDice)
                {
                    SaveRoot.Instructions = "Roll the dice";
                }
                else if (SaveRoot.GameStatus == EnumGameStatusList.MovePiece)
                {
                    if (Cup!.TotalDiceValue == 10)
                    {
                        SaveRoot.Instructions = "Remove an opponents piece from the board.";
                    }
                    else if (Cup.TotalDiceValue == 11)
                    {
                        SaveRoot.Instructions = "Put a piece on any open spaces on the board.  If there are none, then replace the opponent's piece with yours";
                    }
                    else
                    {
                        SaveRoot.Instructions = "Put a piece on the number corresponding to the dice roll.  If there are no open spaces, then replace the opponent's piece with yours";
                    }
                }
            }
            return base.ContinueTurnAsync();
        }
        public override async Task MakeMoveAsync(SpaceInfoCP space)
        {
            SpaceInfoCP newSpace = SaveRoot!.GameBoard[space.Vector];
            SaveRoot.GameBoard.MakeMove(newSpace, SingleInfo!);
            if (SaveRoot.GameBoard.HasWon() == true)
            {
                await ShowWinAsync();
                return;
            }
            int Totals = _model.Cup!.TotalDiceValue;
            if (Totals == 2 || Totals == 12)
            {
                SaveRoot.GameStatus = EnumGameStatusList.RollDice;
                await ContinueTurnAsync();
                return;
            }
            await EndTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
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

            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            SaveRoot.GameBoard.ClearBoard();
            await EndTurnAsync();
        }
        public override async Task AfterRollingAsync()
        {
            int totals = _model.Cup!.TotalDiceValue;
            if (SaveRoot!.GameBoard.HasValidMove() == false)
            {
                if (totals == 2 || totals == 12)
                {
                    SaveRoot.Instructions = "No moves possible.  Take another turn";
                    if (Test!.NoAnimations == false)
                    {
                        await Delay!.DelaySeconds(1);
                    }
                    _model.Cup.CanShowDice = false;
                    await ContinueTurnAsync();
                    return;
                }
                SaveRoot.Instructions = "No moves possible.  Ending turn";
                if (Test!.NoAnimations == false)
                {
                    await Delay!.DelaySeconds(1);
                }
                await EndTurnAsync();
                return;
            }
            SaveRoot.GameStatus = EnumGameStatusList.MovePiece;
            await ContinueTurnAsync();
        }
    }
}