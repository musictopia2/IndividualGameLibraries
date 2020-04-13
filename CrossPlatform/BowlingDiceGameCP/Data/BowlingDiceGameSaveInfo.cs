using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BowlingDiceGameCP.Data
{
    [SingletonGame]
    public class BowlingDiceGameSaveInfo : BasicSavedGameClass<BowlingDiceGamePlayerItem>
    { //anything needed for autoresume is here.
        private readonly BowlingDiceGameVMData _model;
        public BowlingDiceGameSaveInfo()
        {
            _model = cons!.Resolve<BowlingDiceGameVMData>();
        }
        public string DiceData { get; set; } = "";//because of how it was organized, this time, will be just dice data unfortunately.
        private bool _isExtended;
        public bool IsExtended
        {
            get { return _isExtended; }
            set
            {
                if (SetProperty(ref _isExtended, value))
                {
                    _model.IsExtended = value;
                }
            }
        }
        private int _whichPart;
        public int WhichPart
        {
            get { return _whichPart; }
            set
            {
                if (SetProperty(ref _whichPart, value))
                {
                    _model.WhichPart = value;
                }

            }
        }
        private int _whatFrame;
        public int WhatFrame
        {
            get { return _whatFrame; }
            set
            {
                if (SetProperty(ref _whatFrame, value))
                {
                    //can decide what to do when property changes
                    _model.WhatFrame = value;
                }
            }
        }
    }
}