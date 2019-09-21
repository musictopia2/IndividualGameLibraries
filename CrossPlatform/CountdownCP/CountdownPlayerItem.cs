using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace CountdownCP
{
    public class CountdownPlayerItem : SimplePlayer
    { //anything needed is here
        public CustomBasicList<SimpleNumber> NumberList { get; set; } = new CustomBasicList<SimpleNumber>();
    }
}