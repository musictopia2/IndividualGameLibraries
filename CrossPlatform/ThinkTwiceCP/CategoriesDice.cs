using BasicGameFramework.Attributes;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.SimpleMiscClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System;
using System.Threading.Tasks;
using static BasicGameFramework.Dice.SharedDiceRoutines;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace ThinkTwiceCP
{
    [SingletonGame] //can risk doing this too.
    public class CategoriesDice : ObservableObject, ICompleteSingleDice<string>, IHoldDice
    {
        private bool _Hold;
        public bool Hold
        {
            get { return _Hold; }
            set
            {
                if (SetProperty(ref _Hold, value))
                {
                    _mainGame.SaveRoot!.CategoryHeld = value;
                }
            }
        }
        public BasicGameCommand? CategoryClickCommand { get; set; }
        public CategoriesDice(ThinkTwiceMainGameClass mainGame, ThinkTwiceViewModel thisMod)
        {
            MainContainer = mainGame.MainContainer;
            _mainGame = mainGame;
            CategoryClickCommand = new BasicGameCommand(thisMod, async items => await mainGame.CategoryClickedAsync(),
                Items => Visible, thisMod, thisMod.CommandContainer!);
        }
        readonly ThinkTwiceMainGameClass _mainGame;
        public int HeightWidth { get; } = 60;
        private IAsyncDelayer? _delay;
        public IGamePackageResolver? MainContainer { get; set; }
        private string _Value = "";
        public string Value
        {
            get { return _Value; }
            set
            {
                if (SetProperty(ref _Value, value))
                {
                    switch (value)
                    {
                        case "":
                            _mainGame.SaveRoot!.CategoryRolled = -1;
                            return;
                        case "D":
                            _mainGame.SaveRoot!.CategoryRolled = 1;
                            return;
                        case "E":
                            _mainGame.SaveRoot!.CategoryRolled = 2;
                            return;
                        case "H":
                            _mainGame.SaveRoot!.CategoryRolled = 3;
                            return;
                        case "L":
                            _mainGame.SaveRoot!.CategoryRolled = 4;
                            return;
                        case "O":
                            _mainGame.SaveRoot!.CategoryRolled = 5;
                            return;
                        case "S":
                            _mainGame.SaveRoot!.CategoryRolled = 6;
                            return;
                        default:
                            throw new BasicBlankException("Value Not supported");
                    }
                }

            }
        }
        private bool _Visible;

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (SetProperty(ref _Visible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Index;
        public int Index
        {
            get { return _Index; }
            set
            {
                if (SetProperty(ref _Index, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private void FillText(int whichValue)
        {
            if (whichValue == 1)
                Value = "D";
            else if (whichValue == 2)
                Value = "E";
            else if (whichValue == 3)
                Value = "H";
            else if (whichValue == 4)
                Value = "L";
            else if (whichValue == 5)
                Value = "O";
            else if (whichValue == 6)
                Value = "S";
            else
                throw new Exception("Cannot find a value for " + whichValue);
        }
        public void LoadSavedGame()
        {
            if (_mainGame.SaveRoot!.CategoryRolled == -1)
                return;
            FillText(_mainGame.SaveRoot.CategoryRolled); //i think.
            Visible = true;
        }
        public void NewTurn()
        {
            Value = ""; //i think
            Visible = false; //i think
            _mainGame.SaveRoot!.CategorySelected = -1; //i think
        }
        public CustomBasicList<string> GetPossibleList //done
        {
            get
            {
                WeightedAverageLists<string> thisWeight = new WeightedAverageLists<string>();
                thisWeight.MainContainer = MainContainer;
                thisWeight.AddWeightedItem("D", 40, 50).AddWeightedItem("E", 20)
                .AddWeightedItem("O", 25, 30)
                .AddWeightedItem("S", 30).AddWeightedItem("H", 5, 10).AddWeightedItem("L", 35, 40);
                return thisWeight.GetWeightedList();
            }
        }

        public async Task ShowRollingAsync(CustomBasicList<string> thisCol)
        {
            if (Hold == true)
                throw new BasicBlankException("Can't show it rolling because you held on to the dice");

            Visible = true;
            AsyncDelayer.SetDelayer(this, ref _delay!);
            await thisCol.ForEachAsync(async Items =>
            {
                Populate(Items);
                await _delay.DelaySeconds(.07); //i had .05 before so keep that
            });
        }
        public async Task SendMessageAsync(string category, CustomBasicList<string> thisList)
        {
            await _mainGame.ThisNet!.SendAllAsync(category, thisList);
        }
        public async Task<CustomBasicList<string>> GetDiceList(string content)
        {
            return await js.DeserializeObjectAsync<CustomBasicList<string>>(content);
        }
        public CustomBasicList<string> RollDice(int HowManySections = 6)
        {
            AsyncDelayer.SetDelayer(this, ref _delay!);
            return GetSingleRolledDice(HowManySections, this);
        }
        public void Populate(string chosen)
        {
            Value = chosen;
        }
    }
}