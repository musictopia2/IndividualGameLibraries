using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace TeeItUpCP
{
    [SingletonGame]
    public class TeeItUpSaveInfo : BasicSavedCardClass<TeeItUpPlayerItem, TeeItUpCardInformation>
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
        public int Begins { get; set; }
        public bool FirstMulligan { get; set; }
        public EnumStatusType GameStatus { get; set; }
        private TeeItUpViewModel? _thisMod;
        public void LoadMod(TeeItUpViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.Round = Round;
        }
    }
}