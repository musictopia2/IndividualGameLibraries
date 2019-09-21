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
using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
namespace SequenceDiceCP
{
    [SingletonGame]
    public class SequenceDiceMainGameClass : BoardDiceGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>,
        SequenceDicePlayerItem, SequenceDiceSaveInfo, SpaceInfoCP>
    {
        public SequenceDiceMainGameClass(IGamePackageResolver container) : base(container) { }

        private SequenceDiceViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
			base.Init();
            _thisMod = MainContainer.Resolve<SequenceDiceViewModel>();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            if (SaveRoot!.GameStatus == EnumGameStatusList.RollDice)
                _thisMod!.ThisCup!.CanShowDice = false;
            else
                _thisMod!.ThisCup!.CanShowDice = true;
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            SaveRoot!.GameBoard.LoadBoard(PlayerList!, _thisMod!, ThisTest!);
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
            EraseColors();
            SetUpDice();
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            SaveRoot!.GameBoard.ClearBoard();
            await EndTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            if (WhoTurn == 0)
                throw new BasicBlankException("WhoTurn cannot be 0 at the start of the turn.");
            await base.StartNewTurnAsync();
            if (DidChooseColors == true)
            {
                //actions because you start the turn and chose colors (if any).
                SingleInfo = PlayerList!.GetWhoPlayer();
                SaveRoot!.GameStatus = EnumGameStatusList.RollDice;
                _thisMod!.ThisCup!.CanShowDice = false;
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
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
            int Totals = _thisMod!.ThisCup!.TotalDiceValue;
            if (Totals == 2 || Totals == 12)
            {
                SaveRoot.GameStatus = EnumGameStatusList.RollDice;
                await ContinueTurnAsync();
                return;
            }
            await EndTurnAsync();
        }
        protected override void PrepStartTurn()
        {
            base.PrepStartTurn();
            SaveRoot!.GameBoard.StartTurn(WhoTurn); //looks like i do have to override the prepstartturn after all.
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public override async Task AfterRollingAsync()
        {
            int totals = _thisMod!.ThisCup!.TotalDiceValue;
            if (SaveRoot!.GameBoard.HasValidMove() == false)
            {
                if (totals == 2 || totals == 12)
                {
                    SaveRoot.Instructions = "No moves possible.  Take another turn";
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(1);
                    _thisMod.ThisCup.CanShowDice = false;
                    await ContinueTurnAsync();
                    return;
                }
                SaveRoot.Instructions = "No moves possible.  Ending turn";
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                await EndTurnAsync();
                return;
            }
            SaveRoot.GameStatus = EnumGameStatusList.MovePiece;
            await ContinueTurnAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (DidChooseColors == false)
                SaveRoot!.Instructions = "Choose a color";
            else if (SaveRoot!.GameStatus == EnumGameStatusList.RollDice)
                SaveRoot.Instructions = "Roll the dice";
            else if (SaveRoot.GameStatus == EnumGameStatusList.MovePiece)
            {
                if (ThisCup!.TotalDiceValue == 10)
                    SaveRoot.Instructions = "Remove an opponents piece from the board.";
                else if (ThisCup.TotalDiceValue == 11)
                    SaveRoot.Instructions = "Put a piece on any open spaces on the board.  If there are none, then replace the opponent's piece with yours";
                else
                    SaveRoot.Instructions = "Put a piece on the number corresponding to the dice roll.  If there are no open spaces, then replace the opponent's piece with yours";
            }
            await base.ContinueTurnAsync();
        }
    }
}