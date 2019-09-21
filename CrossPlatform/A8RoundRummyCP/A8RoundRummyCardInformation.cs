using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using SkiaSharp;
using System;
namespace A8RoundRummyCP
{
    public class A8RoundRummyCardInformation : SimpleDeckObject, IDeckObject, IComparable<A8RoundRummyCardInformation>
    {
        public A8RoundRummyCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
        private EnumCardType _CardType = EnumCardType.None;
        public EnumCardType CardType
        {
            get
            {
                return _CardType;
            }

            set
            {
                if (SetProperty(ref _CardType, value) == true)
                {
                }
            }
        }
        private EnumCardShape _Shape = EnumCardShape.Blank;
        public EnumCardShape Shape
        {
            get
            {
                return _Shape;
            }

            set
            {
                if (SetProperty(ref _Shape, value) == true)
                {
                }
            }
        }
        private int _Value;
        public int Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (SetProperty(ref _Value, value) == true)
                {
                }
            }
        }
        private EnumColor _Color = EnumColor.Blank;
        public EnumColor Color
        {
            get
            {
                return _Color;
            }

            set
            {
                if (SetProperty(ref _Color, value) == true)
                {
                }
            }
        }
        public void Populate(int chosen)
        {
            int x;
            int y;
            int q;
            int r;
            int z = 0;
            for (x = 1; x <= 3; x++) // shapes
            {
                for (y = 1; y <= 7; y++) // numbers
                {
                    for (r = 1; r <= 2; r++)
                    {
                        for (q = 1; q <= 2; q++) // how many of each
                        {
                            z += 1;
                            if (z == chosen)
                            {
                                Deck = chosen;
                                CardType = EnumCardType.Regular;
                                Shape = (EnumCardShape)x;
                                Value = y;
                                Color = (EnumColor)r;
                                return;
                            }
                        }
                    }
                }
            }
            for (x = 85; x <= 96; x++)
            {
                if (x == chosen)
                {
                    Deck = chosen;
                    CardType = EnumCardType.Wild;
                    return;
                }
            }
            for (x = 97; x <= 100; x++)
            {
                if (x == chosen)
                {
                    Deck = chosen;
                    CardType = EnumCardType.Reverse;
                    return;
                }
            }
            throw new Exception("Can't find the deck " + chosen);
        }
        public void Reset() { }
        int IComparable<A8RoundRummyCardInformation>.CompareTo(A8RoundRummyCardInformation other)
        {
            if (CardType != other.CardType)
                return CardType.CompareTo(other.CardType);
            if (Color != other.Color)
                return Color.CompareTo(other.Color);
            if (Shape != other.Shape)
                return Shape.CompareTo(other.Shape);
            return Value.CompareTo(other.Value);
        }
    }
}