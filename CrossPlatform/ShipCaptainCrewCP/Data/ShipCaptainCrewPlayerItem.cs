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
namespace ShipCaptainCrewCP.Data
{
    public class ShipCaptainCrewPlayerItem : SimplePlayer
    { //anything needed is here
        private bool _wentOut;
        public bool WentOut
        {
            get { return _wentOut; }
            set
            {
                if (SetProperty(ref _wentOut, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _score;
        public int Score
        {
            get { return _score; }
            set
            {
                if (SetProperty(ref _score, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _wins;
        public int Wins
        {
            get { return _wins; }
            set
            {
                if (SetProperty(ref _wins, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public bool TookTurn { get; set; }
    }
}