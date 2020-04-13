using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace ThinkTwiceCP.Data
{
    [SingletonGame]
    public class ThinkTwiceSaveInfo : BasicSavedDiceClass<SimpleDice, ThinkTwicePlayerItem>
    { //anything needed for autoresume is here.
        private int _score;

        public int Score
        {
            get { return _score; }
            set
            {
                if (SetProperty(ref _score, value))
                {
                    if (_model != null)
                    {
                        _model.Score = value;
                    }
                }

            }
        }

        private int _categoryRolled = -1;

        public int CategoryRolled
        {
            get { return _categoryRolled; }
            set
            {
                if (SetProperty(ref _categoryRolled, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public int WhichMulti { get; set; } = -1;
        public bool CategoryHeld { get; set; }
        public int CategorySelected { get; set; }
        private ThinkTwiceVMData? _model; //this is needed so it can hook up.
        internal void LoadMod(ThinkTwiceVMData model)
        {
            _model = model;
            _model.Score = Score;
        }
    }
}