using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace SorryCP
{
    [SingletonGame]
    public class SorrySaveInfo : BasicSavedPlainBoardGameClass<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, SorryPlayerItem>, ISavedCardList<CardInfo>
    {
        public CustomBasicList<MoveInfo> MoveList { get; set; } = new CustomBasicList<MoveInfo>();
        public EnumColorChoice OurColor { get; set; } //decided to use different enum.
        public int PreviousPiece { get; set; }
        public CustomBasicList<int> HighlightList { get; set; } = new CustomBasicList<int>();
        public int MovesMade { get; set; }
        public int SpacesLeft { get; set; }
        public int PreviousSplit { get; set; }
        private SorryViewModel? _thisMod;
        internal void LoadMod(SorryViewModel thisMod)
        {
            _thisMod = thisMod;
            if (DidDraw == false)
                _thisMod.CardDetails = "";
        }
        private bool _DidDraw;
        public bool DidDraw
        {
            get { return _DidDraw; }
            set
            {
                if (SetProperty(ref _DidDraw, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null && value == false)
                        _thisMod.CardDetails = "";
                }
            }
        }
        public CardInfo? CurrentCard { get; set; }
        public DeckRegularDict<CardInfo>? CardList { get; set; } = new DeckRegularDict<CardInfo>(); //i think.
    }
}