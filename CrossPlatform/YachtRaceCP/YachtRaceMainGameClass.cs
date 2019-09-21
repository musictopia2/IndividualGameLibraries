using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace YachtRaceCP
{
    [SingletonGame]
    public class YachtRaceMainGameClass : DiceGameClass<SimpleDice, YachtRacePlayerItem, YachtRaceSaveInfo>, IMiscDataNM
    {
        public YachtRaceMainGameClass(IGamePackageResolver container) : base(container) { }
        private YachtRaceViewModel? _thisMod;
        internal bool HasRolled;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<YachtRaceViewModel>();
        }
        protected override bool ShowDiceUponAutoSave => false;
        private void PrepTurn()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            ProtectedStartTurn(); //i think.
            HasRolled = false;
            if (SingleInfo.PlayerCategory == BasicGameFramework.MultiplayerClasses.BasicPlayerClasses.EnumPlayerCategory.Self)
            {
                _thisMod!.GameTimer!.StartTimer();
                _thisMod.GameTimer.PauseTimer();
            }
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            PrepTurn();
            return Task.CompletedTask;
        }

        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            SetUpDice();
            PlayerList!.ForEach(thisPlayer => thisPlayer.Time = 0);
            PrepTurn();
            await ThisLoader!.FinishUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //leave warning for now.
            {
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
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            _thisMod!.ThisCup!.UnholdDice();
            _thisMod!.CommandContainer!.ManuelFinish = true;
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
            var count = ThisCup!.DiceList.DistinctCount(items => items.Value);
            return count == 1; //hopefully this works.
        }
    }
}