using BasicGameFrameworkLibrary.GameGraphicsCP.GameboardPositionHelpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;

namespace ClueBoardGameCP.Cards
{
    public class RoomInfo : MainInfo
    {
        public GameSpace? Space { get; set; }
        public int RoomPassage { get; set; }
        public CustomBasicList<int> DoorList { get; set; } = new CustomBasicList<int>(); //this will list all the doors that you can go through to get in/out of the room
        public override void Populate(int chosen)
        {
            throw new BasicBlankException("I don't think we need to implement populate for RoomInfo.  If I am wrong, rethink");
        }
        public override void Reset() { }
    }
}