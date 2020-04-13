using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using ShipCaptainCrewCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace ShipCaptainCrewCP.Logic
{
    [SingletonGame]
    public class ShipCaptainCrewMainGameClass : DiceGameClass<SimpleDice, ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>
    {


        private readonly ShipCaptainCrewVMData _model;
        private readonly CommandContainer _command;
        private readonly StandardRollProcesses<SimpleDice, ShipCaptainCrewPlayerItem> _roller;

        public ShipCaptainCrewMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            ShipCaptainCrewVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            BasicGameContainer<ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo> gameContainer,
            StandardRollProcesses<SimpleDice, ShipCaptainCrewPlayerItem> roller) :
            base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, roller)
        {
            _model = currentMod; //if not needed, take this out and the _model variable.
            _command = command;
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
                await Delay!.DelayMilli(300);
            }
            await _roller.RollDiceAsync();
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
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.Wins = 0;
                thisPlayer.WentOut = false;
                thisPlayer.Score = 0;
                thisPlayer.TookTurn = false;
            });
            await FinishUpAsync(isBeginning);
        }

        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            _model!.Cup!.HideDice();
            SaveRoot.RollNumber = 1;
            _model.Cup.CanShowDice = false;
            SingleInfo!.TookTurn = false;
            _model.Cup.UnholdDice();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override bool ShowDiceUponAutoSave => SaveRoot!.RollNumber > 1; //i think.

        private void RefreshPlay()
        {
            PlayerList.ForEach(x => x.Score = 0);
            PlayerList.ForConditionalItems(x => x.InGame == true, (x => x.TookTurn = false));
        }

        private async Task LastStepAsync()
        {
            if (PlayerList.Count(items => items.InGame == true) == 2)
            {
                var tieList = PlayerList.OrderByDescending(items => items.Score).Take(2).ToCustomBasicList();
                if (tieList.First().Score == tieList.Last().Score)
                {
                    RefreshPlay();
                    await StartNewTurnAsync();
                    return;
                }
                int oldTurn = WhoTurn;
                WhoTurn = tieList.First().Id;
                SingleInfo = PlayerList!.GetWhoPlayer();
                SingleInfo.Wins++;
                if (SingleInfo.Wins == 2)
                {
                    await ShowWinAsync();
                    return;
                }
                RefreshPlay();
                WhoTurn = oldTurn;
                await StartNewTurnAsync();
                return;
            }
            var thisList = PlayerList.Where(items => items.InGame == true).OrderBy(items => items.Score).Take(2).ToCustomBasicList();
            if (thisList.Count < 2)
                throw new BasicBlankException("Must be at least 2 players");
            if (thisList.First().Score == thisList.Last().Score)
            {
                await StartNewTurnAsync();
                return;
            }
            thisList.First().InGame = false;
            thisList.First().WentOut = true;
            thisList.First().TookTurn = true;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.InGame == false)
                WhoTurn = await PlayerList.CalculateWhoTurnAsync(true);
            PlayerList.ForEach(thisPlayer => thisPlayer.Score = 0);
            await StartNewTurnAsync();
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            HoldShips();
            if (SaveRoot!.RollNumber <= 3 && _model.Cup!.HowManyHeldDice() < 5)
            {
                await ContinueTurnAsync();
                return;
            }
            var otherList = GetShipList();
            if (otherList.Count < 3)
            {
                await EndTurnAsync();
                return;
            }
            var tempList = GetScoringGuide(otherList);
            var score = tempList.Sum(items => items.Value);
            SingleInfo!.Score = score;
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            await EndTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            _command.ManuelFinish = true;
            SingleInfo!.TookTurn = true;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            if (PlayerList.All(x => x.TookTurn))
            {
                await LastStepAsync();
                return;
            }
            await StartNewTurnAsync();
        }
        public override async Task HoldUnholdDiceAsync(int index)
        {
            var thisList = GetShipList();
            if (thisList.Any(items => items.Index == index))
            {
                await UIPlatform.ShowMessageAsync("Cannot unselect a part of a ship");
                return;
            }
            if (thisList.Count < 3)
            {
                await base.HoldUnholdDiceAsync(index);
            }
            var scoreList = GetScoringGuide(thisList);
            if (scoreList.Any(items => items.Index == index) == false)
                throw new BasicBlankException($"{index} was not found on the scoring or the ship.");
            var thisDice = scoreList.First(items => items.Index == index);
            bool willHold = thisDice.Hold;
            scoreList.ForEach(items => items.Hold = willHold);
            await SendHoldMessageAsync(index);
            await ContinueTurnAsync();
        }
        public override Task AfterHoldUnholdDiceAsync()
        {
            return base.AfterHoldUnholdDiceAsync();
        }

        private void HoldShips()
        {
            var TempList = GetShipList();
            TempList.ForEach(thisDice => thisDice.Hold = true);
        }
        private CustomBasicList<SimpleDice> GetShipList()
        {
            CustomBasicList<SimpleDice> output = new CustomBasicList<SimpleDice>();
            for (int x = 4; x <= 6; x++)
            {
                if (_model.Cup!.DiceList.Any(items => items.Value == x))
                    output.Add(_model.Cup.DiceList.First(items => items.Value == x));
            }
            return output;
        }
        private CustomBasicList<SimpleDice> GetScoringGuide(CustomBasicList<SimpleDice> shipList)
        {
            var output = _model.Cup!.DiceList.ToCustomBasicList();
            output.RemoveGivenList(shipList); //hopefully this simple.
            if (output.Count > 2)
                throw new BasicBlankException("Can only have a maximum of 2 items after removing the ship");
            return output;
        }
    }
}
