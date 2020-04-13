using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using Newtonsoft.Json;
namespace TroubleCP.Data
{
    public class TroublePlayerItem : PlayerBoardGame<EnumColorChoice>
    { //anything needed is here
        [JsonIgnore]
        public override bool DidChooseColor => Color != EnumColorChoice.None;

        public override void Clear()
        {
            Color = EnumColorChoice.None;
        }
        public CustomBasicList<int> PieceList { get; set; } = new CustomBasicList<int>();
    }
}