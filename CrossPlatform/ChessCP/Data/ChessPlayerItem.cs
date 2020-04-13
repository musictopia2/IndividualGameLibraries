using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using Newtonsoft.Json;
namespace ChessCP.Data
{
    public class ChessPlayerItem : PlayerBoardGame<EnumColorChoice>
    { //anything needed is here
        [JsonIgnore]
        public override bool DidChooseColor => Color != EnumColorChoice.None;

        public override void Clear()
        {
            Color = EnumColorChoice.None;
        }
        public CustomBasicList<PlayerSpace> CurrentPieceList { get; set; } = new CustomBasicList<PlayerSpace>();
        public CustomBasicList<PlayerSpace> StartPieceList { get; set; } = new CustomBasicList<PlayerSpace>();
        public bool PossibleTie { get; set; }
    }
}