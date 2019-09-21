using BasicGameFramework.Attributes;
using BasicGameFramework.ColorCards;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace UnoCP
{
    [SingletonGame]
    public class UnoSaveInfo : BasicSavedCardClass<UnoPlayerItem, UnoCardInformation>
    { //anything needed for autoresume is here.
        public bool HasDrawn { get; set; }
        public bool HasSkipped { get; set; }
        public EnumGameStatus GameStatus { get; set; } = EnumGameStatus.NormalPlay;
        public EnumColorTypes CurrentColor { get; set; }
    }
}