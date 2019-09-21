using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.NetworkingClasses.Extensions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace TicTacToeCP
{
    [SingletonGame]
    public class TicTacToeMainGameClass : BasicGameClass<TicTacToePlayerItem, TicTacToeSaveInfo>, IMoveNM
    {
        public TicTacToeMainGameClass(IGamePackageResolver container) : base(container) { }
        private TicTacToeViewModel? _thisMod;
        private TicTacToeGraphicsCP? ThisGraphics;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<TicTacToeViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            RepaintBoard();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            ThisGraphics = MainContainer.Resolve<TicTacToeGraphicsCP>(); //i think.
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList.First().Piece = EnumSpaceType.X;
            PlayerList.Last().Piece = EnumSpaceType.O;
            SaveRoot!.GameBoard.Clear(); //i think here too.
            WinInfo thisWin = SaveRoot.GameBoard.GetWin();
            ThisGraphics!.ThisWin = thisWin;
            RepaintBoard();
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public async Task MoveReceivedAsync(string data)
        {
            SpaceInfoCP tempMove = await js.DeserializeObjectAsync<SpaceInfoCP>(data);
            SpaceInfoCP realMove = SaveRoot!.GameBoard[tempMove.Vector]; //i think
            await MakeMoveAsync(realMove);
        }
        public async Task MakeMoveAsync(SpaceInfoCP thisSpace)
        {
            SingleInfo = SaveRoot!.PlayerList.GetWhoPlayer();
            thisSpace.Status = SingleInfo.Piece;
            WinInfo thisWin = SaveRoot.GameBoard.GetWin();
            ThisGraphics!.ThisWin = thisWin;
            if (ThisData!.MultiPlayer == true && SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                await ThisNet!.SendMoveAsync(thisSpace);
            RepaintBoard();
            if (thisWin.WinList.Count > 0)
            {
                RepaintBoard();
                await ShowWinAsync();
                return;
            }
            if (thisWin.IsDraw == true)
            {
                await ShowTieAsync();
                return;
            }
            await EndTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            _thisMod!.CommandContainer!.ManuelFinish = true;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        private void RepaintBoard()
        {
            ThisE.RepaintMessage(EnumRepaintCategories.fromskiasharpboard);
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
    }
}