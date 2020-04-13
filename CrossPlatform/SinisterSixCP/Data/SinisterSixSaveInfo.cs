using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace SinisterSixCP.Data
{
    [SingletonGame]
    public class SinisterSixSaveInfo : BasicSavedDiceClass<EightSidedDice, SinisterSixPlayerItem>
    { //anything needed for autoresume is here.
        private int _maxRolls;
        public int MaxRolls
        {
            get { return _maxRolls; }
            set
            {
                if (SetProperty(ref _maxRolls, value))
                {
                    //can decide what to do when property changes
                    if (_model == null)
                    {
                        return;
                    }
                    _model.MaxRolls = value;
                }
            }
        }
        private SinisterSixVMData? _model; //this is needed so it can hook up.
        internal void LoadMod(SinisterSixVMData model)
        {
            _model = model;
            _model.MaxRolls = MaxRolls;
        }
    }
}