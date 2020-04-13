using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CombinationHelpers;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SinisterSixCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace SinisterSixCP.Logic
{
    [SingletonGame]
    public class SinisterSixMainGameClass : DiceGameClass<EightSidedDice, SinisterSixPlayerItem, SinisterSixSaveInfo>, IMiscDataNM
    {


        private readonly SinisterSixVMData _model;
        private bool _wasAuto;
        public SinisterSixMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            SinisterSixVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            BasicGameContainer<SinisterSixPlayerItem, SinisterSixSaveInfo> gameContainer,
            StandardRollProcesses<EightSidedDice, SinisterSixPlayerItem> roller) :
            base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, roller)
        {
            _model = currentMod; //if not needed, take this out and the _model variable.
            roller.BeforeRollingAsync = BeforeRollingAsync;
            roller.CanRollAsync = CanRollAsync;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            SaveRoot!.LoadMod(_model!);
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
            SaveRoot!.ImmediatelyStartTurn = true;
            SaveRoot!.LoadMod(_model);
            SaveRoot.MaxRolls = 3;
            PlayerList!.ForEach(items => items.Score = 0);
            await FinishUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "removeselecteddice":
                    await RemoveSelectedDiceAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            _model.Cup!.HowManyDice = 6;
            _wasAuto = false;
            _model.Cup.HideDice();
            _model.Cup.CanShowDice = false;
            ProtectedStartTurn();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            if (SaveRoot!.RollNumber > SaveRoot.MaxRolls)
            {
                if (ContainsSix() == false)
                {
                    _wasAuto = true;
                    await EndTurnAsync();
                    return;
                }
            }
            await ContinueTurnAsync();
        }
        private Task BeforeRollingAsync()
        {
            return Task.CompletedTask; //we have nothing else todo.  some times we do.  did not want to have to creae another interface just for that part.
        }
        private async Task<bool> CanRollAsync()
        {
            if (SaveRoot!.RollNumber > 1)
            {
                if (ContainsSix() == true)
                {
                    await UIPlatform.ShowMessageAsync("Must remove any dice that equal 6");
                    return false;
                }
            }
            return true;
        }
        public async Task RemoveSelectedDiceAsync()
        {
            if (SingleInfo!.CanSendMessage(BasicData!) == true)
                await Network!.SendAllAsync("removeselecteddice");
            _model.Cup!.RemoveSelectedDice();
            if (SaveRoot!.RollNumber > SaveRoot.MaxRolls && ContainsSix() == false)
            {
                _wasAuto = true;
                await EndTurnAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        private bool ContainsSix()
        {
            var thisList = _model.Cup!.DiceList.ToCustomBasicList();
            thisList.KeepConditionalItems(Items => Items.Value == 6);
            var temps = thisList.GetAllPossibleCombinations();
            return temps.Any(Items => Items.Sum(News => News.Value) == 6);
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderBy(items => items.Score).Take(1).Single();
            await ShowWinAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo!.Score = _model.Cup!.DiceList.Sum(Items => Items.Value);
            if (_wasAuto == true && Test!.NoAnimations == false)
                await Delay!.DelaySeconds(1); //if auto is done, needs to see what happened.
            if (WhoTurn == WhoStarts)
                SaveRoot!.MaxRolls = SaveRoot.RollNumber - 1;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(); //i think
            if (WhoTurn == WhoStarts)
            {
                await GameOverAsync();
                return;
            }
            await StartNewTurnAsync();
        }
    }
}