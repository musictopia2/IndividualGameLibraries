using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace A8RoundRummyCP
{
    [SingletonGame]
    public class A8RoundRummySaveInfo : BasicSavedCardClass<A8RoundRummyPlayerItem, A8RoundRummyCardInformation>
    { //anything needed for autoresume is here.
        public CustomBasicCollection<RoundClass> RoundList { get; set; } = new CustomBasicCollection<RoundClass>();
    }
}