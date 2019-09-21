using CommonBasicStandardLibraries.MVVMHelpers;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace PokerCP
{
    public interface INewCard //try to use dependency injection.
    {
        void NewCard(int index);
    }
    public class DisplayCard : ObservableObject //for now 2 classes.  may be able to do as one (?)
    {

        private readonly INewCard _uiCard;
        private readonly PokerViewModel _thisMod;
        public DisplayCard()
        {
            _uiCard = Resolve<INewCard>(); //this means no unit testing for this game.
            _thisMod = Resolve<PokerViewModel>();
        }

        private PokerCardInfo? _CurrentCard;

        public PokerCardInfo CurrentCard
        {
            get { return _CurrentCard!; }
            set
            {
                if (SetProperty(ref _CurrentCard, value))
                {
                    //can decide what to do when property changes
                    int index = _thisMod.PokerList.IndexOf(this);
                    _uiCard.NewCard(index);
                }

            }
        }
        private bool _WillHold;
        public bool WillHold
        {
            get
            {
                return _WillHold;
            }

            set
            {
                if (SetProperty(ref _WillHold, value) == true)
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