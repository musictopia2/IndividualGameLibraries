using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace BowlingDiceGameCP
{
    [SingletonGame]
    public class BowlingDiceGameSaveInfo : BasicSavedGameClass<BowlingDiceGamePlayerItem>
    { //anything needed for autoresume is here.
        public string DiceData { get; set; } = "";//because of how it was organized, this time, will be just dice data unfortunately.
        private BowlingDiceGameViewModel? _thisMod; //this is needed so it can hook up.
        public void LoadMod(BowlingDiceGameViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.IsExtended = IsExtended;
            thisMod.WhatFrame = WhatFrame;
            thisMod.WhichPart = WhichPart;
        }
        private bool _IsExtended;
        public bool IsExtended
        {
            get { return _IsExtended; }
            set
            {
                if (SetProperty(ref _IsExtended, value))
                {
                    if (_thisMod == null)
                        return;
                    _thisMod.IsExtended = value;
                }
            }
        }
        private int _WhichPart;
        public int WhichPart
        {
            get { return _WhichPart; }
            set
            {
                if (SetProperty(ref _WhichPart, value))
                {
                    if (_thisMod == null)
                        return;
                    _thisMod.WhichPart = value;
                }

            }
        }
        private int _WhatFrame;
        public int WhatFrame
        {
            get { return _WhatFrame; }
            set
            {
                if (SetProperty(ref _WhatFrame, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.WhatFrame = value;
                }
            }
        }
    }
}