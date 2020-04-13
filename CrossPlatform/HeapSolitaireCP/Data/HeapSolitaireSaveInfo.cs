using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using HeapSolitaireCP.EventModels;
namespace HeapSolitaireCP.Data
{
    [SingletonGame]
    public class HeapSolitaireSaveInfo : ObservableObject, IMappable
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>();
        //anything else needed to save a game will be here.
        public int PreviousSelected { get; set; }
        private int _Score;

        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    Publish();
                }

            }
        }
        public CustomBasicList<BasicPileInfo<HeapSolitaireCardInfo>>? WasteData { get; set; }
        public CustomBasicList<BasicPileInfo<HeapSolitaireCardInfo>>? MainPiles { get; set; }
        private IEventAggregator? _aggregator;
        private void Publish()
        {
            if (_aggregator == null)
            {
                return;
            }
            _aggregator.Publish(new ScoreEventModel(Score));
        }
        public void Load(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            Publish();
        }
    }
}