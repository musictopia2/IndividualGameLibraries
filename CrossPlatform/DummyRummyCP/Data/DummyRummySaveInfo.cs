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
namespace DummyRummyCP.Data
{
    [SingletonGame]
    public class DummyRummySaveInfo : BasicSavedCardClass<DummyRummyPlayerItem, RegularRummyCard>
    { //anything needed for autoresume is here.
        private int _upTo;

        public int UpTo
        {
            get { return _upTo; }
            set
            {
                if (SetProperty(ref _upTo, value))
                {
                    //can decide what to do when property changes
                    if (_model == null)
                        return;
                    _model.UpTo = UpTo;
                }

            }
        }
        private DummyRummyVMData? _model;
        internal void LoadMod(DummyRummyVMData model)
        {
            _model = model;
            _model.UpTo = UpTo;
        }

        public int PlayerWentOut { get; set; }
        public bool SetsCreated { get; set; }
        public int PointsObtained { get; set; } //i think this was needed too.
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
    }
}