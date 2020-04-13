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
using DeadDie96CP.Data;
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
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;

namespace DeadDie96CP.Logic
{
    [SingletonGame]
    public class DeadDie96MainGameClass : DiceGameClass<TenSidedDice, DeadDie96PlayerItem, DeadDie96SaveInfo>
    {
        

        private readonly DeadDie96VMData _model;
        private readonly StandardRollProcesses<TenSidedDice, DeadDie96PlayerItem> _roller;

        public DeadDie96MainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            DeadDie96VMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            BasicGameContainer<DeadDie96PlayerItem, DeadDie96SaveInfo> gameContainer,
            StandardRollProcesses<TenSidedDice, DeadDie96PlayerItem> roller) : 
            base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, roller)
        {
            _model = currentMod; //if not needed, take this out and the _model variable.
            _roller = roller;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
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
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelayMilli(200);
            }
            await _roller!.RollDiceAsync(); //this is it for computer turn.
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderBy(Items => Items.TotalScore).Take(1).Single();
            await ShowWinAsync();
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SetUpDice();
            PlayerList!.ForEach(x =>
            {
                x.TotalScore = 0;
                x.CurrentScore = 0;
            });
            SaveRoot!.ImmediatelyStartTurn = true;
            await FinishUpAsync(isBeginning);
        }

        
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            _model!.Cup!.HowManyDice = 5;
            _model.Cup.HideDice();
            _model.Cup.CanShowDice = false;
            ProtectedStartTurn();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            var thisList = _model!.Cup!.DiceList.ToCustomBasicList();
            if (thisList.Any(x => x.Value == 6 || x.Value == 9))
            {
                SingleInfo!.CurrentScore = 0;
                if (Test!.NoAnimations == false)
                    await Delay!.DelayMilli(500);
                _model.Cup.RemoveConditionalDice(Items => Items.Value == 6 || Items.Value == 9);
                if (_model.Cup.DiceList.Count() == 0 || Test.ImmediatelyEndGame)
                {
                    await EndTurnAsync();
                    return;
                }
                await ContinueTurnAsync();
                return;
            }
            int totalScore = _model.Cup.DiceList.Sum(Items => Items.Value);
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
