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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommonInterfaces;
using RollEmCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;

namespace RollEmCP.Logic
{
    [SingletonGame]
    public class RollEmMainGameClass : DiceGameClass<SimpleDice, RollEmPlayerItem, RollEmSaveInfo>, IMiscDataNM, IMoveNM
    {
        

        private readonly RollEmVMData _model;
        private readonly RollEmGameContainer _gameContainer;
        private readonly StandardRollProcesses<SimpleDice, RollEmPlayerItem> _roller;
        private readonly GameBoardProcesses _gameBoard;

        public RollEmMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            RollEmVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            RollEmGameContainer gameContainer,
            StandardRollProcesses<SimpleDice, RollEmPlayerItem> roller,
            GameBoardProcesses gameBoard
            
            ) : 
            base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, roller)
        {
            _model = currentMod; //if not needed, take this out and the _model variable.
            _gameContainer = gameContainer;
            _roller = roller;
            _gameBoard = gameBoard;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            SaveRoot!.LoadMod(_model);
            _gameBoard.LoadSavedGame();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadMod();
            GameBoardGraphicsCP.CreateNumberList(_gameContainer);
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (SaveRoot!.GameStatus == EnumStatusList.NeedRoll)
            {
                await _roller.RollDiceAsync();
                return;
            }
            var thisList = ComputerAI.NumberList(_gameBoard);
            if (thisList.Count == 0)
            {
                if (BasicData!.MultiPlayer == true)
                {
                    await Network!.SendEndTurnAsync();
                }
                await EndTurnAsync();
                return;
            }
            await thisList.ForEachAsync(async thisItem =>
            {
                await MakeMoveAsync(thisItem);
            });
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SetUpDice();
            SaveRoot!.LoadMod(_model);
            SaveRoot.Round = 1;
            SaveRoot.GameStatus = EnumStatusList.NeedRoll;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.ScoreGame = 0;
                thisPlayer.ScoreRound = 0;
            });
            await FinishUpAsync(isBeginning);
        }
        internal async Task MakeMoveAsync(int Space)
        {
            if (SingleInfo!.CanSendMessage(BasicData!) == true)
            {
                await Network!.SendMoveAsync(Space);
            }
            await _gameBoard.MakeMoveAsync(Space);
            bool rets = _gameBoard.IsMoveFinished();
            if (rets == false)
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                {
                    if (BasicData!.MultiPlayer == false)
                    {
                        return; //because somehow more moves being made by computer.
                    }
                    if (BasicData.Client == true)
                    {
                        Check!.IsEnabled = true; //has to wait for next move being sent from host.
                        return; //because host will control computer. //i may have to wait for messages (?)
                    }
                }
                await ContinueTurnAsync();
                return;
            }
            _gameBoard.FinishMove();
            SaveRoot!.GameStatus = EnumStatusList.NeedRoll;
            await ContinueTurnAsync();
        }
        public override Task PopulateSaveRootAsync()
        {
            _gameBoard.SaveGame();
            return base.PopulateSaveRootAsync();
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderBy(x => x.ScoreGame).First();
            await ShowWinAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "clearrecent":
                    //figure out what to do about this.
                    _gameBoard.ClearRecent(true);
                    await ContinueTurnAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            _gameBoard.ClearBoard(); //forgot this.
            this.ShowTurn();
            SaveRoot!.GameStatus = EnumStatusList.NeedRoll;
            await ContinueTurnAsync();
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            SaveRoot!.GameStatus = EnumStatusList.ChooseNumbers;
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            int score = _gameBoard.CalculateScore;
            SingleInfo!.ScoreRound = score;
            SingleInfo.ScoreGame += score;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            if (WhoTurn == WhoStarts)
            {
                if (SaveRoot!.Round == 5 || Test!.ImmediatelyEndGame)
                {
                    _gameBoard.ClearBoard();
                    await GameOverAsync();
                    return;
                }
                SaveRoot.Round++;
            }
            await StartNewTurnAsync();
        }
        protected override bool ShowDiceUponAutoSave => SaveRoot!.GameStatus != EnumStatusList.NeedRoll;
        async Task IMoveNM.MoveReceivedAsync(string data)
        {
            await MakeMoveAsync(int.Parse(data));
        }
    }
}