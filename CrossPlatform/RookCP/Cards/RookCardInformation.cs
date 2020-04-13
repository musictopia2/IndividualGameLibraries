using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.ColorCards;
using SkiaSharp;
using System;
namespace RookCP.Cards
{
    public class RookCardInformation : SimpleDeckObject, IDeckObject, ITrickCard<EnumColorTypes>, IColorCard, IComparable<RookCardInformation>
    {


        public int Player { get; set; } //i don't think this needs binding.
        private SKPoint _location;

        public SKPoint Location
        {
            get { return _location; }
            set
            {
                if (SetProperty(ref _location, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private int _points;

        public int Points
        {
            get { return _points; }
            set
            {
                if (SetProperty(ref _points, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _display = "";

        public string Display
        {
            get { return _display; }
            set
            {
                if (SetProperty(ref _display, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public RookCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //that is always default size.
        }

        private EnumColorTypes _color;

        public EnumColorTypes Color
        {
            get { return _color; }
            set
            {
                if (SetProperty(ref _color, value))
                {
                    //can decide what to do when property changes
                }

            }
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
                    Display = CardValue.ToString();
                }

            }
        }

        private bool _isDummy;

        public bool IsDummy
        {
            get { return _isDummy; }
            set
            {
                if (SetProperty(ref _isDummy, value))
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