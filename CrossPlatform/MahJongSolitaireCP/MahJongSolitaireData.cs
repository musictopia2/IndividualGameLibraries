using BaseMahjongTilesCP;
using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace MahJongSolitaireCP
{

    public class BoardInfo : ObservableObject
    {
        public enum EnumBoardCategory
        {
            FarLeft = 1,
            Regular = 2,
            FarRight = 3,
            VeryTop = 4
        }
        public int Floor { get; set; }
        public int RowStart { get; set; }
        public int HowManyColumns { get; set; }
        public int FrontTaken { get; set; }
        public int BackTaken { get; set; }
        public bool Enabled { get; set; } = false; // i think this is needed as well
        private DeckObservableDict<MahjongSolitaireTileInfo> _TileList = new DeckObservableDict<MahjongSolitaireTileInfo>();

        public DeckObservableDict<MahjongSolitaireTileInfo> TileList
        {
            get { return _TileList; }
            set
            {
                if (SetProperty(ref _TileList, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public EnumBoardCategory BoardCategory { get; set; } = EnumBoardCategory.Regular; // most are regular
        public int ColumnStart { get; set; }
    }
}