using CommonBasicStandardLibraries.CollectionClasses;
namespace MonopolyCardGameCP
{
    public class ListInfo // i think this is still needed (?)
    {
        public EnumCardType WhatCard;
        public int NumberOfHouses;
        public bool HasHotel;
        public int Group;
        public int RailRoads;
        public int ID;
    }
    public class SendTrade
    {
        public CustomBasicList<int> CardList = new CustomBasicList<int>();
        public int Player;
    }
}