using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using ConnectFourCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace ConnectFourCP.Logic
{
    [SingletonGame]
    public class ConnectFourMainGameClass
        : SimpleBoardGameClass<ConnectFourPlayerItem, ConnectFourSaveInfo, EnumColorChoice, int>
    {
        public ConnectFourMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            ConnectFourVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            ConnectFourGameContainer container
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, container)
        {
            _command = command;
        }

        private readonly CommandContainer _command;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            RepaintBoard();
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void RepaintBoard()
        {
            Aggregator.RepaintMessage(EnumRepaintCategories.FromSkiasharpboard);
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

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
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            //hopefully the erasing of colors is already handled.
            await FinishUpAsync(isBeginning);
        }

        public override async Task StartNewTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                PrepStartTurn(); //if you did not choose colors, no need to prepstart because something else will do it.
                //code to run but only if you actually chose color.

            }


            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon continue turn.  many board games require other things.

            }
            return base.ContinueTurnAsync();
        }
        public override async Task MakeMoveAsync(int column)
        {
            if (BasicData!.MultiPlayer == true && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                await Network!.SendMoveAsync(column);
            Vector topSpace = SaveRoot!.GameBoard[1, column].Vector;
            SpaceInfoCP tempSpace = new SpaceInfoCP();
            tempSpace.Color = SingleInfo!.Color.ToString().ToColor();
            tempSpace.Player = WhoTurn;
            tempSpace.Vector = new Vector(1, column);
            Vector BottomSpace = SaveRoot.GameBoard.GetBottomSpace(column);
            await Aggregator.AnimateMovePiecesAsync(topSpace, BottomSpace, tempSpace, true);
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
        public override async Task EndTurnAsync()
        {
            _command!.ManuelFinish = true; //could be here (?)
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
            SaveRoot!.GameBoard.Clear();
            RepaintBoard();
            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            await EndTurnAsync();
        }
    }
}
