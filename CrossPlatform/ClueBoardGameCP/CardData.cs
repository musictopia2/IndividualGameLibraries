using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.GameGraphicsCP.GameboardPositionHelpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
namespace ClueBoardGameCP
{
    public abstract class MainInfo : SimpleDeckObject, IDeckObject
    {
        public string Name { get; set; } = "";
        public abstract void Populate(int Chosen);
        public abstract void Reset();
    }
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
    public class CharacterInfo
    {
        public string Name { get; set; } = ""; //for now has to be repeated until i figure out what to do
        public int Player { get; set; } //this for sure needs the player associated to it.
        private EnumNameList _Piece;
        public EnumNameList Piece
        {
            get
            {
                return _Piece;
            }
            set
            {
                _Piece = value;
                if (value == EnumNameList.Green)
                    Name = "Mr. Green";
                else if (value == EnumNameList.Mustard)
                    Name = "Colonel Mustard";
                else if (value == EnumNameList.Peacock)
                    Name = "Mrs. Peacock";
                else if (value == EnumNameList.Plum)
                    Name = "Professor Plum";
                else if (value == EnumNameList.Scarlet)
                    Name = "Miss Scarlet";
                else if (value == EnumNameList.White)
                    Name = "Mrs. White";
            }
        }
        public int CurrentRoom { get; set; }
        public int Space { get; set; }
        public int PreviousRoom { get; set; }
        public int FirstSpace { get; set; }
        public int FirstNumber { get; set; }
        public ComputerInfo ComputerData = new ComputerInfo();
        public string MainColor { get; set; } = ""; //hopefully this works too.
    }
    public class CardInfo : MainInfo
    {
        public CardInfo()
        {
            DefaultSize = new SKSize(55, 72);
        }
        public EnumCardType WhatType { get; set; }
        private EnumCardValues _CardValue;
        public EnumCardValues CardValue
        {
            get { return _CardValue; }
            set
            {
                if (SetProperty(ref _CardValue, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public int Number { get; set; }
        public override void Populate(int chosen)
        {
            throw new BasicBlankException("You have to use the global function so we can get the name.");
        }
        public override void Reset() { }
    }
}