using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MiscProcesses;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConnectFourCP
{
    [SingletonGame]
    public class ConnectFourMainGameClass : SimpleBoardGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>,
        ConnectFourPlayerItem, ConnectFourSaveInfo, int>
    {
        public ConnectFourMainGameClass(IGamePackageResolver container) : base(container) { }
        private ConnectFourViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<ConnectFourViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            RepaintBoard();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            EraseColors();
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            SaveRoot!.GameBoard.Clear();
            RepaintBoard();
            await EndTurnAsync(); //hopefully this simple.
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task MakeMoveAsync(int column)
        {
            if (ThisData!.MultiPlayer == true && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                await ThisNet!.SendMoveAsync(column);
            Vector topSpace = SaveRoot!.GameBoard[1, column].Vector;
            SpaceInfoCP tempSpace = new SpaceInfoCP();
            tempSpace.Color = SingleInfo!.Color.ToString().ToColor();
            tempSpace.Player = WhoTurn;
            tempSpace.Vector = new Vector(1, column);
            Vector BottomSpace = SaveRoot.GameBoard.GetBottomSpace(column);
            await ThisE.AnimateMovePiecesAsync(topSpace, BottomSpace, tempSpace, true);
            SaveRoot.GameBoard[BottomSpace].Color = tempSpace.Color;
            SaveRoot.GameBoard[BottomSpace].Player = WhoTurn;
            RepaintBoard();
            WinInfo thisWin = SaveRoot.GameBoard.GetWin();
            if (thisWin.IsDraw == true)
            {
                await ShowTieAsync();
                return;
            }
            if (thisWin.WinList.Count > 0)
            {
                RepaintBoard();
                await ShowWinAsync();
                return;
            }
            await EndTurnAsync();
        }
        private void RepaintBoard()
        {
            ThisE.RepaintMessage(EnumRepaintCategories.fromskiasharpboard);
        }
        public override async Task EndTurnAsync()
        {
            _thisMod!.CommandContainer!.ManuelFinish = true;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
    }
}