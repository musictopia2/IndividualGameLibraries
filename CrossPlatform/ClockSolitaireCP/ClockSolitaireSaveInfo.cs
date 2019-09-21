using BaseSolitaireClassesCP.ClockClasses;
using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace ClockSolitaireCP
{
    [SingletonGame]
    public class ClockSolitaireSaveInfo : ObservableObject
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>(); //hopefully now its okay to use decklist since we don't have globals anymore
        public CustomBasicList<ClockInfo> SavedClocks = new CustomBasicList<ClockInfo>();
        public int CurrentCard { get; set; }
        public int PreviousOne { get; set; }
        private int _CardsLeft;

        public int CardsLeft
        {
            get { return _CardsLeft; }
            set
            {
                if (SetProperty(ref _CardsLeft, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.CardsLeft = value;
                }

            }
        }
        private ClockSolitaireViewModel? _thisMod;
        public void LoadMod(ClockSolitaireViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.CardsLeft = CardsLeft;
        }
    }
}
