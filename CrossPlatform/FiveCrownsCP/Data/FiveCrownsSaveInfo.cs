using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using FiveCrownsCP.Cards;
namespace FiveCrownsCP.Data
{
    [SingletonGame]
    public class FiveCrownsSaveInfo : BasicSavedCardClass<FiveCrownsPlayerItem, FiveCrownsCardInformation>
    { //anything needed for autoresume is here.
        private int _upTo;
        public int UpTo
        {
            get { return _upTo; }
            set
            {
                if (SetProperty(ref _upTo, value))
                {
                    //can decide what to do when property changes
                    if (_model == null)
                        return;
                    _model.UpTo = UpTo;
                }

            }
        }
        private FiveCrownsVMData? _model;
        internal void LoadMod(FiveCrownsVMData model)
        {
            _model = model;
            _model.UpTo = UpTo;
        }
        public int PlayerWentOut { get; set; }
        public bool SetsCreated { get; set; }
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
    }
}