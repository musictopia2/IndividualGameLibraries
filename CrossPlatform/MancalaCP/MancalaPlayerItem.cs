using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MancalaCP
{
    public class MancalaPlayerItem : SimplePlayer
    { //anything needed is here
        public int HowManyPiecesAtHome { get; set; }
        public CustomBasicList<PlayerPieceData> ObjectList { get; set; } = new CustomBasicList<PlayerPieceData>();
    }
}