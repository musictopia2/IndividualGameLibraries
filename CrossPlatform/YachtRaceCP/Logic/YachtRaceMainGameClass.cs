using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using YachtRaceCP.Data;

namespace YachtRaceCP.Logic
{
    [SingletonGame]
    public class YachtRaceMainGameClass : DiceGameClass<SimpleDice, YachtRacePlayerItem, YachtRaceSaveInfo>, IMiscDataNM
    {


        private readonly YachtRaceVMData _model;
        private readonly CommandContainer _command;

        internal bool HasRolled { get; set; }
        public YachtRaceMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            YachtRaceVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            BasicGameContainer<YachtRacePlayerItem, YachtRaceSaveInfo> gameContainer,
            StandardRollProcesses<SimpleDice, YachtRacePlayerItem> roller) :
            base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, roller)
        {
            _model = currentMod; //if not needed, take this out and the _model variable.
            _command = command;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            PrepTurn();
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadMod();
            IsLoaded = true; //i think needs to be here.
        }
        private void PrepTurn()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            ProtectedStartTurn(); //i think.
            HasRolled = false;
            this.ShowTurn();
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.GameTimer.StartTimer();
                _model.GameTimer.PauseTimer();
            }
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
            SetUpDice();
            PlayerList!.ForEach(thisPlayer => thisPlayer.Time = 0);
            PrepTurn();
            await FinishUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                //put in cases here.
                case "fivekind":
                    await ProcessFiveKindAsync(float.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            PrepTurn();

            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            HasRolled = true;
            //anything else that needs to happen after rolling happens here.
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            _model.Cup!.UnholdDice();
            _command.ManuelFinish = true;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            if (WhoTurn == WhoStarts)
            {
                SingleInfo = PlayerList.OrderBy(items => items.Time).First();
                await ShowWinAsync();
                return;
            }
            await StartNewTurnAsync();
        }
        internal async Task ProcessFiveKindAsync(float howLong)
        {
            SingleInfo!.Time = howLong;
            HasRolled = false;
            await EndTurnAsync();
        }
        internal bool HasYahtzee()
        {
            if (Test!.AllowAnyMove)
            {
                return true;
            }
            var count = Cup!.DiceList.DistinctCount(items => items.Value);
            return count == 1; //hopefully this works.
        }
    }
}
