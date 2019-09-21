using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace RollEmCP
{
    [SingletonGame]
    public class RollEmSaveInfo : BasicSavedDiceClass<SimpleDice, RollEmPlayerItem>
    { //anything needed for autoresume is here.
        public CustomBasicList<string> SpaceList { get; set; } = new CustomBasicList<string>();
        public EnumStatusList GameStatus { get; set; }
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
        private RollEmViewModel? _thisMod;
        internal void LoadMod(RollEmViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.Round = Round;
        }
    }
}