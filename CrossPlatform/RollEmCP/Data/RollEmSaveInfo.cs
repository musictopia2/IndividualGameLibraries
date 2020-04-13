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
using BasicGameFrameworkLibrary.Dice;
//i think this is the most common things i like to do
namespace RollEmCP.Data
{
    [SingletonGame]
    public class RollEmSaveInfo : BasicSavedDiceClass<SimpleDice, RollEmPlayerItem>
    { //anything needed for autoresume is here.
        public CustomBasicList<string> SpaceList { get; set; } = new CustomBasicList<string>();
        public EnumStatusList GameStatus { get; set; }
        private int _round;
        public int Round
        {
            get { return _round; }
            set
            {
                if (SetProperty(ref _round, value))
                {
                    //can decide what to do when property changes
                    if (_model == null)
                    {
                        return;
                    }
                    _model.Round = value;
                }
            }
        }
        private RollEmVMData? _model;
        internal void LoadMod(RollEmVMData model)
        {
            _model = model;
            _model.Round = Round;
        }
    }
}