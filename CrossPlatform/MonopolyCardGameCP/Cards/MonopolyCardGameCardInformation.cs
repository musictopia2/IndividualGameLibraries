using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.Exceptions;
using MonopolyCardGameCP.Data;
using SkiaSharp;
using System;
namespace MonopolyCardGameCP.Cards
{
    public class MonopolyCardGameCardInformation : SimpleDeckObject, IDeckObject, IComparable<MonopolyCardGameCardInformation>
    {
        public MonopolyCardGameCardInformation()
        {
            DefaultSize = new SKSize(55, 72);
        }
        private int _cardValue;
        public int CardValue
        {
            get { return _cardValue; }
            set
            {
                if (SetProperty(ref _cardValue, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public EnumCardType WhatCard { get; set; }
        public string Title { get; set; } = "";
        public int Group { get; set; }
        public decimal Money { get; set; }
        public string Description { get; set; } = "";
        public int HouseNum { get; set; }
        private int FindIndex(int chosen)
        {
            if (chosen <= 0)
                throw new Exception("The deck must be greater than 0");
            switch (chosen)
            {
                case object _ when chosen < 7:
                    {
                        return chosen;
                    }

                case object _ when chosen < 11:
                    {
                        return 7;
                    }

                case object _ when chosen < 16:
                    {
                        return 8;
                    }

                case object _ when chosen < 20:
                    {
                        return 9;
                    }

                case object _ when chosen < 23:
                    {
                        return 10;
                    }

                case object _ when chosen < 25:
                    {
                        return 11;
                    }

                case object _ when chosen < 27:
                    {
                        return 12;
                    }

                case object _ when chosen < 55:
                    {
                        return chosen - 14;
                    }

                case object _ when chosen < 57:
                    {
                        return 41;
                    }

                case object _ when chosen < 61:
                    {
                        return 42;
                    }

                default:
                    {
                        throw new BasicBlankException("Does not go this high");
                    }
            }
        }
        public void Populate(int chosen)
        {
            int indexs;
            try
            {
                indexs = FindIndex(chosen);
            }
            catch (Exception)
            {
                throw new BasicBlankException("Cannot find the card information for deck " + chosen);
            }
            Deck = chosen;
            CardValue = indexs;
            switch (indexs)
            {
                case object _ when indexs < 7:
                    {
                        WhatCard = EnumCardType.IsToken;
                        Description = "Pays off value of one color-group";
                        Money = 0;
                        Group = 0;
                        if (Deck == 1)
                            Title = "Dog";
                        else if (Deck == 2)
                            Title = "Horse";
                        else if (Deck == 3)
                            Title = "Iron";
                        else if (Deck == 4)
                            Title = "Car";
                        else if (Deck == 5)
                            Title = "Ship";
                        else if (Deck == 6)
                            Title = "Hat";
                        break;
                    }

                case 7:
                    {
                        WhatCard = EnumCardType.IsMr;
                        Money = 0;
                        Group = 0;
                        Title = "Mr. Monopoly";
                        Description = "$1,000 to the player with the most of these cards";
                        break;
                    }

                case 8:
                    {
                        WhatCard = EnumCardType.IsHouse;
                        Money = 0;
                        Group = 0;
                        Title = "1st House";
                        HouseNum = 1;
                        break;
                    }

                case 9:
                    {
                        WhatCard = EnumCardType.IsHouse;
                        Money = 0;
                        Group = 0;
                        Title = "2nd House";
                        HouseNum = 2;
                        break;
                    }

                case 10:
                    {
                        WhatCard = EnumCardType.IsHouse;
                        Money = 0;
                        Group = 0;
                        Title = "3rd House";
                        HouseNum = 3;
                        break;
                    }

                case 11:
                    {
                        WhatCard = EnumCardType.IsHouse;
                        Money = 0;
                        Group = 0;
                        Title = "4th House";
                        HouseNum = 4;
                        break;
                    }

                case 12:
                    {
                        WhatCard = EnumCardType.IsHotel;
                        Money = 0;
                        Group = 0;
                        Title = "Hotel";
                        Description = "Add $500 to value of one color-group with 4 houses";
                        break;
                    }

                case 13:
                    {
                        WhatCard = EnumCardType.IsRailRoad;
                        Money = 0;
                        Group = 0;
                        Title = "Reading Railroad";
                        Description = "PAIR: $250" + Constants.vbCrLf + "THREE:  " + "$500" + Constants.vbCrLf + "FOUR:  $1000";
                        break;
                    }

                case 14:
                    {
                        WhatCard = EnumCardType.IsRailRoad;
                        Money = 0;
                        Group = 0;
                        Title = "Pennysylvania Railroad";
                        Description = "PAIR: $250" + Constants.vbCrLf + "THREE:  " + "$500" + Constants.vbCrLf + "FOUR:  $1000";
                        break;
                    }

                case 15:
                    {
                        WhatCard = EnumCardType.IsRailRoad;
                        Money = 0;
                        Group = 0;
                        Title = "B. & O. Railroad";
                        Description = "PAIR: $250" + Constants.vbCrLf + "THREE:  " + "$500" + Constants.vbCrLf + "FOUR:  $1000";
                        break;
                    }

                case 16:
                    {
                        WhatCard = EnumCardType.IsRailRoad;
                        Money = 0;
                        Group = 0;
                        Title = "Short Line Railroad";
                        Description = "PAIR: $250" + Constants.vbCrLf + "THREE:  " + "$500" + Constants.vbCrLf + "FOUR:  $1000";
                        break;
                    }

                case 17:
                    {
                        WhatCard = EnumCardType.IsUtilities;
                        // Money = 500
                        Group = 0;
                        Description = "Pair:  $500";
                        Title = "Electric Company";
                        break;
                    }

                case 18:
                    {
                        WhatCard = EnumCardType.IsUtilities;
                        // Money = 500
                        Group = 0;
                        Description = "Pair:  $500";
                        Title = "Waterworks";
                        break;
                    }

                case 19:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 50;
                        Group = 1;
                        Description = "Set Value $50 each house adds $50 more";
                        Title = "Mediterranean Avenue";
                        break;
                    }

                case 20:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 50;
                        Group = 1;
                        Description = "Set Value $50 each house adds $50";
                        Title = "Baltic Avenue";
                        break;
                    }

                case 21:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 100;
                        Group = 2;
                        Description = "Set Value $100 each house adds $100 more";
                        Title = "Oriental Avenue";
                        break;
                    }

                case 22:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 100;
                        Group = 2;
                        Description = "Set Value $100 each house adds $100 more";
                        Title = "Vermont Avenue";
                        break;
                    }

                case 23:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 100;
                        Group = 2;
                        Description = "Set Value $100 each house adds $100 more";
                        Title = "Connecticut Avenue";
                        break;
                    }

                case 24:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 150;
                        Group = 3;
                        Description = "Set Value $150 each house adds $150 more";
                        Title = "St. Charles Place";
                        break;
                    }

                case 25:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 150;
                        Group = 3;
                        Description = "Set Value $150 each house adds $150 more";
                        Title = "States Avenue";
                        break;
                    }

                case 26:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 150;
                        Group = 3;
                        Description = "Set Value $150 each house adds $150 more";
                        Title = "Virginia Avenue";
                        break;
                    }

                case 27:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 200;
                        Group = 4;
                        Description = "Set Value $200 each house adds $200 more";
                        Title = "St. James Place";
                        break;
                    }

                case 28:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 200;
                        Group = 4;
                        Description = "Set Value $200 each house adds $200 more";
                        Title = "Tennessee Avenue";
                        break;
                    }

                case 29:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 200;
                        Group = 4;
                        Description = "Set Value $200 each house adds $200 more";
                        Title = "New York Avenue";
                        break;
                    }

                case 30:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 250;
                        Group = 5;
                        Description = "Set Value $250 each house adds $250 more";
                        Title = "Kentucky Avenue";
                        break;
                    }

                case 31:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 250;
                        Group = 5;
                        Description = "Set Value $250 each house adds $250 more";
                        Title = "Indiana Avenue";
                        break;
                    }

                case 32:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 250;
                        Group = 5;
                        Description = "Set Value $250 each house adds $250 more";
                        Title = "Illinois Avenue";
                        break;
                    }

                case 33:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 300;
                        Group = 6;
                        Description = "Set Value $300 each house adds $300 more";
                        Title = "Atlantic Avenue";
                        break;
                    }

                case 34:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 300;
                        Group = 6;
                        Description = "Set Value $300 each house adds $300 more";
                        Title = "Ventor Avenue";
                        break;
                    }

                case 35:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 300;
                        Group = 6;
                        Description = "Set Value $300 each house adds $300 more";
                        Title = "Marvin Gardens";
                        break;
                    }

                case 36:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 350;
                        Group = 7;
                        Description = "Set Value $350 each house adds $350 more";
                        Title = "Pacific Avenue";
                        break;
                    }

                case 37:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 350;
                        Group = 7;
                        Description = "Set Value $350 each house adds $350 more";
                        Title = "No. Carolina Avenue";
                        break;
                    }

                case 38:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 350;
                        Group = 7;
                        Description = "Set Value $350 each house adds $350 more";
                        Title = "Pennysylvania Avenue";
                        break;
                    }

                case 39:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 400;
                        Group = 8;
                        Description = "Set Value $400 each house adds $400 more";
                        Title = "Park Place";
                        break;
                    }

                case 40:
                    {
                        WhatCard = EnumCardType.IsProperty;
                        Money = 400;
                        Group = 8;
                        Description = "Set Value $400 each house adds $400 more";
                        Title = "Boardwalk";
                        break;
                    }

                case 41:
                    {
                        WhatCard = EnumCardType.IsChance;
                        Money = 0;
                        Group = 0;
                        Description = "Wild if you are the Lay Down Player.  Otherwise, makes your hand worthless!";
                        Title = "Chance";
                        break;
                    }

                case 42:
                    {
                        WhatCard = EnumCardType.IsGo;
                        Money = 200;
                        Group = 0;
                        Description = "COLLECT $200.00";
                        Title = "GO";
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Nothing found");
                    }
            }
        }
        public void Reset() { }
        int IComparable<MonopolyCardGameCardInformation>.CompareTo(MonopolyCardGameCardInformation other)
        {
            if (WhatCard != other.WhatCard)
                return WhatCard.CompareTo(other.WhatCard);
            if (Group != other.Group)
                return Group.CompareTo(other.Group);
            return Title.CompareTo(other.Title);
        }
    }
}
