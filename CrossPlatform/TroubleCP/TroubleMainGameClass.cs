using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace TroubleCP
{
    [SingletonGame]
    public class TroubleMainGameClass : BoardDiceGameClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>,
        TroublePlayerItem, TroubleSaveInfo, int>
    {
        public TroubleMainGameClass(IGamePackageResolver container) : base(container) { }
        internal TroubleViewModel? ThisMod;
        public override async Task ContinueTurnAsync()
        {
            if (ThisTest!.DoubleCheck)
            {
                ThisTest.DoubleCheck = false;
                await ThisMod!.GameBoard1!.GetValidMovesAsync(); //so i can see what is wrong.
                return;
            }
            await base.ContinueTurnAsync();
        }
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<TroubleViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            ThisMod!.GameBoard1!.LoadSavedGame(); //i think
            BoardGameSaved(); //i think.
            ThisMod.ThisCup!.CanShowDice = SaveRoot!.DiceNumber > 0;
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
            ThisMod!.GameBoard1!.ClearBoard();
            await EndTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (DidChooseColors == true)
            {
                ThisMod!.GameBoard1!.StartTurn(); //i think
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task MakeMoveAsync(int space)
        {
            await ThisMod!.GameBoard1!.MakeMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            ThisMod!.CommandContainer!.ManuelFinish = true;
            await StartNewTurnAsync();
        }
        public override async Task AfterRollingAsync()
        {
            SaveRoot!.DiceNumber = ThisMod!.ThisCup!.ValueOfOnlyDice; //i think.
            await ThisMod.GameBoard1!.GetValidMovesAsync();
        }
    }
}