using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.SimpleMiscClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFramework.Dice.SharedDiceRoutines;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace ThinkTwiceCP
{
    [SingletonGame] //can risk doing this too.
    public class Multiplier : ObservableObject, ICompleteSingleDice<int>
    {
        private int _Value = -1;

        public int Value
        {
            get { return _Value; }
            set
            {
                if (SetProperty(ref _Value, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        readonly ThinkTwiceMainGameClass _mainGame;
        public int HeightWidth { get; } = 60;
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

        public void LoadSavedGame()
        {
            if (_mainGame.SaveRoot!.WhichMulti == -1)
                return;
            Value = _mainGame.SaveRoot.WhichMulti;
            Visible = true;
        }
        public void NewTurn()
        {
            Value = -1; //i think
            Visible = false; //i think
            _mainGame.SaveRoot!.WhichMulti = -1; //i think
        }
        private IAsyncDelayer? _delay;
        public IGamePackageResolver? MainContainer { get; set; }
        public Multiplier(ThinkTwiceMainGameClass mainGame)
        {
            MainContainer = mainGame.MainContainer;
            _mainGame = mainGame;
        }
        public CustomBasicList<int> GetPossibleList
        {
            get
            {
                WeightedAverageLists<int> ThisWeight = new WeightedAverageLists<int>();
                ThisWeight.MainContainer = MainContainer;
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
            await _mainGame.ThisNet!.SendAllAsync(category, thisList);
        }
        public async Task ShowRollingAsync(CustomBasicList<int> thisCol)
        {
            Visible = true;
            AsyncDelayer.SetDelayer(this, ref _delay!);
            await thisCol.ForEachAsync(async items =>
            {
                Populate(items);
                await _delay.DelaySeconds(.07); //i had .05 before so keep that
            });
        }
        public async Task<CustomBasicList<int>> GetDiceList(string content)
        {
            return await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
        }
        public CustomBasicList<int> RollDice(int howManySections = 7)
        {
            AsyncDelayer.SetDelayer(this, ref _delay!);
            return GetSingleRolledDice(howManySections, this);
        }
        public void Populate(int chosen)
        {
            Value = chosen;
        }
    }
}