using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Collections.Generic;
namespace ClueBoardGameCP
{
    public class ComputerInfo
    {
        public CustomBasicList<GivenInfo> CluesGiven = new CustomBasicList<GivenInfo>(); // i think listof is fine for this
        public CustomBasicList<ReceivedInfo> CluesReceived = new CustomBasicList<ReceivedInfo>();
        public string RoomHeaded = "";
        public string Weapon = ""; // this is the weapon the computer thinks it is
        public string Character = ""; // this is the character the computer thinks it is
    }
    public class GivenInfo
    {
        public int Player; // this is who you gave the clue to
        public string Clue = ""; // this will be the string
    }
    public class ReceivedInfo
    {
        public string Name = "";
        public bool PlayerOwned; // default will be false
    }
    public class DetectiveInfo : ObservableObject
    {
        private string _Name = "";
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (SetProperty(ref _Name, value) == true)
                {
                }
            }
        }
        private EnumCardType _Category;
        public EnumCardType Category
        {
            get
            {
                return _Category;
            }
            set
            {
                if (SetProperty(ref _Category, value) == true)
                {
                }
            }
        }
        private bool _IsChecked;
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (SetProperty(ref _IsChecked, value) == true)
                {
                }
            }
        }
    }
    public class PredictionInfo
    {
        public string RoomName { get; set; } = "";
        public string WeaponName { get; set; } = "";
        public string CharacterName { get; set; } = ""; // not sure if it needs to be bindable (?)
    }
    public class FieldInfo
    {
        public Dictionary<int, MoveInfo> Neighbors = new Dictionary<int, MoveInfo>(); // i think its a list of moves (well see)
    }
    public class MoveInfo
    {

        public int SpaceNumber; // if filled out, then this is the space number
        public int RoomNumber; // if filled out, then this is the room
        public EnumPositionInfo Position;
    }
}