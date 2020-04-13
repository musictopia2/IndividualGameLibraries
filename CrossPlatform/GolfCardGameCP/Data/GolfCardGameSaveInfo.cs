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
namespace GolfCardGameCP.Data
{
    [SingletonGame]
    public class GolfCardGameSaveInfo : BasicSavedCardClass<GolfCardGamePlayerItem, RegularSimpleCard>
    { //anything needed for autoresume is here.
        public EnumStatusType GameStatus { get; set; } //still needed here so it can communicate with the client when its next round, etc.
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
                        return;
                    _model.Round = value;
                }
            }
        }

        private GolfCardGameVMData? _model;

        internal void LoadMod(GolfCardGameVMData model)
        {
            _model = model;
            model.Round = Round;
        }
    }
}