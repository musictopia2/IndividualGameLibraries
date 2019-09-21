using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ShipCaptainCrewCP
{
    [SingletonGame]
    public class ShipCaptainCrewMainGameClass : DiceGameClass<SimpleDice, ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>
    {
        public ShipCaptainCrewMainGameClass(IGamePackageResolver container) : base(container) { }
        private ShipCaptainCrewViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<ShipCaptainCrewViewModel>();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
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
            await ThisRoll!.RollDiceAsync(); //the computer just rolls every time period.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            SetUpDice();
            SaveRoot!.ImmediatelyStartTurn = true;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.Wins = 0;
                thisPlayer.WentOut = false;
                thisPlayer.Score = 0;
            });
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            _thisMod!.ThisCup!.HideDice();
            _thisMod.ThisCup.CanShowDice = false;
            _thisMod.ThisCup.UnholdDice();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override bool ShowDiceUponAutoSave => SaveRoot!.RollNumber > 1; //i think.
        private async Task LastStepAsync()
        {
            if (PlayerList.Count(items => items.InGame == true) == 2)
            {
                var tieList = PlayerList.OrderByDescending(items => items.Score).Take(2).ToCustomBasicList();
                if (tieList.First().Score == tieList.Last().Score)
                {
                    PlayerList!.ForEach(thisPlayer => thisPlayer.Score = 0);
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
                PlayerList.ForEach(thisPlayer => thisPlayer.Score = 0);
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
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.InGame == false)
                WhoTurn = await PlayerList.CalculateWhoTurnAsync(true);
            PlayerList.ForEach(thisPlayer => thisPlayer.Score = 0);
            await StartNewTurnAsync();
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            HoldShips();
            if (SaveRoot!.RollNumber <= 3 && _thisMod!.ThisCup!.HowManyHeldDice() < 5)
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
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            await EndTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            _thisMod!.CommandContainer!.ManuelFinish = true;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            if (WhoTurn == WhoStarts)
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
                await _thisMod!.ShowGameMessageAsync("Cannot unselect a part of a ship");
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
                if (_thisMod!.ThisCup!.DiceList.Any(items => items.Value == x))
                    output.Add(_thisMod.ThisCup.DiceList.First(items => items.Value == x));
            }
            return output;
        }
        private CustomBasicList<SimpleDice> GetScoringGuide(CustomBasicList<SimpleDice> shipList)
        {
            var output = _thisMod!.ThisCup!.DiceList.ToCustomBasicList();
            output.RemoveGivenList(shipList); //hopefully this simple.
            if (output.Count > 2)
                throw new BasicBlankException("Can only have a maximum of 2 items after removing the ship");
            return output;
        }
    }
}