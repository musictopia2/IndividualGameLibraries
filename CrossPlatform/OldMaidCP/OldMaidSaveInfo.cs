using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace OldMaidCP
{
    [SingletonGame]
    public class OldMaidSaveInfo : BasicSavedCardClass<OldMaidPlayerItem, RegularSimpleCard>
    { //anything needed for autoresume is here.
        public bool RemovePairs { get; set; }
        public bool AlreadyChoseOne { get; set; }
    }
}