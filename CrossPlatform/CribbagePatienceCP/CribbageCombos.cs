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
using CommonBasicStandardLibraries.MVVMHelpers;
//i think this is the most common things i like to do
namespace CribbagePatienceCP
{
    public class CribbageCombos : ObservableObject
    {
        private string _Description; // this is used for display so its better to show property change safe than sorry
        public string Description
        {
            get
            {
                return _Description;
            }

            set
            {
                if (SetProperty(ref _Description, value) == true)
                {
                }
            }
        }

        // Public Property HandCategory As EnumHandCategory 'needs this so it can be linked to the ui.

        // Public Property CanFromHand As Boolean 'maybe this is fine (?)

        // don't even need this.



        public int NumberNeeded { get; set; }
        public int CardsToUse { get; set; }
        public int NumberInStraight { get; set; }
        public bool IsFlush { get; set; }
        public int NumberForKind { get; set; }
        public bool IsFullHouse { get; set; }
        private int _Points; // this is used for display so its better safe than sorry
        public int Points
        {
            get
            {
                return _Points;
            }

            set
            {
                if (SetProperty(ref _Points, value) == true)
                {
                }
            }
        }



        public bool DoublePairNeeded { get; set; }
        public EnumScoreGroup Group { get; set; } = EnumScoreGroup.NoGroup;
        public EnumJackType JackStatus { get; set; } = EnumJackType.None;
    }
}
