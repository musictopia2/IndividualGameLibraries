using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.NetworkingClasses.Extensions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace DiceDominosCP
{
    [SingletonGame]
    public class DiceDominosMainGameClass : DiceGameClass<SimpleDice, DiceDominosPlayerItem, DiceDominosSaveInfo>, IMoveNM
    {
        public DiceDominosMainGameClass(IGamePackageResolver container) : base(container) { }
        private DiceDominosViewModel? _thisMod;
        public GameBoardCP? GameBoard1;
        private DiceDominosComputerAI? _ai;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<DiceDominosViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            GameBoard1!.LoadSavedGame(SaveRoot!.BoardDice!);
            _thisMod!.DominosList!.ClearObjects(); //has to clear objects first.
            _thisMod.DominosList.OrderedObjects(); //i think this should be fine.
            AfterRestoreDice(); //i think
            if (SaveRoot.DidHold == true || SaveRoot.HasRolled == true)
                _thisMod!.ThisCup!.CanShowDice = true;
            else
                _thisMod!.ThisCup!.CanShowDice = false;
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            GameBoard1 = MainContainer.Resolve<GameBoardCP>(); //has to be here because it relies on the main game class.
            GameBoard1.Text = "Dominos";
            GameBoard1.SendEnableProcesses(_thisMod!, () =>
            {
                return SaveRoot!.HasRolled;
            });
            LoadUpDice();
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(items =>
            {
                items.DominosWon = 0;
            });
            SetUpDice();
            SaveRoot!.ImmediatelyStartTurn = true;
            _thisMod!.DominosList!.ClearObjects();
            _thisMod.DominosList.ShuffleObjects(); //i think.
            GameBoard1!.ClearPieces(); //i think
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.BoardDice = GameBoard1!.ObjectList.ToRegularDeckDict();
            return Task.CompletedTask;
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            _thisMod!.ThisCup!.UnholdDice();
            SaveRoot!.HasRolled = false;
            SaveRoot.DidHold = false;
            _thisMod.ThisCup.CanShowDice = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override Task AfterHoldUnholdDiceAsync()
        {
            SaveRoot!.DidHold = true;
            SaveRoot.HasRolled = false; //because you can roll one more time.
            return base.AfterHoldUnholdDiceAsync();
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            SaveRoot!.HasRolled = true;
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public async Task MakeMoveAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendMoveAsync(deck);
            SingleInfo!.DominosWon++;
            GameBoard1!.MakeMove(deck);
            if (IsGameOver(SingleInfo.DominosWon) == true)
            {
                await ShowWinAsync();
                return;
            }
            await EndTurnAsync();
        }
        public async Task MoveReceivedAsync(string Data)
        {
            await MakeMoveAsync(int.Parse(Data));
        }
        protected override async Task ComputerTurnAsync()
        {
            if (_ai == null)
                _ai = MainContainer.Resolve<DiceDominosComputerAI>();
            if (SaveRoot!.HasRolled == false)
            {
                await ThisRoll!.RollDiceAsync();
                return;
            }
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            int nums = _ai.Move();
            if (nums == 0 && SaveRoot.DidHold == true)
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendEndTurnAsync();
                await EndTurnAsync();
                return;
            }
            if (nums == 0 && SaveRoot.DidHold == false)
            {
                int ThisNum = _ai.DominoToHold();
                await HoldUnholdDiceAsync(ThisNum); //i think
                return;
            }
            await MakeMoveAsync(nums);
        }
        private bool IsGameOver(int score)
        {
            if (score == 13 && PlayerList.Count() == 2)
                return true;
            if (score == 9 && PlayerList.Count() == 3)
                return true;
            if (score == 7 && PlayerList.Count() == 4)
                return true;
            if (score == 6 && PlayerList.Count() == 5)
                return true;
            if (score == 5 && PlayerList.Count() == 6)
                return true;
            return false;
        }
    }
}