using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace DominosMexicanTrainCP.Data
{
    [SingletonGame]
    public class DominosMexicanTrainSaveInfo : BasicSavedDominosClass<MexicanDomino, DominosMexicanTrainPlayerItem>
    { //anything needed for autoresume is here.
        public SavedTrain? TrainData { get; set; }
        public int NewestTrain { get; set; }
        public int FirstPlayerPlayed { get; set; }
        public bool CurrentPlayerDouble { get; set; }
        private int _upTo = -1;
        public int UpTo
        {
            get { return _upTo; }
            set
            {
                if (SetProperty(ref _upTo, value))
                {
                    if (_thisMod != null)
                        _thisMod.UpTo = value;
                }
            }
        }
        private DominosMexicanTrainVMData? _thisMod;
        public void LoadMod(DominosMexicanTrainVMData thisMod)
        {
            _thisMod = thisMod;
            _thisMod.UpTo = UpTo;
        }
    }
}