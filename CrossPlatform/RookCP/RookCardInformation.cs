using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.ColorCards;
using SkiaSharp;
using System;
namespace RookCP
{
    public class RookCardInformation : SimpleDeckObject, IDeckObject, ITrickCard<EnumColorTypes>, IColorCard, IComparable<RookCardInformation>
    {


        public int Player { get; set; } //i don't think this needs binding.
        private SKPoint _Location;

        public SKPoint Location
        {
            get { return _Location; }
            set
            {
                if (SetProperty(ref _Location, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private int _Points;

        public int Points
        {
            get { return _Points; }
            set
            {
                if (SetProperty(ref _Points, value))
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

        public RookCardInformation()
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

        private int _CardValue;

        public int CardValue
        {
            get { return _CardValue; }
            set
            {
                if (SetProperty(ref _CardValue, value))
                {
                    //can decide what to do when property changes
                    Display = CardValue.ToString();
                }

            }
        }

        private bool _IsDummy;

        public bool IsDummy
        {
            get { return _IsDummy; }
            set
            {
                if (SetProperty(ref _IsDummy, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public int ReadMainValue => CardValue;

        public EnumColorTypes GetColor => Color;

        public EnumColorTypes GetSuit => Color;

        public int GetPoints => Points;

        public bool IsObjectWild => false; //this has no wilds.

        public object CloneCard()
        {
            return MemberwiseClone(); //hopefully this simple (?)
        }


        public void Populate(int chosen)
        {
            Deck = chosen;
            int x;
            int y;
            int z = 0;
            for (x = 1; x <= 4; x++)
            {
                for (y = 4; y <= 14; y++)
                {
                    z += 1;
                    if (z == Deck)
                    {
                        Color = (EnumColorTypes)x;
                        CardValue = y;
                        if (y == 5)
                            Points = 5;
                        else if ((y == 10) | (y == 14))
                            Points = 10;
                        else
                            Points = 0;
                        return;
                    }
                }
            }
            throw new Exception("Sorry; cannot find the deck " + Deck);

        }

        public void Reset()
        {
            //anything that is needed to reset.
        }

        int IComparable<RookCardInformation>.CompareTo(RookCardInformation other)
        {
            if (Color != other.Color)
                return Color.CompareTo(other.Color);
            return CardValue.CompareTo(other.CardValue);
        }
    }
}