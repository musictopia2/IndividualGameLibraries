using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BowlingDiceGameCP
{
    [SingletonGame] //taking a risk
    public class SingleDiceInfo : ObservableObject, IBasicDice<bool>, IDiceContainer<bool>
    {
        public int HeightWidth { get; } = 60;
        private bool _Value;
        public bool Value
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
        private bool _DidHit;
        public bool DidHit
        {
            get { return _DidHit; }
            set
            {
                if (SetProperty(ref _DidHit, value))
                {
                    //can decide what to do when property changes
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
        public IGamePackageResolver? MainContainer { get; set; }
        public CustomBasicList<bool> GetPossibleList
        {
            get
            {
                WeightedAverageLists<bool> ThisWeight = new WeightedAverageLists<bool>();
                ThisWeight.MainContainer = MainContainer;
                ThisWeight.AddSubItem(60, 20).AddSubItem(30, 10);
                ThisWeight.FillExtraSubItems(5, 10);
                ThisWeight.AddWeightedItem(false);
                ThisWeight.AddSubItem(700, 100).AddSubItem(400, 50).AddSubItem(70, 40);
                ThisWeight.FillExtraSubItems(70, 140);
                ThisWeight.AddWeightedItem(true);
                return ThisWeight.GetWeightedList();
            }
        }
        public void Populate(bool chosen)
        {
            Value = chosen;
        }
    }
}