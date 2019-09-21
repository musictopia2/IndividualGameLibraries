using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace DummyRummyCP
{
    [SingletonGame]
    public class DummyRummySaveInfo : BasicSavedCardClass<DummyRummyPlayerItem, RegularRummyCard>
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
        private DummyRummyViewModel? _thisMod;
        internal void LoadMod(DummyRummyViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.UpTo = UpTo;
        }

        public int PlayerWentOut { get; set; }
        public bool SetsCreated { get; set; }
        public int PointsObtained { get; set; } //i think this was needed too.
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
    }
}