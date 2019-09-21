using BasicGameFramework.ChooserClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace MastermindCP
{
    public class CustomColorClass : IEnumListClass<EnumColorPossibilities>
    {
        private GlobalClass? _thisGlobal;

        CustomBasicList<EnumColorPossibilities> IEnumListClass<EnumColorPossibilities>.GetEnumList()
        {
            if (_thisGlobal == null)
                _thisGlobal = Resolve<GlobalClass>();
            return _thisGlobal.ColorList;
        }
    }
}