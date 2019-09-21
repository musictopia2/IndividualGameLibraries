using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Collections.Generic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PassOutDiceGameCP
{
    [SingletonGame]
    public class PassOutDiceGameMainGameClass : BoardDiceGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>,
        PassOutDiceGamePlayerItem, PassOutDiceGameSaveInfo, int>
    {
        internal Dictionary<int, SpaceInfo>? SpaceList;
        public PassOutDiceGameMainGameClass(IGamePackageResolver container) : base(container) { }
        internal PassOutDiceGameViewModel? ThisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<PassOutDiceGameViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            ThisMod!.GameBoard1!.LoadSavedGame();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            GameBoardGraphicsCP.CreateSpaceList(this);
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            EraseColors();
            SetUpDice();
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public override Task PopulateSaveRootAsync()
        {
            if (DidChooseColors == true)
                ThisMod!.GameBoard1!.SaveGame();
            return base.PopulateSaveRootAsync();
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            ThisMod!.CommandContainer!.ManuelFinish = true;
            RepaintBoard(); //has to still repaint.  otherwise, a player can't see the board.
            ThisMod.GameBoard1!.ClearBoard();
            await EndTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (DidChooseColors == true)
            {
                SaveRoot!.DidRoll = false;
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task MakeMoveAsync(int space)
        {
            ThisMod!.GameBoard1!.MakeMove(space);
            int wons = ThisMod.GameBoard1.WhoWon;
            if (wons > 0)
            {
                SingleInfo = PlayerList![WhoTurn];
                await ShowWinAsync();
                return;
            }
            await EndTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            ThisMod!.CommandContainer!.ManuelFinish = true;
            await StartNewTurnAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.DidRoll == false && DidChooseColors == true)
            {
                if (ThisData!.MultiPlayer == true && SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                {
                    ThisCheck!.IsEnabled = true;
                    return;
                }
                SaveRoot.DidRoll = true; //otherwise, it will do forever.
                await ThisRoll!.RollDiceAsync();
                return;
            }
            await base.ContinueTurnAsync();
        }
        public void RepaintBoard()
        {
            ThisE.RepaintBoard(); //hopefully this simple.
        }
        public override async Task AfterRollingAsync()
        {
            SaveRoot!.DidRoll = true;
            await ContinueTurnAsync();
        }
        private CustomBasicList<int> PossibleMoves()
        {
            CustomBasicList<int> output = new CustomBasicList<int>();
            for (int i = 0; i < 21; i++)
            {
                int x = i + 1;
                if (ThisMod!.GameBoard1!.IsValidMove(x))
                    output.Add(x);
            }
            return output;
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            var thisCol = PossibleMoves();
            if (thisCol.Count == 0)
            {
                await EndTurnAsync();
                return;
            }
            await MakeMoveAsync(thisCol.GetRandomItem());
        }
    }
}