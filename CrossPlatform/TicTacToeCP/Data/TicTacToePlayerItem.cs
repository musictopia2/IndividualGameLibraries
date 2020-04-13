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
//i think this is the most common things i like to do
namespace TicTacToeCP.Data
{
    public class TicTacToePlayerItem : SimplePlayer
    { //anything needed is here
        private EnumSpaceType _piece;

        public EnumSpaceType Piece
        {
            get { return _piece; }
            set
            {
                if (SetProperty(ref _piece, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}
