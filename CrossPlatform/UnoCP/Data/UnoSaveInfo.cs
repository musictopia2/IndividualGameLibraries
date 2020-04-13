using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using UnoCP.Cards;
namespace UnoCP.Data
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