using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BlackjackCP
{
    [SingletonGame]
    public class BlackjackSaveInfo : ObservableObject
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>(); //hopefully now its okay to use decklist since we don't have globals anymore
        //anything else needed to save a game will be here.

    }
}
