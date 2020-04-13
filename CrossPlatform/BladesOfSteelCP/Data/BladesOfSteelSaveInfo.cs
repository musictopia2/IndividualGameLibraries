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
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
//i think this is the most common things i like to do
namespace BladesOfSteelCP.Data
{
    [SingletonGame]
    public class BladesOfSteelSaveInfo : BasicSavedCardClass<BladesOfSteelPlayerItem, RegularSimpleCard>
    { //anything needed for autoresume is here.
        private string _instructions = "";
        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                    if (_model == null)
                        return;
                    _model.Instructions = value;
                }

            }
        }

        private string _otherPlayer = "";

        public string OtherPlayer
        {
            get { return _otherPlayer; }
            set
            {
                if (SetProperty(ref _otherPlayer, value))
                {
                    //can decide what to do when property changes
                    if (_model == null)
                        return;
                    _model.OtherPlayer = value;
                }

            }
        }

        public bool IsFaceOff { get; set; }
        public bool WasTie { get; set; }
        private BladesOfSteelVMData? _model;
        internal void LoadMod(BladesOfSteelVMData model)
        {
            _model = model;
            _model.Instructions = Instructions;
            _model.OtherPlayer = OtherPlayer;
        }
    }
}