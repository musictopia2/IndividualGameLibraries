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
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
//i think this is the most common things i like to do
namespace CaptiveQueensSolitaireCP
{
    public class CaptiveQueensSolitaireViewModel : SolitaireMainViewModel<CaptiveQueensSolitaireSaveInfo>
    {
        public int FirstNumber
        {
            get
            {
                return 5;
            }
        }

        public int SecondNumber
        {
            get
            {
                return 6;
            }
        }
        public CaptiveQueensSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
    }
}