using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace FiveCrownsCP
{
    [SingletonGame]
    public class FiveCrownsSaveInfo : BasicSavedCardClass<FiveCrownsPlayerItem, FiveCrownsCardInformation>
    { //anything needed for autoresume is here.
        private int _UpTo;
        public int UpTo
        {
            get { return _UpTo; }
            set
            {
                if (SetProperty(ref _UpTo, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.UpTo = UpTo;
                }

            }
        }
        private FiveCrownsViewModel? _thisMod;
        internal void LoadMod(FiveCrownsViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.UpTo = UpTo;
        }
        public int PlayerWentOut { get; set; }
        public bool SetsCreated { get; set; }
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
    }
}