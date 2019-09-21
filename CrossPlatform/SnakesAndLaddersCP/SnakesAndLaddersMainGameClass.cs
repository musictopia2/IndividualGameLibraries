using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SnakesAndLaddersCP
{
    [SingletonGame]
    public class SnakesAndLaddersMainGameClass : BasicGameClass<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>
        , IStandardRoller<SimpleDice, SnakesAndLaddersPlayerItem>, IFinishStart, IMoveNM
    {
        public SnakesAndLaddersMainGameClass(IGamePackageResolver container) : base(container) { }
        private SnakesAndLaddersViewModel? _thisMod;
        internal StandardRollProcesses<SimpleDice, SnakesAndLaddersPlayerItem>? ThisRoll;
        internal GameBoardProcesses? GameBoard1;
        public DiceCup<SimpleDice> ThisCup => _thisMod!.ThisCup!;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<SnakesAndLaddersViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            _thisMod!.LoadCup(SaveRoot!, true);
            SaveRoot!.DiceList.MainContainer = MainContainer;
            if (SaveRoot.HasRolled == true)
                _thisMod.ThisCup!.CanShowDice = true;
            else
                _thisMod.ThisCup!.CanShowDice = false;
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            GameBoard1 = MainContainer.Resolve<GameBoardProcesses>();
            ThisRoll = MainContainer.Resolve<StandardRollProcesses<SimpleDice, SnakesAndLaddersPlayerItem>>();
            IsLoaded = true; //i think needs to be here.
        }
        public async Task MakeMoveAsync(int space)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendMoveAsync(space);
            await GameBoard1!.MakeMoveAsync(space);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelayMilli(800); //to see move made
            if (GameBoard1.IsGameOver() == true)
            {
                await ShowWinAsync();
                return;
            }
            await EndTurnAsync();
        }
        public async Task MoveReceivedAsync(string data)
        {
            await MakeMoveAsync(int.Parse(data));
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelayMilli(200);
            if (SaveRoot!.HasRolled == false)
            {
                await ThisRoll!.RollDiceAsync();
                return;
            }
            int NewSpace = SingleInfo!.SpaceNumber + _thisMod!.ThisCup!.ValueOfOnlyDice;
            await MakeMoveAsync(NewSpace);
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            if (IsLoaded == false)
            {
                LoadControls();
                _thisMod!.LoadCup(SaveRoot!, false);
                SaveRoot!.DiceList.MainContainer = MainContainer;
            }
            SaveRoot!.ImmediatelyStartTurn = true;
            PlayerList!.ForEach(thisPlayer => thisPlayer.SpaceNumber = 0);
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            SaveRoot!.HasRolled = false;
            _thisMod!.ThisCup!.CanShowDice = false;
            GameBoard1!.NewTurn(); //i think here.
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public async Task AfterRollingAsync()
        {
            SaveRoot!.HasRolled = true;
            if (GameBoard1!.HasValidMove() == false)
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    await _thisMod!.ShowGameMessageAsync("No moves available.  Therefore, the turn will end");
                await EndTurnAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        public Task AfterSelectUnselectDiceAsync()
        {
            throw new BasicBlankException("There is no select/unselect dice");
        }
        public Task FinishStartAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            GameBoard1!.NewTurn();
            return Task.CompletedTask;
        }
    }
}