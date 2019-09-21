using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace CaliforniaJackCP
{
    [SingletonGame]
    public class CaliforniaJackSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, CaliforniaJackCardInformation, CaliforniaJackPlayerItem> { }
}