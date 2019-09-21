using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace BackgammonCP
{
    public class BackgammonPlayerItem : PlayerBoardGame<EnumColorChoice>
    {
        public override bool DidChooseColor => Color != EnumColorChoice.None;

        public override void Clear()
        {
            Color = EnumColorChoice.None;
        }
        public BackgammonPlayerDetails? StartTurnData { get; set; }
        public BackgammonPlayerDetails? CurrentTurnData { get; set; }
    }
}