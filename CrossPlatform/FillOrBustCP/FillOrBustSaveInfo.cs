using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace FillOrBustCP
{
    [SingletonGame]
    public class FillOrBustSaveInfo : BasicSavedCardClass<FillOrBustPlayerItem, FillOrBustCardInformation>
    { //anything needed for autoresume is here.
        private FillOrBustViewModel? _thisMod; //this is needed so it can hook up.
        public int FillsRequired { get; set; }
        public EnumGameStatusList GameStatus { get; set; }
        private int _TempScore;
        public int TempScore
        {
            get { return _TempScore; }
            set
            {
                if (SetProperty(ref _TempScore, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.TempScore = value;
                }

            }
        }
        private int _DiceScore;
        public int DiceScore
        {
            get { return _DiceScore; }
            set
            {
                if (SetProperty(ref _DiceScore, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.DiceScore = value;
                }

            }
        }

        public DiceList<SimpleDice> DiceList = new DiceList<SimpleDice>();
        public void LoadMod(FillOrBustViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.DiceScore = DiceScore;
            thisMod.TempScore = TempScore;
        }
    }
}