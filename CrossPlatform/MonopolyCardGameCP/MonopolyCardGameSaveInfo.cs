using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace MonopolyCardGameCP
{
    [SingletonGame]
    public class MonopolyCardGameSaveInfo : BasicSavedCardClass<MonopolyCardGamePlayerItem, MonopolyCardGameCardInformation>
    {
        public EnumWhatStatus GameStatus { get; set; }
        public int WhoWentOut { get; set; }
    }
}