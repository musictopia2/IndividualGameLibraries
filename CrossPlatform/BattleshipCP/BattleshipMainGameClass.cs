using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MiscProcesses;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace BattleshipCP
{
    [SingletonGame]
    public class BattleshipMainGameClass : BasicGameClass<BattleshipPlayerItem, BattleshipSaveInfo>, IMiscDataNM, IMoveNM
    {
        internal GameBoardCP? GameBoard1; //needed for view model for human waiting.
        internal ShipControlCP? Ships;
        public Vector CurrentSpace { get; set; }
        public BattleshipComputerAI? AI;
        public EnumStatusList StatusOfGame { get; set; } //has to be part of game class.
        public BattleshipMainGameClass(IGamePackageResolver container) : base(container) { }
        private BattleshipViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<BattleshipViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            if (ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Only multiplayer should have received this since no autoresume");
            if (ThisData.Client == false)
                throw new BasicBlankException("The host should never get this");
            LoadControls();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            GameBoard1 = Resolve<GameBoardCP>();
            Ships = Resolve<ShipControlCP>();
            if (ThisData!.MultiPlayer == false)
                AI = Resolve<BattleshipComputerAI>();
            StatusOfGame = EnumStatusList.PlacingShips;
            Ships.ClearBoard();
            GameBoard1.ClearBoard();
            _thisMod!.ShipDirectionsVisible = true; //hopefully this is it (?)
            IsLoaded = true; //i think needs to be here.
        }
        public async override Task ContinueTurnAsync()
        {
            if (StatusOfGame == EnumStatusList.PlacingShips)
            {
                _thisMod!.NormalTurn = "None";
                _thisMod.Status = "Please place your ships by clicking on the ship and cliking on the grid where you want to place them.";
                _thisMod.CommandContainer!.ManuelFinish = false;
                _thisMod.CommandContainer.IsExecuting = false; //at this point, show its done so you can do what you need to do in order to choose ships.
                return;
            }
            await base.ContinueTurnAsync();
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public async Task MakeMoveAsync(Vector Space, EnumWhatHit HitType)
        {
            if (HitType == EnumWhatHit.None)
                throw new BasicBlankException("Must be hit or miss");
            bool IsSelf;
            IsSelf = SingleInfo!.PlayerCategory == EnumPlayerCategory.Self;
            if (IsSelf == true)
            {
                GameBoard1!.MarkField(Space, HitType);
                if (HitType == EnumWhatHit.Hit && GameBoard1.HumanWon() == true)
                {
                    await ShowWinAsync();
                    return;
                }
            }
            else if (HitType == EnumWhatHit.Hit && Ships!.HasLost() == true)
            {
                await ShowWinAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        private async Task ComputerPlaceShipsAsync()
        {
            await AI!.ComputerPlaceShipsAsync();
            await StartGameAsync();
        }
        public async Task HumanFinishedPlacingShipsAsync()
        {
            _thisMod!.Status = "Waiting for other player to place their ships";
            _thisMod.ShipDirectionsVisible = false;
            _thisMod.CommandContainer!.ManuelFinish = true; //now has to manually be done.
            if (ThisData!.MultiPlayer == true)
            {
                await ThisNet!.SendAllAsync("finishedships");
                ThisCheck!.IsEnabled = true;
                return; //waiting for other player
            }
            await ComputerPlaceShipsAsync();
        }
        protected async override Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.3);
            Vector space = AI!.ComputerMove();
            FieldInfoCP ThisField = GameBoard1!.ComputerList![space];
            if (Ships!.HasHit(space))
            {
                ThisField.Hit = EnumWhatHit.Hit;
                AI.MarkHit(space);
                await MakeMoveAsync(space, EnumWhatHit.Hit);
                return;
            }
            ThisField.Hit = EnumWhatHit.Miss;
            await MakeMoveAsync(space, EnumWhatHit.Miss);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "finishedships":
                    await StartGameAsync();
                    break;
                case "hit":
                    await MakeMoveAsync(CurrentSpace, EnumWhatHit.Hit);
                    break;
                case "miss":
                    await MakeMoveAsync(CurrentSpace, EnumWhatHit.Miss);
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        async Task IMoveNM.MoveReceivedAsync(string data)
        {
            Vector space = await js.DeserializeObjectAsync<Vector>(data);
            bool hasHit = Ships!.HasHit(space);
            if (hasHit == true)
            {
                await ThisNet!.SendAllAsync("hit");
                await MakeMoveAsync(space, EnumWhatHit.Hit);
            }
            else
            {
                await ThisNet!.SendAllAsync("miss");
                await MakeMoveAsync(space, EnumWhatHit.Miss);
            }
        }
        public async Task StartGameAsync()
        {
            StatusOfGame = EnumStatusList.InGame;
            if (ThisData!.MultiPlayer == false)
                AI!.StartNewGame();
            GameBoard1!.StartGame(); //i think.
            if (GameBoard1.HumanList.Any(Items => Items.ShipNumber > 0) == false)
                throw new BasicBlankException("Human ships was never marked.  Rethink");
            await StartNewTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            CurrentSpace = new Vector();
            PrepStartTurn(); //i think
            await ContinueTurnAsync(); //i think
        }
    }
}