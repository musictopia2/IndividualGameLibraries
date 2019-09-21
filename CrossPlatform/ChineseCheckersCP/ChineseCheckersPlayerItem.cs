using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ChineseCheckersCP
{
    public class ChineseCheckersPlayerItem : PlayerBoardGame<EnumColorList>
    {
        public override bool DidChooseColor => Color != EnumColorList.None;
        public override void Clear()
        {
            Color = EnumColorList.None;
        }
        public CustomBasicList<int> PieceList { get; set; } = new CustomBasicList<int>();
    }
}