using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.TriangleClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.Messenging;
using PyramidSolitaireCP.EventModels;
//i think this is the most common things i like to do
namespace PyramidSolitaireCP.Data
{
    [SingletonGame]
    public class PyramidSolitaireSaveInfo : ObservableObject, IMappable
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>();
        //anything else needed to save a game will be here.


        public SavedDiscardPile<SolitaireCard>? DiscardPileData { get; set; }
        public SavedDiscardPile<SolitaireCard>? CurrentPileData { get; set; }
        public SavedTriangle? TriangleData { get; set; }
        //public DeckRegularDict<SolitaireCard> TriangleList { get; set; } = new DeckRegularDict<SolitaireCard>();
        public DeckRegularDict<SolitaireCard> PlayList { get; set; } = new DeckRegularDict<SolitaireCard>();

        public SolitaireCard RecentCard { get; set; } = new SolitaireCard();

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
        
        public int FirstDeck { get; set; }
        public int SecondDeck { get; set; }
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