using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace RummyDiceCP.Data
{
    [SingletonGame]
    public class RummyDiceSaveInfo : BasicSavedGameClass<RummyDicePlayerItem>
    { //anything needed for autoresume is here.

        private readonly RummyDiceVMData _model;
        public RummyDiceSaveInfo()
        {
            _model = cons!.Resolve<RummyDiceVMData>();
        }

        public CustomBasicCollection<RummyDiceInfo> DiceList = new CustomBasicCollection<RummyDiceInfo>();
        private int _rollNumber;

        public int RollNumber
        {
            get { return _rollNumber; }
            set
            {
                if (SetProperty(ref _rollNumber, value))
                {
                    _model.RollNumber = value;

                }

            }
        }

        public bool SomeoneFinished { get; set; }
        public CustomBasicList<CustomBasicList<RummyDiceInfo>> TempSets { get; set; } = new CustomBasicList<CustomBasicList<RummyDiceInfo>>();
    }
}