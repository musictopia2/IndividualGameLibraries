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
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Attributes;
using MastermindCP.Data;
using MastermindCP.Logic;
//i think this is the most common things i like to do
namespace MastermindCP.ViewModels
{
    [InstanceGame]
    public class SolutionViewModel : Screen, IMainScreen
    {
        public CustomBasicList<Bead> SolutionList = new CustomBasicList<Bead>();
        public SolutionViewModel(GlobalClass global)
        {
            if (global.Solution == null)
            {
                throw new BasicBlankException("There is no solution found.  Rethink");
            }
            SolutionList = global.Solution;
        }
    }
}
