using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace DutchBlitzCP
{
    [SingletonGame]
    public class DutchBlitzSaveInfo : BasicSavedCardClass<DutchBlitzPlayerItem, DutchBlitzCardInformation> { }
}