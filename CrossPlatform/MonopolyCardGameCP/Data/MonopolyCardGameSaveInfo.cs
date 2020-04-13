using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using MonopolyCardGameCP.Cards;
namespace MonopolyCardGameCP.Data
{
    [SingletonGame]
    public class MonopolyCardGameSaveInfo : BasicSavedCardClass<MonopolyCardGamePlayerItem, MonopolyCardGameCardInformation>
    { //anything needed for autoresume is here.
        public EnumWhatStatus GameStatus { get; set; }
        public int WhoWentOut { get; set; }
    }
}