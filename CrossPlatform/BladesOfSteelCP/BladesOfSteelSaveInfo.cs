using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace BladesOfSteelCP
{
    [SingletonGame]
    public class BladesOfSteelSaveInfo : BasicSavedCardClass<BladesOfSteelPlayerItem, RegularSimpleCard>
    { //anything needed for autoresume is here.
        private string _Instructions = "";
        public string Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.Instructions = value;
                }

            }
        }

        private string _OtherPlayer = "";

        public string OtherPlayer
        {
            get { return _OtherPlayer; }
            set
            {
                if (SetProperty(ref _OtherPlayer, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.OtherPlayer = value;
                }

            }
        }

        private bool _IsFaceOff;

        public bool IsFaceOff
        {
            get { return _IsFaceOff; }
            set
            {
                if (SetProperty(ref _IsFaceOff, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.IsFaceOff = value;
                }

            }
        }
        public bool WasTie { get; set; }
        private BladesOfSteelViewModel? _thisMod;
        internal void LoadMod(BladesOfSteelViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.IsFaceOff = IsFaceOff;
            _thisMod.Instructions = Instructions;
            _thisMod.OtherPlayer = OtherPlayer;
        }
    }
}