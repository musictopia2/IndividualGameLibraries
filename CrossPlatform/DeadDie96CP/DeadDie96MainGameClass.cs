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
using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Dice;
namespace DeadDie96CP
{
    [SingletonGame]
    public class DeadDie96MainGameClass : DiceGameClass<TenSidedDice, DeadDie96PlayerItem, DeadDie96SaveInfo>
    {
        public DeadDie96MainGameClass(IGamePackageResolver container) : base(container) { }
		private DeadDie96ViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
			base.Init();
            _thisMod = MainContainer.Resolve<DeadDie96ViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            //anything else needed is here.
            AfterRestoreDice(); //i think
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelayMilli(200);
            await ThisRoll!.RollDiceAsync(); //this is it for computer turn.
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderBy(Items => Items.TotalScore).Take(1).Single();
            await ShowWinAsync();
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            SetUpDice();
            PlayerList!.ForEach(Items =>
            {
                Items.TotalScore = 0;
                Items.CurrentScore = 0;
            });
            SaveRoot!.ImmediatelyStartTurn = true;
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            _thisMod!.ThisCup!.HowManyDice = 5;
            _thisMod.ThisCup.HideDice();
            _thisMod.ThisCup.CanShowDice = false;
            ProtectedStartTurn();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            var thisList = _thisMod!.ThisCup!.DiceList.ToCustomBasicList();
            if (thisList.Any(Items => Items.Value == 6 || Items.Value == 9))
            {
                SingleInfo!.CurrentScore = 0;
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelayMilli(500);
                _thisMod.ThisCup.RemoveConditionalDice(Items => Items.Value == 6 || Items.Value == 9);
                if (_thisMod.ThisCup.DiceList.Count() == 0)
                {
                    await EndTurnAsync();
                    return;
                }
                await ContinueTurnAsync();
                return;
            }
            int totalScore = _thisMod.ThisCup.DiceList.Sum(Items => Items.Value);
            SingleInfo!.CurrentScore = totalScore;
            SingleInfo.TotalScore += totalScore;
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            if (WhoTurn == WhoStarts)
            {
                await GameOverAsync();
                return;
            }
            await StartNewTurnAsync();
        }
    }
}