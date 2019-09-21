using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RollEmCP
{
    [SingletonGame]
    public class RollEmMainGameClass : DiceGameClass<SimpleDice, RollEmPlayerItem, RollEmSaveInfo>, IMiscDataNM, IMoveNM
    {
        public RollEmMainGameClass(IGamePackageResolver container) : base(container) { }
        public RollEmViewModel? ThisMod;
        internal Dictionary<int, NumberInfo>? NumberList;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<RollEmViewModel>();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            SaveRoot!.LoadMod(ThisMod!);
            ThisMod!.GameBoard1!.LoadSavedGame();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            GameBoardGraphicsCP.CreateNumberList(this);
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (SaveRoot!.GameStatus == EnumStatusList.NeedRoll)
            {
                await ThisRoll!.RollDiceAsync();
                return;
            }
            var thisList = ComputerAI.NumberList(ThisMod!.GameBoard1!);
            if (thisList.Count == 0)
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendEndTurnAsync();
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
            SetUpDice();
            SaveRoot!.LoadMod(ThisMod!);
            SaveRoot.Round = 1;
            SaveRoot.GameStatus = EnumStatusList.NeedRoll;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.ScoreGame = 0;
                thisPlayer.ScoreRound = 0;
            });
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        internal async Task MakeMoveAsync(int Space)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendMoveAsync(Space);
            await ThisMod!.GameBoard1!.MakeMoveAsync(Space);
            bool rets = ThisMod.GameBoard1.IsMoveFinished();
            if (rets == false)
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                {
                    if (ThisData!.MultiPlayer == false)
                        return; //because somehow more moves being made by computer.
                    if (ThisData.Client == true)
                    {
                        ThisCheck!.IsEnabled = true; //has to wait for next move being sent from host.
                        return; //because host will control computer. //i may have to wait for messages (?)
                    }
                }
                await ContinueTurnAsync();
                return;
            }
            ThisMod.GameBoard1.FinishMove();
            SaveRoot!.GameStatus = EnumStatusList.NeedRoll;
            await ContinueTurnAsync();
        }
        public override Task PopulateSaveRootAsync()
        {
            ThisMod!.GameBoard1!.SaveGame();
            return base.PopulateSaveRootAsync();
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderBy(Items => Items.ScoreGame).First();
            await ShowWinAsync();
        }
        public void RepaintBoard()
        {
            ThisE.RepaintBoard(); //hopefully this simple.
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //leave warning for now.
            {
                case "clearrecent":
                    //figure out what to do about this.
                    ThisMod!.GameBoard1!.ClearRecent(true);
                    await ContinueTurnAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            ThisMod!.GameBoard1!.ClearBoard(); //forgot this.
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
            int score = ThisMod!.GameBoard1!.CalculateScore;
            SingleInfo!.ScoreRound = score;
            SingleInfo.ScoreGame += score;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            if (WhoTurn == WhoStarts)
            {
                if (SaveRoot!.Round == 5)
                {
                    ThisMod.GameBoard1.ClearBoard();
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