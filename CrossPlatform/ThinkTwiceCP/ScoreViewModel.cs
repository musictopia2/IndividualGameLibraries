using BasicGameFramework.Attributes;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.Dice;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Linq; //sometimes i do use linq.
namespace ThinkTwiceCP
{
    [SingletonGame]
    public class ScoreViewModel : ObservableObject
    {
        private readonly ThinkTwiceViewModel _thisMod;
        private readonly ThinkTwiceMainGameClass _mainGame;
        public CustomBasicList<string> TextList;
        private int _ItemSelected = -1;
        public int ItemSelected
        {
            get
            {
                return _ItemSelected;
            }

            set
            {
                if (SetProperty(ref _ItemSelected, value) == true)
                {
                    if (ItemSelected == -1)
                        _thisMod.CategoryChosen = "None";
                    else
                        _thisMod.CategoryChosen = TextList[ItemSelected];
                    _mainGame.SaveRoot!.CategorySelected = value; //i think.
                }
            }
        }
        private string _Text = "";
        public string Text
        {
            get { return _Text; }
            set
            {
                if (SetProperty(ref _Text, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public void ClearBoard()
        {
            ItemSelected = -1;
        }
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (SetProperty(ref _IsEnabled, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public BasicGameCommand ScoreDescriptionCommand { get; set; }
        public BasicGameCommand ScoreCalculateCommand { get; set; }
        public BasicGameCommand<string> ChangeSelectionCommand { get; set; }

        private string GetDescriptionText()
        {
            if (ItemSelected == 5)
                return "2 of a kind:  10 points" + Constants.vbCrLf + "3 of a kind:  20 points" + Constants.vbCrLf + "4 of a kind:  30 points" + Constants.vbCrLf + "5 of a kind:  50 points" + Constants.vbCrLf + "6 of a kind:  100 points";
            return "Sum of all the dice";
        } // hopefully its this simple (?)

        public ScoreViewModel(ThinkTwiceViewModel thisMod, ThinkTwiceMainGameClass mainGame)
        {
            _thisMod = thisMod;
            _mainGame = mainGame;
            TextList = new CustomBasicList<string>
            {
                "Different (1, 2, 3, 4, 5, 6)",
                "Even (2, 4, 6)",
                "High (4, 5, 6)",
                "Low (1, 2, 3)",
                "Odd (1, 3, 5)",
                "Same (2, 2, 2)"
            };
            Text = "Category Information"; //forgot this part.
            _thisMod.CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully this works.
            ScoreDescriptionCommand = new BasicGameCommand(thisMod, async items =>
            {
                if (ItemSelected == -1)
                    throw new BasicBlankException("Nothing Selected");
                string ThisText = GetDescriptionText();
                await thisMod.ShowGameMessageAsync(ThisText);
            }, items => CanClickCommands(true), thisMod, thisMod.CommandContainer!);
            ChangeSelectionCommand = new BasicGameCommand<string>(thisMod, items =>
            {
                ItemSelected = TextList.IndexOf(items);
            }, items => CanClickCommands(false), thisMod, thisMod.CommandContainer!);
            ScoreCalculateCommand = new BasicGameCommand(thisMod, async items => await mainGame.ScoreClickedAsync(),
                items => CanClickCommands(true), thisMod, thisMod.CommandContainer!);
        }
        private bool CanClickCommands(bool isMain)
        {
            if (_mainGame.SaveRoot!.CategoryRolled == -1)
                return false;// no matter what
            if (isMain == false)
                return true;
            if (ItemSelected == -1)
                return false;
            return true; // well see
        }
        private void CommandContainer_ExecutingChanged()
        {
            IsEnabled = !_thisMod.CommandContainer!.IsExecuting;
        }
        #region "Score Processes"
        public int CalculateScore()
        {
            int index = ItemSelected;
            if (index == -1)
                throw new BasicBlankException("Cannot calculate the score because nothing was selected");
            if (_mainGame.SaveRoot!.CategoryRolled == -1)
                throw new BasicBlankException("Cannot calculate the score because the category dice has not been rolled");
            if (_mainGame.SaveRoot.WhichMulti == 0)
                return 0;
            bool sameCategory;
            sameCategory = WasSameCategory();
            int newMulti = MultiToUse(sameCategory);
            CustomBasicList<SimpleDice> tempList;
            tempList = _thisMod.ThisCup!.DiceList.ToCustomBasicList(); //i think
            CustomBasicList<int> mainList = new CustomBasicList<int>();
            foreach (var thisDice in tempList)
                mainList.Add(thisDice.Value);
            int scoress;
            scoress = FirstScore(mainList, out bool Additionals);
            if (Additionals == true)
                scoress *= 2;
            return scoress * newMulti;
        }
        private int FirstScore(CustomBasicList<int> thisCol, out bool receiveMults)
        {
            int index;
            index = ItemSelected;
            if (index == 5)
            {
                int maxs;
                maxs = MaxKinds(thisCol, out receiveMults);
                if (maxs == 2)
                    return 10;
                if (maxs == 3)
                    return 20;
                if (maxs == 4)
                    return 30;
                if (maxs == 5)
                    return 50;
                if (maxs == 6)
                    return 100;
                return 0;
            }
            if (index == 0)
                return ScoreDifferent(thisCol, out receiveMults);
            int x;
            CustomBasicList<int> newCol = new CustomBasicList<int>();
            if (index == 3)
            {
                for (x = 1; x <= 3; x++)
                    newCol.Add(x);
                int tempScore;
                tempScore = ScoreCombo(thisCol, newCol, out receiveMults);
                return tempScore + 10;
            }
            if (index == 1)
            {
                newCol.Add(2);
                newCol.Add(4);
                newCol.Add(6);
            }
            else if (index == 2)
            {
                for (x = 4; x <= 6; x++)
                    newCol.Add(x);
            }
            else if (index == 4)
            {
                newCol.Add(1);
                newCol.Add(3);
                newCol.Add(5);
            }
            else
                throw new BasicBlankException("Can't find the combo for index " + index);
            return ScoreCombo(thisCol, newCol, out receiveMults);
        }
        private int ScoreCombo(CustomBasicList<int> mainCol, CustomBasicList<int> comboList, out bool receiveMults)
        {
            int sums = mainCol.Where(items => comboList.Contains(items)).Sum();
            int counts = mainCol.Where(items => comboList.Contains(items)).Count();
            if (counts == 6)
                receiveMults = true;
            else
                receiveMults = false;
            return sums;
        }
        private int ScoreDifferent(CustomBasicList<int> thisCol, out bool receiveMults)
        {
            var temps = thisCol.GroupBy(Items => Items).ToCustomBasicList();
            if (temps.Count == 6)
            {
                receiveMults = true;
                return thisCol.Sum(Items => Items);
            }
            else
            {
                receiveMults = false;
                return temps.Sum(Items => Items.Key);
            }
        }
        private int MaxKinds(CustomBasicList<int> thisCol, out bool receiveMults)
        {
            int counts = thisCol.MaximumDuplicates();
            if (counts == 6)
                receiveMults = true;
            else
                receiveMults = false;
            return counts;
        }
        private int MultiToUse(bool sameCategory)
        {
            int firstMulti;
            if (_mainGame.SaveRoot!.WhichMulti == -1)
                firstMulti = 1;
            else
                firstMulti = _mainGame.SaveRoot.WhichMulti;
            if (sameCategory == false)
                return firstMulti;
            return firstMulti * 2;
        }
        private bool WasSameCategory()
        {
            int index;
            index = ItemSelected;
            if (index + 1 == _mainGame.SaveRoot!.CategoryRolled)
                return true;
            return false;
        }
        #endregion
    }
}