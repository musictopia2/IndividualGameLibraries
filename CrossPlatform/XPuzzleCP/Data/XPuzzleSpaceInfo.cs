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
using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace XPuzzleCP.Data
{
    public class XPuzzleSpaceInfo : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; }

        private string _text = "";
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                if (SetProperty(ref _text, value) == true)
                {
                }
            }
        }

        private string _color = cs.Transparent; //now we use string.
        public string Color
        {
            get
            {
                return _color;
            }

            set
            {
                if (SetProperty(ref _color, value) == true)
                {
                }
            }
        }

        public void ClearSpace()
        {
            Color = cs.Transparent;
            Text = "";
        }

        public bool IsFilled()
        {
            return !string.IsNullOrWhiteSpace(Text);
        }
    }
}
