using AccordianSolitaireCP.EventModels;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace AccordianSolitaireCP.Data
{
    [SingletonGame]
    public class AccordianSolitaireSaveInfo : ObservableObject, IMappable
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>();
        //anything else needed to save a game will be here.
        public DeckRegularDict<AccordianSolitaireCardInfo> HandList = new DeckRegularDict<AccordianSolitaireCardInfo>();
        public int DeckSelected { get; set; }
        public int NewestOne { get; set; }
        private int _score;

        public int Score
        {
            get { return _score; }
            set
            {
                if (SetProperty(ref _score, value))
                {
                    //can decide what to do when property changes
                    if (_aggregator != null)
                    {
                        PublishScore();
                    }
                }

            }
        }
        private void PublishScore()
        {
            if (_aggregator == null)
            {
                throw new BasicBlankException("No event aggrevator was sent to publish score.  Rethink");
            }
            _aggregator.Publish(new ScoreEventModel(Score));
        }
        private IEventAggregator? _aggregator;
        public void LoadMod(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            PublishScore();
        }
    }
}