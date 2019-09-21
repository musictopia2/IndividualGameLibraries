using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace ThinkTwiceCP
{
    [SingletonGame]
    public class ThinkTwiceSaveInfo : BasicSavedDiceClass<SimpleDice, ThinkTwicePlayerItem>
    { //anything needed for autoresume is here.
        private int _Score;

        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    if (_thisMod != null)
                        _thisMod.Score = value;
                }

            }
        }

        private int _CategoryRolled = -1;

        public int CategoryRolled
        {
            get { return _CategoryRolled; }
            set
            {
                if (SetProperty(ref _CategoryRolled, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public int WhichMulti { get; set; } = -1;
        public bool CategoryHeld { get; set; }
        public int CategorySelected { get; set; }
        private ThinkTwiceViewModel? _thisMod; //this is needed so it can hook up.
        public void LoadMod(ThinkTwiceViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.Score = Score;
        }
    }
}