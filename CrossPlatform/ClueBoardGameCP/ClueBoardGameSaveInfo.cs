using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using System.Collections.Generic;
namespace ClueBoardGameCP
{
    [SingletonGame]
    public class ClueBoardGameSaveInfo : BasicSavedBoardDiceGameClass<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, ClueBoardGamePlayerItem>
    {
        public int DiceNumber { get; set; }
        public PredictionInfo? CurrentPrediction { get; set; }
        private int _MovesLeft;
        public int MovesLeft
        {
            get { return _MovesLeft; }
            set
            {
                if (SetProperty(ref _MovesLeft, value))
                {
                    if (_thisMod != null)
                        _thisMod.LeftToMove = MovesLeft; //i think.
                }
            }
        }
        public bool AccusationMade { get; set; }
        public bool ShowedMessage { get; set; }
        public Dictionary<int, CharacterInfo> CharacterList { get; set; } = new Dictionary<int, CharacterInfo>();
        public PredictionInfo Solution { get; set; } = new PredictionInfo();
        public Dictionary<int, MoveInfo> PreviousMoves { get; set; } = new Dictionary<int, MoveInfo>();
        public Dictionary<int, WeaponInfo> WeaponList { get; set; } = new Dictionary<int, WeaponInfo>(); //needs this also.
        private EnumClueStatusList _GameStatus;
        public EnumClueStatusList GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    if (_thisMod != null)
                        _thisMod.GameStatus = value;
                }
            }
        }
        private ClueBoardGameViewModel? _thisMod;
        internal void LoadMod(ClueBoardGameViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.GameStatus = GameStatus;
        }
    }
}