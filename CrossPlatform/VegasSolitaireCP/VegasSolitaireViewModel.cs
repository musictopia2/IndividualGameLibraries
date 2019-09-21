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
namespace VegasSolitaireCP
{
    public class VegasSolitaireViewModel : SolitaireMainViewModel<VegasSolitaireSaveInfo>
    {
        private decimal _Money = 500; //start out with 500.

        public decimal Money
        {
            get { return _Money; }
            set
            {
                if (SetProperty(ref _Money, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public VegasSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
    }
}