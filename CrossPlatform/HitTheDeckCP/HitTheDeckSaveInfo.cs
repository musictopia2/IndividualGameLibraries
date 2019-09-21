using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace HitTheDeckCP
{
    [SingletonGame]
    public class HitTheDeckSaveInfo : BasicSavedCardClass<HitTheDeckPlayerItem, HitTheDeckCardInformation>
    { //anything needed for autoresume is here.
        public bool HasDrawn { get; set; }
        public bool WasFlipped { get; set; }
        private string _NextPlayer = "";
        public string NextPlayer
        {
            get { return _NextPlayer; }
            set
            {
                if (SetProperty(ref _NextPlayer, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.NextPlayer = value;
                }
            }
        }
        private HitTheDeckViewModel? _thisMod;
        public void LoadMod(HitTheDeckViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.NextPlayer = NextPlayer;
        }
    }
}