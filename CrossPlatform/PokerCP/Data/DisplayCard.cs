using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using PokerCP.EventModels;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace PokerCP.Data
{
    public class DisplayCard : ObservableObject //for now 2 classes.  may be able to do as one (?)
    {

        private readonly IEventAggregator _aggregator;

        //private readonly INewCard _uiCard;
        //private readonly PokerViewModel _thisMod;
        public DisplayCard()
        {
            _aggregator = Resolve<IEventAggregator>();
            //can't do resolve for view model.  otherwise, you get a brand new one.
            //_thisMod = Resolve<MainVi>();
        }

        private PokerCardInfo? _currentCard;

        public PokerCardInfo CurrentCard
        {
            get { return _currentCard!; }
            set
            {
                if (SetProperty(ref _currentCard, value))
                {
                    //can decide what to do when property changes
                    int index = GlobalClass.PokerList.IndexOf(this);
                    _aggregator.Publish(new NewCardEventModel(index));
                }

            }
        }
        private bool _willHold;
        public bool WillHold
        {
            get
            {
                return _willHold;
            }

            set
            {
                if (SetProperty(ref _willHold, value) == true)
                    // code to run
                    OnPropertyChanged(nameof(Text));
            }
        }


        public string Text
        {
            get
            {
                if (WillHold == true)
                    return "Hold";
                return "";
            }
        }


    }
}
