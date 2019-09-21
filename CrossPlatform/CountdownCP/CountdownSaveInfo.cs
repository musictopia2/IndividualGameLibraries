using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace CountdownCP
{
    [SingletonGame]
    public class CountdownSaveInfo : BasicSavedDiceClass<CountdownDice, CountdownPlayerItem>
    { //anything needed for autoresume is here.
        private int _Round;

        public int Round
        {
            get { return _Round; }
            set
            {
                if (SetProperty(ref _Round, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.Round = value;
                }

            }
        }
        public CustomBasicList<int> HintList { get; set; } = new CustomBasicList<int>();

        private CountdownViewModel? _thisMod; //this is needed so it can hook up.
        public void LoadMod(CountdownViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.Round = Round;
        }
    }
}