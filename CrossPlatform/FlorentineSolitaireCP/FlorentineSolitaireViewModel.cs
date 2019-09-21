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
namespace FlorentineSolitaireCP
{
    public class FlorentineSolitaireViewModel : SolitaireMainViewModel<FlorentineSolitaireSaveInfo>
    {
        private int _StartingNumber;

        public int StartingNumber
        {
            get { return _StartingNumber; }
            set
            {
                if (SetProperty(ref _StartingNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        protected override void CommandExecutingChanged()
        {
            StartingNumber = MainPiles1!.StartNumber();
        }
        public FlorentineSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
    }
}