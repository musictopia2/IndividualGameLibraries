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
using BasicGameFrameworkLibrary.MiscProcesses;
using XPuzzleCP.Logic;
//i think this is the most common things i like to do
namespace XPuzzleCP.Data
{
    [SingletonGame]
    public class XPuzzleSaveInfo : ObservableObject, IMappable
    {
        private Vector _previousOpen; //has to make it a vector now.

        public Vector PreviousOpen
        {
            get { return _previousOpen; }
            set
            {
                if (SetProperty(ref _previousOpen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public XPuzzleCollection SpaceList = new XPuzzleCollection();
    }
}