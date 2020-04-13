using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.CollectionClasses;
using FlinchCP.Cards;
namespace FlinchCP.Data
{
    public class FlinchPlayerItem : PlayerSingleHand<FlinchCardInformation>
    { //anything needed is here
        public DeckRegularDict<FlinchCardInformation> StockList { get; set; } = new DeckRegularDict<FlinchCardInformation>();

        private string _inStock = "";

        public string InStock
        {
            get { return _inStock; }
            set
            {
                if (SetProperty(ref _inStock, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _stockLeft;

        public int StockLeft
        {
            get { return _stockLeft; }
            set
            {
                if (SetProperty(ref _stockLeft, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _discard1 = "";
        public string Discard1
        {
            get
            {
                return _discard1; // for now, can do copy.  may later do something else (?)
            }

            set
            {
                if (SetProperty(ref _discard1, value) == true)
                {
                }
            }
        }

        private string _discard2 = "";
        public string Discard2
        {
            get
            {
                return _discard2;
            }

            set
            {
                if (SetProperty(ref _discard2, value) == true)
                {
                }
            }
        }

        private string _discard3 = "";
        public string Discard3
        {
            get
            {
                return _discard3;
            }

            set
            {
                if (SetProperty(ref _discard3, value) == true)
                {
                }
            }
        }

        private string _discard4 = "";
        public string Discard4
        {
            get
            {
                return _discard4;
            }

            set
            {
                if (SetProperty(ref _discard4, value) == true)
                {
                }
            }
        }
        //this is used so i can copy/paste to flinch.
        private string _discard5 = "";

        public string Discard5
        {
            get { return _discard5; }
            set
            {
                if (SetProperty(ref _discard5, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public CustomBasicList<BasicPileInfo<FlinchCardInformation>> DiscardList { get; set; } = new CustomBasicList<BasicPileInfo<FlinchCardInformation>>();
    }
}
