using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace AggravationCP
{
    [SingletonGame]
    public class AggravationSaveInfo : BasicSavedBoardDiceGameClass<EnumColorChoice, MarblePiecesCP<EnumColorChoice>, AggravationPlayerItem>
    {
        public int PreviousSpace { get; set; }
        public CustomBasicList<MoveInfo> MoveList { get; set; } = new CustomBasicList<MoveInfo>();
        public EnumColorChoice OurColor { get; set; } //decided to use different enum.
        public int DiceNumber { get; set; } //hopefully this is fine too.
    }
}