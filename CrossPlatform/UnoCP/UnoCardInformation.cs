using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.CommonInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
namespace UnoCP
{
    public class UnoCardInformation : SimpleDeckObject, IColorCard, IPointsObject, IComparable<UnoCardInformation>
    {
        public UnoCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
        private string _Display = "";

        public string Display
        {
            get { return _Display; }
            set
            {
                if (SetProperty(ref _Display, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private EnumCardTypeList _WhichType;

        public EnumCardTypeList WhichType
        {
            get { return _WhichType; }
            set
            {
                if (SetProperty(ref _WhichType, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private int _Draw;

        public int Draw
        {
            get { return _Draw; }
            set
            {
                if (SetProperty(ref _Draw, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Number;

        public int Number
        {
            get { return _Number; }
            set
            {
                if (SetProperty(ref _Number, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private EnumColorTypes _Color;

        public EnumColorTypes Color
        {
            get { return _Color; }
            set
            {
                if (SetProperty(ref _Color, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


        public int Points { get; set; }

        public int GetPoints => Points;

        public int ReadMainValue => Number;

        public EnumColorTypes GetColor => Color;

        public void Reset()
        {
            if (WhichType == EnumCardTypeList.Wild)
                Color = EnumColorTypes.ZOther;
        }
        public override string ToString()
        {
            if (WhichType == EnumCardTypeList.Wild)
                return $"{Display} Deck: {Deck}";
            return $"{Color.ToString()} {Display} Deck: {Deck}";
        }
        public void Populate(int chosen)
        {
            for (int x = 1; x <= 4; x++)
            {
                if (chosen == x)
                {
                    Deck = x;
                    Points = 0;
                    Number = 0;
                    Display = "0"; //this is for displaying
                    WhichType = EnumCardTypeList.Regular;
                    Color = (EnumColorTypes)x;
                    return;
                }
            }
            int z = 4;

            for (int x = 1; x <= 4; x++)
            {
                for (int y = 1; y <= 12; y++)
                {
                    for (int q = 0; q < 2; q++)
                    {
                        z++;
                        if (chosen == z)
                        {
                            Deck = z;
                            Color = (EnumColorTypes)x;
                            Number = y;
                            if (y < 10)
                            {
                                Points = y;
                                Display = y.ToString();
                                WhichType = EnumCardTypeList.Regular;
                                return;
                            }
                            Points = 20;
                            switch (y)
                            {
                                case 10:
                                    WhichType = EnumCardTypeList.Draw2;
                                    Draw = 2;
                                    Display = "D2";
                                    return;
                                case 11:
                                    WhichType = EnumCardTypeList.Skip;
                                    Display = "S";
                                    return;
                                case 12:
                                    WhichType = EnumCardTypeList.Reverse;
                                    Display = "R";
                                    return;
                                default:
                                    throw new BasicBlankException("Not Supported");
                            }
                        }
                    }

                }
            }
            for (int x = 1; x <= 2; x++)
            {
                for (int y = 1; y <= 4; y++)
                {
                    z++;
                    if (chosen == z)
                    {
                        Deck = z;
                        WhichType = EnumCardTypeList.Wild;
                        Color = EnumColorTypes.ZOther;
                        Number = -4;
                        //not sure about the other part.  if needed, can put back in (?)
                        if (x == 1)
                        {
                            Draw = 0;
                            Display = "W";
                            Points = 50;
                        }
                        else
                        {
                            Draw = 4;
                            Display = "W4";
                            Points = 100;
                        }
                        return;
                    }
                }
            }
            throw new BasicBlankException($"Cannot find deck of {chosen}");
        }

        int IComparable<UnoCardInformation>.CompareTo(UnoCardInformation other)
        {

            int ID;
            ID = Color.CompareTo(other.Color);
            if (ID != 0)
                return ID;
            ID = Points.CompareTo(other.Points);
            if (ID != 0)
                return ID;
            return WhichType.CompareTo(other.WhichType);
        }
    }
}