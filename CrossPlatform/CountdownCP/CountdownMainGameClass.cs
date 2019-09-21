using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CountdownCP
{
    [SingletonGame]
    public class CountdownMainGameClass : DiceGameClass<CountdownDice, CountdownPlayerItem, CountdownSaveInfo>, IMiscDataNM, IFinishStart
    {
        public CustomBasicList<SimpleNumber> GetNewNumbers()
        {
            var FirstList = Enumerable.Range(1, 10).ToList();
            return (from Items in FirstList
                    select new SimpleNumber() { Used = false, Value = Items }).ToCustomBasicList();
        }
        public bool CanChooseNumber(SimpleNumber thisNumber) //done.
        {
            if (thisNumber.Used == true)
                throw new BasicBlankException("It should not have considered this number because it was already used.");
            return SaveRoot!.HintList.Any(items => items == thisNumber.Value);
        }
        public async Task ProcessChosenNumberAsync(SimpleNumber thisNumber)
        {
            _thisMod!.CommandContainer!.ManuelFinish = true; //try this.
            thisNumber.IsSelected = true;
            CountdownViewModel.ShowHints = false;
            ThisE.RepaintBoard();
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1); //so you can see the results.
            thisNumber.Used = true;
            thisNumber.IsSelected = false;
            ThisE.RepaintBoard();
            await EndTurnAsync();
        }
        public CountdownMainGameClass(IGamePackageResolver container) : base(container) { }
        private CountdownViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<CountdownViewModel>();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            if (ThisTest!.DoubleCheck == true)
            {
                var thisList = _thisMod!.ThisCup!.DiceList.OrderByDescending(Items => Items.Value);
                if (thisList.Count() != 2)
                    throw new BasicBlankException("There should have been only 2 dice");
                SaveRoot!.HintList = GetPossibleValues(thisList.First().Value, thisList.Last().Value);
                VBCompat.Stop(); //double check means i have to see what is happening.
            }
            SaveRoot!.LoadMod(_thisMod!); //i think here too.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            SaveRoot!.LoadMod(_thisMod!);
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            var thisList = SingleInfo!.NumberList.Where(Items => Items.Used == false).ToCustomBasicList();
            var newList = thisList.Where(Items => CanChooseNumber(Items) == true).ToCustomBasicList();
            if (newList.Count == 0)
            {
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelayMilli(200); //so you can at least see the computer tried to take their turn.
                if (ThisData!.MultiPlayer == true)
                    throw new BasicBlankException("Should not be multiplayer because only 2 players are allowed");
                await EndTurnAsync();
                return;
            }
            await ProcessChosenNumberAsync(newList.GetRandomItem());
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            _wasSetUp = true;
            SaveRoot!.Round = 1;
            SaveRoot.ImmediatelyStartTurn = true;
            LoadControls();
            SetUpDice();
            if (isBeginning == false)
                ResetPlayers();
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //leave warning for now.
            {
                //put in cases here.
                case "choosenumber":
                    SimpleNumber thisNumber = SingleInfo!.NumberList.Single(Items => Items.Value == int.Parse(content));
                    await ProcessChosenNumberAsync(thisNumber);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //maybe this still.
            if (ThisData!.MultiPlayer == true)
            {
                if (SingleInfo!.CanSendMessage(ThisData) == false)
                {
                    ThisCheck!.IsEnabled = true;
                    return; //to wait for the message for rolling dice.
                }
            }
            await ThisRoll!.RollDiceAsync(); //i think. 
        }
        private bool _wasSetUp;
        protected override async Task ProtectedAfterRollingAsync()
        {
            CountdownViewModel.ShowHints = false; //you have to choose it everytime.
            int DiceValue = _thisMod!.ThisCup!.DiceList.Sum(items => items.Value);
            if (DiceValue == 12)
            {
                await _thisMod.ShowGameMessageAsync($"{SingleInfo!.NickName}  has to start over again for rolling a 12");
                await StartOverAsync(SingleInfo);
                return;
            }
            if (DiceValue == 11)
            {
                CountdownPlayerItem thisPlayer;
                if (WhoTurn == 1)
                    thisPlayer = PlayerList![2];
                else
                    thisPlayer = PlayerList![1];
                await _thisMod.ShowGameMessageAsync($"{thisPlayer.NickName}  has to start over again for rolling a 11");
                await StartOverAsync(thisPlayer);
                return;
            }
            var thisList = _thisMod.ThisCup.DiceList.OrderByDescending(items => items.Value);
            if (thisList.Count() != 2)
                throw new BasicBlankException("There should have been only 2 dice");
            SaveRoot!.HintList = GetPossibleValues(thisList.First().Value, thisList.Last().Value);
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            _thisMod.CommandContainer.ManuelFinish = true;
            SaveRoot!.HintList.Clear(); //just in case.
            if (SingleInfo!.NumberList.Any(items => items.Used == false) == false)
            {
                await GameOverAsync(false);
                return;
            }
            if (SaveRoot.Round >= 30 && WhoTurn != WhoStarts)
            {
                //has to end no matter what.
                int firstCount;
                int secondCount;
                CountdownPlayerItem thisPlayer = PlayerList![1];
                firstCount = thisPlayer.NumberList.Count(Items => Items.Used == true);
                thisPlayer = PlayerList[2];
                secondCount = thisPlayer.NumberList.Count(Items => Items.Used == true);
                bool wasTie = false;
                if (firstCount > secondCount)
                    SingleInfo = PlayerList[1];
                else if (secondCount > firstCount)
                    SingleInfo = PlayerList[2];
                else
                    wasTie = true;
                await GameOverAsync(wasTie);
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            if (WhoStarts == WhoTurn)
                SaveRoot.Round++;
            await StartNewTurnAsync();
        }
        private async Task GameOverAsync(bool wasTie)
        {
            if (wasTie == false)
                await ShowWinAsync();
            else
                await ShowTieAsync();
        }
        private async Task StartOverAsync(CountdownPlayerItem thisPlayer) //done.
        {
            thisPlayer.NumberList.ForEach(items =>
            {
                items.IsSelected = false;
                items.Used = false;
            });
            ThisE.RepaintBoard();
            await EndTurnAsync();
        }
        private CustomBasicList<int> GetPossibleValues(int firstValue, int secondValue)
        {
            int addValue;
            int subtractValue;
            addValue = firstValue + secondValue;
            subtractValue = firstValue - secondValue;
            CustomBasicList<int> thisList = new CustomBasicList<int>
            {
                addValue
            };
            if (subtractValue > 0)
                thisList.Add(subtractValue);
            int multValue;
            multValue = firstValue * secondValue;
            if (multValue <= 10 && multValue != addValue && multValue != secondValue)
                thisList.Add(multValue);
            if (firstValue == secondValue && addValue != 1 && secondValue != 1 && multValue != 1)
                thisList.Add(1);
            if (firstValue == 1 && secondValue == 1)
                thisList.Add(1);// this is for sure valid for several reasons.
            if (firstValue == 6 && secondValue == 2)
                thisList.Add(3);
            if (firstValue == 6 && secondValue == 3)
                thisList.Add(2);
            return thisList;
        }
        async Task IFinishStart.FinishStartAsync()
        {
            int x = 0;
            if (_wasSetUp == false)
            {
                ThisE.RepaintBoard(); //just in case.  best to do too many than not enough.
                return;
            }
            do
            {
                x++;
                if (x > 500)
                    throw new BasicBlankException("After 5 seconds, did not get player info.  Rethink");
                if (PlayerList.First().NumberList.Count == 10 && PlayerList.Last().NumberList.Count == 10)
                    break;
                await Task.Delay(10);

            } while (true);
            ResetPlayers();
            ThisE.RepaintBoard(); //maybe needed
        }
        private void ResetPlayers()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.NumberList.ForEach(items =>
                {
                    items.Used = false;
                    items.IsSelected = false; //to reset this part.
                });
            });
        }
    }
}