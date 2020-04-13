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
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
//i think this is the most common things i like to do
namespace SorryCP.Data
{
    [SingletonGame]
    public class SorrySaveInfo : BasicSavedGameClass<SorryPlayerItem>, ISavedCardList<CardInfo>
    { //anything needed for autoresume is here.
        public CustomBasicList<MoveInfo> MoveList { get; set; } = new CustomBasicList<MoveInfo>();
        public EnumColorChoice OurColor { get; set; } //decided to use different enum.
        public int PreviousPiece { get; set; }
        public CustomBasicList<int> HighlightList { get; set; } = new CustomBasicList<int>();
        public int MovesMade { get; set; }
        public int SpacesLeft { get; set; }
        public int PreviousSplit { get; set; }
        private SorryVMData? _thisMod;

        internal void LoadMod(SorryVMData thisMod)
        {
            _thisMod = thisMod;
            if (DidDraw == false)
            {
                _thisMod.CardDetails = "";
            }
            _thisMod.Instructions = Instructions;
        }
        private bool _didDraw;
        public bool DidDraw
        {
            get { return _didDraw; }
            set
            {
                if (SetProperty(ref _didDraw, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null && value == false)
                    {
                        _thisMod.CardDetails = "";
                    }
                }
            }
        }
        private string _instructions = "";

        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                    {
                        _thisMod.Instructions = value;
                    }
                }

            }
        }

        public CardInfo? CurrentCard { get; set; }
        public DeckRegularDict<CardInfo>? CardList { get; set; } = new DeckRegularDict<CardInfo>(); //i think.

    }
}