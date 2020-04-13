using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.SolitaireClasses.ClockClasses;
using ClockSolitaireCP.EventModels;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace ClockSolitaireCP.Data
{
    [SingletonGame]
    public class ClockSolitaireSaveInfo : ObservableObject, IMappable
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>();
        //anything else needed to save a game will be here.

        public CustomBasicList<ClockInfo> SavedClocks = new CustomBasicList<ClockInfo>();
        public int CurrentCard { get; set; }
        public int PreviousOne { get; set; }
        private int _cardsLeft;

        public int CardsLeft
        {
            get { return _cardsLeft; }
            set
            {
                if (SetProperty(ref _cardsLeft, value))
                {
                    //can decide what to do when property changes
                    Publish();
                }

            }
        }

        private void Publish()
        {
            if (_aggregator == null)
            {
                return;
            }
            _aggregator.Publish(new CardsLeftEventModel(CardsLeft));
        }
        private IEventAggregator? _aggregator;
        public void LoadMod(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            Publish();
        }
    }
}