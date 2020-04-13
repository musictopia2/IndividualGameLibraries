using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using Newtonsoft.Json;
namespace BackgammonCP.Data
{
    public class BackgammonPlayerItem : PlayerBoardGame<EnumColorChoice>
    { //anything needed is here
        [JsonIgnore]
        public override bool DidChooseColor => Color != EnumColorChoice.None;

        public override void Clear()
        {
            Color = EnumColorChoice.None;
        }
        public BackgammonPlayerDetails? StartTurnData { get; set; }
        public BackgammonPlayerDetails? CurrentTurnData { get; set; }
    }
}