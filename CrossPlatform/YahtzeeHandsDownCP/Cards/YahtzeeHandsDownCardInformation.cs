using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
using YahtzeeHandsDownCP.Data;
namespace YahtzeeHandsDownCP.Cards
{
    public class YahtzeeHandsDownCardInformation : SimpleDeckObject, IDeckObject, ICard, IComparable<YahtzeeHandsDownCardInformation>
    {
        private EnumColor _color;
        public EnumColor Color
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
        private int _firstValue;

        public int FirstValue
        {
            get { return _firstValue; }
            set
            {
                if (SetProperty(ref _firstValue, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _secondValue;
        public int SecondValue
        {
            get { return _secondValue; }
            set
            {
                if (SetProperty(ref _secondValue, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _isWild;
        public bool IsWild
        {
            get { return _isWild; }
            set
            {
                if (SetProperty(ref _isWild, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public YahtzeeHandsDownCardInformation()
        {
            DefaultSize = new SKSize(55, 72);
        }
        public void Populate(int chosen)
        {
            Deck = chosen;
            int x;
            int y;
            int t;
            int z = 0;
            int ups;
            for (y = 1; y <= 3; y++) // 3 possible colors
            {
                for (x = 1; x <= 6; x++) // for the regular numbers
                {
                    if (x <= 3)
                        ups = 3;
                    else
                        ups = 2;
                    var loopTo = ups;
                    for (t = 1; t <= loopTo; t++)
                    {
                        z += 1;
                        if (z == Deck)
                        {
                            FirstValue = x;
                            Color = (EnumColor)y;
                            return;
                        }
                    }
                }
                for (x = 1; x <= 3; x++) // this is for the combo numbers
                {
                    for (t = 1; t <= 2; t++) // this is the frequency
                    {
                        z += 1;
                        if (z == Deck)
                        {
                            Color = (EnumColor)y;
                            switch (x)
                            {
                                case 1:
                                    {
                                        FirstValue = 1;
                                        SecondValue = 6;
                                        break;
                                    }

                                case 2:
                                    {
                                        FirstValue = 2;
                                        SecondValue = 5;
                                        break;
                                    }

                                case 3:
                                    {
                                        FirstValue = 3;
                                        SecondValue = 4;
                                        break;
                                    }

                                default:
                                    {
                                        throw new BasicBlankException("Nothing found");
                                    }
                            }
                            return;
                        }
                    }
                }
                for (x = 1; x <= 3; x++) // for the wilds
                {
                    z += 1;
                    if (z == Deck)
                    {
                        IsWild = true;
                        Color = (EnumColor)y;
                        return;
                    }
                }
            }
            for (x = 1; x <= 6; x++) // any ones
            {
                for (y = 1; y <= 2; y++)
                {
                    // for the anys
                    z += 1;
                    if (z == Deck)
                    {
                        Color = EnumColor.Any;
                        FirstValue = x;
                        return;
                    }
                }
            }
            throw new Exception("Can't find the deck " + Deck);
        }
        public void Reset() { }
        int IComparable<YahtzeeHandsDownCardInformation>.CompareTo(YahtzeeHandsDownCardInformation other)
        {
            if (Color != other.Color)
                return Color.CompareTo(other.Color);
            if (FirstValue != other.FirstValue)
                return FirstValue.CompareTo(other.FirstValue);
            if (SecondValue != other.SecondValue)
                return SecondValue.CompareTo(other.SecondValue);
            return IsWild.CompareTo(other.IsWild);
        }
    }
}
