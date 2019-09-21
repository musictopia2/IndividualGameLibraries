using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.ColorCards;
using BasicGameFramework.CommonInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
namespace Phase10CP
{
    public class Phase10CardInformation : SimpleDeckObject, IColorCard, IPointsObject
        , IComparable<Phase10CardInformation>, IRummmyObject<EnumColorTypes, EnumColorTypes>
    {
        public Phase10CardInformation()
        {
            DefaultSize = new SKSize(55, 72); //that is always default size.
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

        private EnumCardCategory _CardCategory;

        public EnumCardCategory CardCategory
        {
            get { return _CardCategory; }
            set
            {
                if (SetProperty(ref _CardCategory, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        int IRummmyObject<EnumColorTypes, EnumColorTypes>.GetSecondNumber => 0;
        int ISimpleValueObject<int>.ReadMainValue => Number;
        bool IWildObject.IsObjectWild => CardCategory == EnumCardCategory.Wild;
        bool IIgnoreObject.IsObjectIgnored => CardCategory == EnumCardCategory.Skip;
        EnumColorTypes ISuitObject<EnumColorTypes>.GetSuit => Color;
        EnumColorTypes IColorObject<EnumColorTypes>.GetColor => Color;
        int IPointsObject.GetPoints
        {
            get
            {
                if (CardCategory == EnumCardCategory.Skip)
                    return 15;
                else if (CardCategory == EnumCardCategory.Wild)
                    return 25;
                else if (Number < 10)
                    return 5;
                else
                    return 10;
            }
        }
        public void Populate(int chosen)
        {
            int x;
            int y;
            int z;
            int q;
            q = 0;
            for (x = 1; x <= 4; x++)
            {
                for (y = 1; y <= 13; y++)
                {
                    for (z = 1; z <= 2; z++)
                    {
                        q += 1;
                        if (q == chosen)
                        {
                            Color = (EnumColorTypes)x;
                            if (y < 13)
                            {
                                CardCategory = EnumCardCategory.None;
                                Number = y;
                                Display = Number.ToString();
                            }
                            else
                            {
                                CardCategory = EnumCardCategory.Wild;
                                Display = "W";
                                Number = 50;
                            }
                            Deck = chosen;
                            return;
                        }
                    }
                }
            }
            for (x = 105; x <= 108; x++)
            {
                if (x == chosen)
                {
                    CardCategory = EnumCardCategory.Skip;
                    Color = EnumColorTypes.Blue;
                    Display = "S";
                    Number = 50;
                    Deck = chosen;
                    return;
                }
            }
            throw new BasicBlankException($"Cannot find the deck {chosen}");
        }
        public void Reset()
        {
            if (CardCategory == EnumCardCategory.Wild)
            {
                Display = "W";
                Number = 0; //hopefully that works.
            }
        }
        int IComparable<Phase10CardInformation>.CompareTo(Phase10CardInformation other)
        {
            if (Number != other.Number)
                return Number.CompareTo(other.Number);
            else if (Color.Equals(other.Color) == false)
                return Color.CompareTo(other.Color);
            else
                return CardCategory.CompareTo(other.CardCategory);
        }
    }
}