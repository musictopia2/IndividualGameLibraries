using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace TroubleCP
{
    [SingletonGame]
    public class TroubleSaveInfo : BasicSavedBoardDiceGameClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, TroublePlayerItem>
    {
        //anything needed goes here.
        public CustomBasicList<MoveInfo> MoveList { get; set; } = new CustomBasicList<MoveInfo>();
        public EnumColorChoice OurColor { get; set; } //decided to use different enum.
        public int DiceNumber { get; set; } //hopefully this is fine too.
    }
}