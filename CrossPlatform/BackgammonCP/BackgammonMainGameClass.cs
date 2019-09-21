using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BackgammonCP
{
    [SingletonGame]
    public class BackgammonMainGameClass : BoardDiceGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>,
        BackgammonPlayerItem, BackgammonSaveInfo, int>, IMiscDataNM, IFinishStart
    {
        public BackgammonMainGameClass(IGamePackageResolver container) : base(container) { }
        internal GlobalClass? ThisGlobal;
        internal BackgammonViewModel? ThisMod;
        private bool _autoResume;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<BackgammonViewModel>();
            ThisGlobal = MainContainer.Resolve<GlobalClass>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            _autoResume = true;
            ThisMod!.ThisCup!.ShowDiceListAlways = true;
            ThisMod.ThisCup.Visible = true;
            ThisMod.ThisCup.CanShowDice = true;
            SaveRoot!.LoadMod(ThisMod);
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            ThisMod!.GameBoard1!.LoadBoard();
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            EraseColors();
            SetUpDice();
            _autoResume = false;
            SaveRoot!.LoadMod(ThisMod!);
            SaveRoot.ImmediatelyStartTurn = true; //maybe this is needed now (?)
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public override async Task ContinueTurnAsync()
        {
            if (DidChooseColors == false)
            {
                ThisMod!.LastStatus = "";
                await base.ContinueTurnAsync();  //hopefully the instructions are already filled out
                return;
            }
            //ThisMod!.GameBoard1!.StartPaint();
            if (SaveRoot!.GameStatus == EnumGameStatus.EndingTurn)
            {
                ThisMod!.LastStatus = "Finished Moves";
                SaveRoot.Instructions = "Either End Turn Or Undo All Moves";
            }
            if (SaveRoot.MovesMade == 4)
                ThisMod!.LastStatus = "Made Moves With Doubles.";
            else if (SaveRoot.MovesMade > 0 && SaveRoot.SpaceHighlighted == -1)
                ThisMod!.LastStatus = "Made At Least One Move";
            else if (SaveRoot.SpaceHighlighted > -1)
            {
                if (SaveRoot.GameStatus == EnumGameStatus.EndingTurn)
                    throw new BasicBlankException("It can't be ending turn if a space is highlighted");
                ThisMod!.LastStatus = "";
                SaveRoot.Instructions = "Either Unhighlight space or finish move";
            }
            else
            {
                ThisMod!.LastStatus = "";
                SaveRoot.Instructions = "Make Moves";
            }
            await base.ContinueTurnAsync();
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            SaveRoot!.GameStatus = EnumGameStatus.MakingMoves; //i think.  hopefully this simple.
            ThisE.RepaintBoard(); //hopefully this simple.
            await Task.Delay(300); //so it can populate the board properly.
            ThisMod!.GameBoard1!.ClearBoard();
            await EndTurnAsync(); //decided to do an endturn this time.
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "undomove":
                    await ThisMod!.GameBoard1!.UndoAllMovesAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (DidChooseColors == false)
            {
                await ContinueTurnAsync();
                return; //hopefully this simple.
            }
            await StartRollingAsync();
        }
        private async Task StartRollingAsync()
        {
            ThisCup!.DiceList.ForEach(thisDice => thisDice.Visible = true);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                ThisCheck!.IsEnabled = true;
                return; //because waiting for other player to roll dice.
            }
            await ThisRoll!.RollDiceAsync(); //hopefully this simple.
        }
        public override async Task MakeMoveAsync(int space)
        {
            await ThisMod!.GameBoard1!.MakeMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            ThisMod!.CommandContainer!.ManuelFinish = true;
            SaveRoot!.ComputerSpaceTo = -1;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public override async Task AfterRollingAsync()
        {
            await ThisMod!.GameBoard1!.StartNewTurnAsync(); //hopefully this works.
            await ContinueTurnAsync();
        }
        public void DiceVisibleProcesses()
        {
            var thisList = ThisCup!.DiceList;
            if (SaveRoot!.NumberUsed == 0 && SaveRoot.MovesMade == 0 && SaveRoot.MadeAtLeastOneMove == false)
            {
                thisList.ForEach(thisDice => thisDice.Visible = true);
                return;
            }
            if (ThisGlobal!.HadDoubles())
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
            if (SaveRoot.NumberUsed == ThisGlobal.FirstDiceValue)
                thisList.First().Visible = false;
            else if (SaveRoot.NumberUsed == ThisGlobal.SecondDiceValue)
                thisList.Last().Visible = false;
            else
                throw new BasicBlankException("Not Sure");
        }
        async Task IFinishStart.FinishStartAsync()
        {
            if (_autoResume && DidChooseColors == true)
                await ThisMod!.GameBoard1!.ReloadSavedGameAsync(); //hopefully this simple.
        }
        protected override async Task ComputerTurnAsync()
        {

            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            if (SaveRoot!.GameStatus == EnumGameStatus.EndingTurn)
            {
                await EndTurnAsync();
                return;
            }
            if (SaveRoot.SpaceHighlighted == -1)
            {
                var (spaceFrom, _) = ComputerAI.GetComputerMove(this);
                await ThisMod!.GameBoard1!.MakeMoveAsync(spaceFrom);
                return;
            }
            if (SaveRoot.ComputerSpaceTo == -1)
                throw new BasicBlankException("The SpaceTo cannot be -1 because no space was highlighted. There should have been populated");
            await ThisMod!.GameBoard1!.MakeMoveAsync(SaveRoot.ComputerSpaceTo);
        }
    }
}