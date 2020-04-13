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
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using SkipboCP.Cards;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
//i think this is the most common things i like to do
namespace SkipboCP.Data
{
    public class SkipboPlayerItem : PlayerSingleHand<SkipboCardInformation>
    { //anything needed is here
        public CustomBasicList<BasicPileInfo<SkipboCardInformation>> DiscardList { get; set; } = new CustomBasicList<BasicPileInfo<SkipboCardInformation>>();

        //public CustomBasicList<BasicPileInfo<SkipboCardInformation>> StockList { get; set; } = new CustomBasicList<BasicPileInfo<SkipboCardInformation>>();


        public DeckRegularDict<SkipboCardInformation> StockList { get; set; } = new DeckRegularDict<SkipboCardInformation>();

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
    }
}
