using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using CribbagePatienceCP.EventModels;
namespace CribbagePatienceCP.Data
{
    [SingletonGame]
    public class CribbagePatienceSaveInfo : ObservableObject, IMappable
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>();
        //anything else needed to save a game will be here.
        public CustomBasicCollection<int> ScoreList { get; set; } = new CustomBasicCollection<int>();
        public SavedDiscardPile<CribbageCard>? StartCard { get; set; } //this is the data for the start card.

        private CustomBasicList<ScoreHandCP> _handScores = new CustomBasicList<ScoreHandCP>();

        public CustomBasicList<ScoreHandCP> HandScores
        {
            get { return _handScores; }
            set
            {
                if (SetProperty(ref _handScores, value))
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
            _aggregator.Publish(new HandScoresEventModel(_handScores));
        }

        private IEventAggregator? _aggregator;

        public void Load(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            Publish();

        }
    }
}