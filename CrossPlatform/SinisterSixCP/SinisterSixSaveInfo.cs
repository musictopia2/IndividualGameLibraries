using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace SinisterSixCP
{
    [SingletonGame]
    public class SinisterSixSaveInfo : BasicSavedDiceClass<EightSidedDice, SinisterSixPlayerItem>
    { //anything needed for autoresume is here.
        private int _MaxRolls;
        public int MaxRolls
        {
            get { return _MaxRolls; }
            set
            {
                if (SetProperty(ref _MaxRolls, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.MaxRolls = value;
                }
            }
        }
        private SinisterSixViewModel? _thisMod; //this is needed so it can hook up.
        public void LoadMod(SinisterSixViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.MaxRolls = MaxRolls;
        }
    }
}