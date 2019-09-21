using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommonInterfaces;
using SkiaSharp;
using System;
namespace RageCardGameCP
{
    public class RageCardGameCardInformation : SimpleDeckObject, IDeckObject, ITrickCard<EnumColor>, IComparable<RageCardGameCardInformation>
    {
        public RageCardGameCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
        public int Player { get; set; }
        public SKPoint Location { get; set; }
        private EnumColor _Color;

        public EnumColor Color
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
        private int _Value;

        public int Value
        {
            get { return _Value; }
            set
            {
                if (SetProperty(ref _Value, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private EnumSpecialType _SpecialType = EnumSpecialType.Blank;

        public EnumSpecialType SpecialType
        {
            get { return _SpecialType; }
            set
            {
                if (SetProperty(ref _SpecialType, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        int ISimpleValueObject<int>.ReadMainValue => Value;
        EnumColor ISuitObject<EnumColor>.GetSuit => Color;
        public int GetPoints
        {
            get
            {

                if (SpecialType == EnumSpecialType.Mad || SpecialType == EnumSpecialType.Bonus)
                    return Value;
                return 0;
            }
        }
        bool IWildObject.IsObjectWild => SpecialType == EnumSpecialType.Wild;
        public void Populate(int chosen)
        {
            int x;
            int y;
            int z = 0;
            for (x = 1; x <= 6; x++)
            {
                for (y = 0; y <= 15; y++)
                {
                    z += 1;
                    if (z == chosen)
                    {
                        Color = (EnumColor)x;
                        Value = y;
                        Deck = z;
                        SpecialType = EnumSpecialType.None;
                        return;
                    }
                }
            }
            for (x = 1; x <= 2; x++)
            {
                for (y = 1; y <= 4; y++)
                {
                    z += 1;
                    if (z == chosen)
                    {
                        Deck = z;
                        Value = 0;
                        SpecialType = (EnumSpecialType)x;
                        Color = EnumColor.None;
                        return;
                    }
                }
            }
            for (x = 3; x <= 5; x++)
            {
                for (y = 1; y <= 2; y++)
                {
                    z += 1;
                    if (z == chosen)
                    {
                        Deck = z;
                        // ThisCard.Value = 0
                        SpecialType = (EnumSpecialType)x;

                        if (SpecialType == EnumSpecialType.Mad)
                            Value = -5;
                        else if (SpecialType == EnumSpecialType.Bonus)
                            Value = 5;
                        else
                            Value = 0;
                        Color = EnumColor.None;
                        return;
                    }
                }
            }
            throw new Exception("Can't find the deck " + chosen);
        }
        public void Reset() { }
        object ITrickCard<EnumColor>.CloneCard()
        {
            return MemberwiseClone();
        }
        int IComparable<RageCardGameCardInformation>.CompareTo(RageCardGameCardInformation other)
        {
            if (Color != other.Color)
                return Color.CompareTo(other.Color);
            else if (Value != other.Value)
                return Value.CompareTo(other.Value);
            else
                return SpecialType.CompareTo(other.SpecialType);
        }
    }
}