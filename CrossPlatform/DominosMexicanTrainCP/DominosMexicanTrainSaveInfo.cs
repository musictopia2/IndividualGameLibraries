using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace DominosMexicanTrainCP
{
    [SingletonGame]
    public class DominosMexicanTrainSaveInfo : BasicSavedDominosClass<MexicanDomino, DominosMexicanTrainPlayerItem>
    {
        public SavedTrain? TrainData { get; set; }
        public int NewestTrain { get; set; }
        public int FirstPlayerPlayed { get; set; }
        public bool CurrentPlayerDouble { get; set; }
        private int _UpTo = -1;
        public int UpTo
        {
            get { return _UpTo; }
            set
            {
                if (SetProperty(ref _UpTo, value))
                {
                    if (_thisMod != null)
                        _thisMod.UpTo = value;
                }
            }
        }
        private DominosMexicanTrainViewModel? _thisMod;
        public void LoadMod(DominosMexicanTrainViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.UpTo = UpTo;
        }
    }
}