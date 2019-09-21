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
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace HitTheDeckCP
{
    public class HitTheDeckPlayerItem : PlayerSingleHand<HitTheDeckCardInformation>
    { //anything needed is here
        private int _PreviousPoints;

        public int PreviousPoints
        {
            get { return _PreviousPoints; }
            set
            {
                if (SetProperty(ref _PreviousPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _TotalPoints;

        public int TotalPoints
        {
            get { return _TotalPoints; }
            set
            {
                if (SetProperty(ref _TotalPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}