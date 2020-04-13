using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using TeeItUpCP.Cards;
namespace TeeItUpCP.Data
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
                    if (_model != null)
                        _model.Round = value;
                }

            }
        }
        public int Begins { get; set; }
        public bool FirstMulligan { get; set; }
        public EnumStatusType GameStatus { get; set; }
        private TeeItUpVMData? _model;
        public void LoadMod(TeeItUpVMData model)
        {
            _model = model;
            _model.Round = Round;
        }
    }
}