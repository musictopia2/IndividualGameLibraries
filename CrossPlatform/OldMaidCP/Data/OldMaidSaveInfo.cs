using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
//i think this is the most common things i like to do
namespace OldMaidCP.Data
{
    [SingletonGame]
    public class OldMaidSaveInfo : BasicSavedCardClass<OldMaidPlayerItem, RegularSimpleCard>
    { //anything needed for autoresume is here.
        public bool RemovePairs { get; set; }
        public bool AlreadyChoseOne { get; set; }
    }
}