using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace BowlingDiceGameCP.Data
{
    [SingletonGame] //taking a risk
    public class SingleDiceInfo : ObservableObject, IBasicDice<bool>, IDiceContainer<bool>
    {
        public int HeightWidth { get; } = 60;
        private bool _value;
        public bool Value
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
        private bool _didHit;
        public bool DidHit
        {
            get { return _didHit; }
            set
            {
                if (SetProperty(ref _didHit, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
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
        public IGamePackageResolver? MainContainer { get; set; }
        public CustomBasicList<bool> GetPossibleList
        {
            get
            {
                WeightedAverageLists<bool> weight = new WeightedAverageLists<bool>();
                weight.MainContainer = MainContainer!;
                weight.AddSubItem(60, 20).AddSubItem(30, 10);
                weight.FillExtraSubItems(5, 10);
                weight.AddWeightedItem(false);
                weight.AddSubItem(700, 100).AddSubItem(400, 50).AddSubItem(70, 40);
                weight.FillExtraSubItems(70, 140);
                weight.AddWeightedItem(true);
                return weight.GetWeightedList();
            }
        }
        public void Populate(bool chosen)
        {
            Value = chosen;
        }
    }
}