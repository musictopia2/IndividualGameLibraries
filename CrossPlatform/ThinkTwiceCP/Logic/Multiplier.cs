using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThinkTwiceCP.Data;
using static BasicGameFrameworkLibrary.Dice.SharedDiceRoutines;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace ThinkTwiceCP.Logic
{
    [SingletonGame] //can risk doing this too.
    [AutoReset]
    public class Multiplier : ObservableObject, ICompleteSingleDice<int>
    {
        private int _value = -1;

        public int Value
        {
            get { return _value; }
            set
            {
                if (SetProperty(ref _value, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        readonly ThinkTwiceGameContainer _gameContainer;
        public int HeightWidth { get; } = 60;
        private bool _visible;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (SetProperty(ref _visible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _index;

        public int Index
        {
            get { return _index; }
            set
            {
                if (SetProperty(ref _index, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public void LoadSavedGame()
        {
            if (_gameContainer.SaveRoot!.WhichMulti == -1)
                return;
            Value = _gameContainer.SaveRoot.WhichMulti;
            Visible = true;
        }
        public void NewTurn()
        {
            Value = -1; //i think
            Visible = false; //i think
            _gameContainer.SaveRoot!.WhichMulti = -1; //i think
        }
        //private IAsyncDelayer? _delay;
        public IGamePackageResolver? MainContainer { get; set; }
        public Multiplier(ThinkTwiceGameContainer gameContainer)
        {
            MainContainer = gameContainer.Resolver;
            _gameContainer = gameContainer;
        }
        public CustomBasicList<int> GetPossibleList
        {
            get
            {
                WeightedAverageLists<int> ThisWeight = new WeightedAverageLists<int>();
                ThisWeight.MainContainer = MainContainer!;
                ThisWeight.AddSubItem(0, 200).AddSubItem(5, 5);
                CustomBasicList<int> SixList = ThisWeight.GetSubList();
                ThisWeight.AddSubItem(0, 80).AddSubItem(2, 20);
                CustomBasicList<int> FiveList = ThisWeight.GetSubList();
                ThisWeight.AddWeightedItem(0, 20, 30).AddWeightedItem(1, 35)
                .AddWeightedItem(2, 20, 25)
                .AddWeightedItem(3, 10, 20).AddWeightedItem(4, 3).AddWeightedItem(5, FiveList)
                .AddWeightedItem(6, SixList);
                return ThisWeight.GetWeightedList();
            }
        }
        public async Task SendMessageAsync(string category, CustomBasicList<int> thisList)
        {
            await _gameContainer.Network!.SendAllAsync(category, thisList);
        }
        public async Task ShowRollingAsync(CustomBasicList<int> thisCol)
        {
            Visible = true;
            await thisCol.ForEachAsync(async items =>
            {
                Populate(items);
                await _gameContainer.Delay.DelaySeconds(.07); //i had .05 before so keep that
            });
        }
        public async Task<CustomBasicList<int>> GetDiceList(string content)
        {
            return await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
        }
        public CustomBasicList<int> RollDice(int howManySections = 7)
        {
            return GetSingleRolledDice(howManySections, this);
        }
        public void Populate(int chosen)
        {
            Value = chosen;
        }
    }
}