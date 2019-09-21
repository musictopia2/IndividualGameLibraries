using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace BackgammonCP
{
    [SingletonGame]
    public class BackgammonSaveInfo : BasicSavedBoardDiceGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, BackgammonPlayerItem>
    {
        private BackgammonViewModel? _thisMod;
        public int SpaceHighlighted { get; set; } = -1;
        public int NumberUsed { get; set; }
        public int ComputerSpaceTo { get; set; }
        public bool MadeAtLeastOneMove { get; set; }
        public EnumGameStatus GameStatus { get; set; }
        private int _MovesMade;
        public int MovesMade
        {
            get { return _MovesMade; }
            set
            {
                if (SetProperty(ref _MovesMade, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.MovesMade = value;
                }

            }
        }
        public void LoadMod(BackgammonViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.MovesMade = MovesMade;
        }
    }
}