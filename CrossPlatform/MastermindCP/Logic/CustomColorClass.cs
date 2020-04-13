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
using BasicGameFrameworkLibrary.ChooserClasses;
using MastermindCP.Data;
using BasicGameFrameworkLibrary.Attributes;
namespace MastermindCP.Logic
{
    //[SingletonGame]
    public class CustomColorClass : IEnumListClass<EnumColorPossibilities>
    {
        private readonly GlobalClass _global;

        public CustomColorClass(GlobalClass global)
        {
            _global = global;
        }

        CustomBasicList<EnumColorPossibilities> IEnumListClass<EnumColorPossibilities>.GetEnumList()
        {
            return _global.ColorList;
        }
    }
}
