using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FillOrBustCP
{
    [SingletonGame]
    public class FillOrBustMainGameClass : CardGameClass<FillOrBustCardInformation, FillOrBustPlayerItem, FillOrBustSaveInfo>, IMiscDataNM,
        IStandardRoller<SimpleDice, FillOrBustPlayerItem>, IFinishStart
    {
        internal StandardRollProcesses<SimpleDice, FillOrBustPlayerItem>? ThisRoll;

        public DiceCup<SimpleDice> ThisCup => _thisMod!.ThisCup!;
        public FillOrBustMainGameClass(IGamePackageResolver container) : base(container) { }

        private FillOrBustViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<FillOrBustViewModel>();
            ThisRoll = MainContainer.Resolve<StandardRollProcesses<SimpleDice, FillOrBustPlayerItem>>();
        }
        protected override async Task AfterDrawingAsync()
        {
            var thisCard = _thisMod!.Pile1!.GetCardInfo();
            if (thisCard.IsOptional == true)
            {
                if (CanPlayRevenge() == true)
                    SaveRoot!.GameStatus = EnumGameStatusList.ChoosePlay;
                else
                    SaveRoot!.GameStatus = EnumGameStatusList.DrawCard;
            }
            else if (thisCard.Status == EnumCardStatusList.NoDice)
            {
                SaveRoot!.TempScore = 0;
                SaveRoot.DiceScore = 0;
                SaveRoot.GameStatus = EnumGameStatusList.EndTurn;
            }
            else
            {
                SaveRoot!.GameStatus = EnumGameStatusList.RollDice;
                SaveRoot.FillsRequired = thisCard.FillsRequired;
            }
            if (SaveRoot.GameStatus != EnumGameStatusList.DrawCard && SaveRoot.GameStatus != EnumGameStatusList.EndTurn)
                _thisMod.ThisCup!.HowManyDice = 6;
            await ContinueTurnAsync();
        }

        public async Task AfterRollingAsync()
        {
            if (SaveRoot!.GameStatus == EnumGameStatusList.ChoosePlay)
            {
                SaveRoot.FillsRequired = 1; //because you chose vengence.
                var thisList = WinList();
                thisList.ForEach(items =>
                {
                    FillOrBustPlayerItem thisPlayer = PlayerList![items];
                    int points = thisPlayer.TotalScore;
                    if (points < 2500)
                        points = 0;
                    else
                        points -= 2500;
                    thisPlayer.TotalScore = points;
                });
            }
            await ProcessScoresAsync();
            await ContinueTurnAsync();
        }

        public async Task AfterSelectUnselectDiceAsync()
        {
            await ContinueTurnAsync();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            //anything else needed is here.
            SaveRoot!.LoadMod(_thisMod!);
            _thisMod!.LoadCup(SaveRoot, true);
            SaveRoot.DiceList.MainContainer = MainContainer;
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            //if there is nothing, then just won't do anything.
            await Task.CompletedTask;
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (IsLoaded == false)
            {
                SaveRoot!.LoadMod(_thisMod!);
                _thisMod!.LoadCup(SaveRoot, false);
                SaveRoot.DiceList.MainContainer = MainContainer;
            }
            LoadControls();
            if (isBeginning == false)
            {
                PlayerList!.ForEach(items =>
                {
                    items.TotalScore = 0;
                    items.CurrentScore = 0;
                });
            }
            SaveRoot!.GameStatus = EnumGameStatusList.DrawCard;
            return base.StartSetUpAsync(isBeginning);
        }
        public override Task ContinueTurnAsync()
        {
            _thisMod!.Instructions = SaveRoot!.GameStatus switch
            {
                EnumGameStatusList.DrawCard => "Draw a card",
                EnumGameStatusList.EndTurn => "Need to end your turn",
                EnumGameStatusList.RollDice => "Roll the dice",
                EnumGameStatusList.ChoosePlay => "Either play the vengeance or draw again",
                EnumGameStatusList.ChooseDice => "Choose at least one scoring dice to remove",
                EnumGameStatusList.ChooseDraw => "Either draw a card or end your turn",
                EnumGameStatusList.ChooseRoll => "Either roll the dice to get more points or end your turn to keep your existing score this round",
                _ => throw new BasicBlankException("No status"),
            };
            return base.ContinueTurnAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                //put in cases here.
                case "choosedice":
                    await ThisRoll!.SelectUnSelectDiceAsync(int.Parse(content));
                    break;
                case "updatescore":
                    await AddToTempAsync(int.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public Task FinishStartAsync()
        {
            if (SaveRoot!.GameStatus != EnumGameStatusList.RollDice)
            {
                _thisMod!.ThisCup!.HowManyDice = _thisMod!.ThisCup.DiceList.Count();
                _thisMod.ThisCup.CanShowDice = true;
                _thisMod.ThisCup.Visible = true;
            }
            else
            {
                _thisMod!.ThisCup!.HowManyDice = 6;
                _thisMod.ThisCup.Visible = false;
                _thisMod.ThisCup.CanShowDice = false;
            }
            return Task.CompletedTask;
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.GameStatus = EnumGameStatusList.DrawCard;
            SingleInfo!.CurrentScore = 0;
            ThisCup.CanShowDice = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            TransferScore();
            SingleInfo!.TotalScore += SingleInfo.CurrentScore;
            _thisMod!.CommandContainer!.ManuelFinish = true;
            if (SingleInfo.TotalScore < 5000)
            {
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                await StartNewTurnAsync();
                return;
            }
            var ThisList = WinList();
            if (ThisList.Count > 1)
            {
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                await StartNewTurnAsync();
                return;
            }
            SingleInfo = PlayerList![ThisList.Single()];
            await ShowWinAsync(); //i think this simple.
        }
        #region "Unique Game Features"
        private async Task ProcessScoresAsync()
        {
            var tempList = _thisMod!.ThisCup!.DiceList.ToCustomBasicList();
            int nums;
            nums = CalculateScore(tempList, true, out bool Fills);
            var thisCard = _thisMod.Pile1!.GetCardInfo();
            if (nums == 0)
            {
                if (thisCard.Status != EnumCardStatusList.MustBust)
                    SaveRoot!.TempScore = 0;
                SaveRoot!.GameStatus = EnumGameStatusList.EndTurn;
            }
            else
            {
                if (Fills == true)
                {
                    SaveRoot!.TempScore += nums;
                    if (thisCard.Status == EnumCardStatusList.MustBust)
                    {
                        if (ThisTest!.NoAnimations == false)
                            await Delay!.DelaySeconds(2);
                        _thisMod.ThisCup.HowManyDice = 6;
                        _thisMod.ThisCup.CanShowDice = false;
                        SaveRoot.GameStatus = EnumGameStatusList.RollDice;
                    }
                    else
                    {
                        SaveRoot.TempScore += thisCard.BonusAmount;
                        if (SaveRoot.FillsRequired > 0)
                            SaveRoot.FillsRequired--;
                        if (SaveRoot.FillsRequired == 0)
                        {
                            if (thisCard.AddToScore == true)
                            {
                                if (thisCard.Status == EnumCardStatusList.DoubleTrouble)
                                    SaveRoot.TempScore *= 2;
                                TransferScore();
                                SaveRoot.GameStatus = EnumGameStatusList.DrawCard;
                            }
                            else
                                SaveRoot.GameStatus = EnumGameStatusList.ChooseDraw;
                        }
                        else
                        {
                            if (ThisTest!.NoAnimations == false)
                                await Delay!.DelaySeconds(2);
                            _thisMod.ThisCup.HowManyDice = 6;
                            _thisMod.ThisCup.CanShowDice = false;
                            SaveRoot.GameStatus = EnumGameStatusList.RollDice;
                        }
                    }
                }
                else
                {
                    SaveRoot!.DiceScore = nums;
                    SaveRoot.GameStatus = EnumGameStatusList.ChooseDice;
                }
            }
            await ContinueTurnAsync();
        }
        private void TransferScore()
        {
            SingleInfo!.CurrentScore += SaveRoot!.TempScore;
            SaveRoot.TempScore = 0;
        }
        public int CalculateScore()
        {
            var tempList = ThisCup.DiceList.GetSelectedItems();
            return CalculateScore(tempList, false, out _);
        }
        public async Task AddToTempAsync(int score)
        {
            SaveRoot!.DiceScore = 0;
            SaveRoot.TempScore += score;
            _thisMod!.ThisCup!.RemoveSelectedDice();
            var thisCard = _thisMod.Pile1!.GetCardInfo();
            if (SaveRoot.FillsRequired > 0 || thisCard.Status == EnumCardStatusList.MustBust)
                SaveRoot.GameStatus = EnumGameStatusList.RollDice;
            else
                SaveRoot.GameStatus = EnumGameStatusList.ChooseRoll;
            await ContinueTurnAsync();
        }
        private int CalculateScore(CustomBasicList<SimpleDice> thisCol, bool considerAll, out bool hasFill)
        {
            hasFill = false; //until proven true.
            if (thisCol.Any(Items => Items.Index == 0))
                throw new BasicBlankException("Cannot have a 0 for number");
            if (thisCol.Count == 6 && considerAll == true)
            {
                if (thisCol.HasDuplicates(Items => Items.Value) == false)
                {
                    hasFill = true;
                    return 1500; //hopefully this works.
                }
            }
            int output;
            output = 0;
            int Nums = 0;
            bool rets;
            int x;
            for (x = 1; x <= 2; x++)
            {
                if (thisCol.Count < 3)
                    break;
                rets = HasTriple(ref thisCol, ref Nums);
                if (rets == true)
                {
                    if (Nums == 1)
                        output += 1000;
                    else if (Nums == 2)
                        output += 200;
                    else if (Nums == 3)
                        output += 300;
                    else if (Nums == 4)
                        output += 400;
                    else if (Nums == 5)
                        output += 500;
                    else if (Nums == 6)
                        output += 600;
                    else
                        throw new BasicBlankException("Cannot find the score for " + Nums);
                }
                else
                    break;// if the first had no triple; then the second won't either
            }
            if (thisCol.Count == 0)
            {
                if (considerAll == true)
                    hasFill = true;
                return output;
            }
            rets = HasSpecific(thisCol, 1, out int Manys);
            if (rets == true)
                output += (Manys * 100);
            if (thisCol.Count == 0)
            {
                if (considerAll == true)
                    hasFill = true;
                return output;
            }
            rets = HasSpecific(thisCol, 5, out Manys);
            if (rets == true)
                output += (Manys * 50);
            if (thisCol.Count == 0)
            {
                if (considerAll == true)
                    hasFill = true;
                return output;
            }
            if (considerAll == false)
                return 0;
            return output;
        }

        private CustomBasicList<int> WinList()
        {
            CustomBasicList<FillOrBustPlayerItem> tempList = PlayerList.OrderByDescending(items => items.TotalScore).ToCustomBasicList();
            if (tempList.First().TotalScore > tempList[1].TotalScore)
                return new CustomBasicList<int> { tempList.First().Id };
            int highScore = tempList.First().TotalScore;
            return tempList.Where(Items => Items.TotalScore == highScore).Select(items => items.Id).ToCustomBasicList();
        }
        private bool CanPlayRevenge()
        {
            var thisList = WinList();
            if (thisList.Any(items => items == WhoTurn))
                return false;
            return true;
        }
        private bool HasSpecific(CustomBasicList<SimpleDice> thisCol, int whichOne, out int howMany)
        {
            howMany = thisCol.Count(items => items.Value == whichOne);
            if (howMany == 0)
                return false;
            thisCol.RemoveAllOnly(items => items.Value == whichOne); //i think
            return true;
        }
        private bool HasTriple(ref CustomBasicList<SimpleDice> thisCol, ref int number)
        {
            int x;
            int howMany;
            int y;
            int lists;
            lists = thisCol.Count;
            CustomBasicList<SimpleDice> newList = new CustomBasicList<SimpleDice>();
            for (x = 1; x <= 6; x++)
            {
                howMany = thisCol.Count(Items => Items.Value == x);
                if (howMany >= 3)
                {
                    number = x;

                    newList = thisCol.Where(Items => Items.Value == x).ToCustomBasicList();

                    for (y = 0; y <= 2; y++)
                        thisCol.RemoveSpecificItem(newList[y]);
                    if (thisCol.Count == 0 & lists > 3)
                        throw new BasicBlankException("There must be at least one item yet for triples");
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}