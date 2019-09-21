using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
namespace YahtzeeHandsDownCP
{
    public class YahtzeeHandsDownCardInformation : SimpleDeckObject, IDeckObject, ICard, IComparable<YahtzeeHandsDownCardInformation>
    {
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
        private int _FirstValue;

        public int FirstValue
        {
            get { return _FirstValue; }
            set
            {
                if (SetProperty(ref _FirstValue, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _SecondValue;
        public int SecondValue
        {
            get { return _SecondValue; }
            set
            {
                if (SetProperty(ref _SecondValue, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _IsWild;
        public bool IsWild
        {
            get { return _IsWild; }
            set
            {
                if (SetProperty(ref _IsWild, value))
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
    public class ChanceCardInfo : SimpleDeckObject, IDeckObject
    {
        //not sure if it needs to implment ideck or not (?)
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
        public ChanceCardInfo()
        {
            DefaultSize = new SKSize(55, 72);
        }
        public void Populate(int chosen)
        {
            Deck = chosen;
            if (Deck <= 3)
                Points = 1;
            else if (Deck <= 6)
                Points = 2;
            else if (Deck <= 9)
                Points = 3;
            else if (Deck <= 11)
                Points = 5;
            else if (Deck == 12)
                Points = 7;
            else
                throw new BasicBlankException("Can't find the chance card.  Rethink");
        }
        public void Reset() { }
    }
    public class ComboCardInfo : SimpleDeckObject, IDeckObject
    {
        public class SampleCardClass : ICard
        {
            public int FirstValue { get; set; }
            public int SecondValue { get; set; }
            public bool IsWild { get; set; }
            public EnumColor Color { get; set; }
        }
        public int FirstSet { get; set; }
        public int NumberInStraight { get; set; }
        public int SecondSet { get; set; }
        public int Points { get; set; }
        public string FirstDescription { get; set; } = "";
        public string SecondDescription { get; set; } = "";
        public CustomBasicList<SampleCardClass> SampleList { get; set; } = new CustomBasicList<SampleCardClass>();
        public ComboCardInfo CurrentCombo => this;
        public ComboCardInfo()
        {
            DefaultSize = new SKSize(230, 130);
        }
        public void Reset() { }
        public void Populate(int chosen)
        {
            Deck = chosen;
            switch (Deck)
            {
                case 1:
                    {
                        Points = 2;
                        FirstDescription = "3 OF A KIND";
                        FirstSet = 3;
                        SecondDescription = "Same number|and same color!";
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 6 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, IsWild = true });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 6 });
                        break;
                    }

                case 2:
                    {
                        Points = 4;
                        FirstSet = 4;
                        FirstDescription = "4 OF A KIND";
                        SecondDescription = "Same number|and same color!";
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 3 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 3 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 3 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Any, FirstValue = 3 });
                        break;
                    }

                case 3:
                    {
                        Points = 5;
                        FirstSet = 3;
                        SecondSet = 2;
                        FirstDescription = "FULL HOUSE";
                        SecondDescription = "2 of a kind|PLUS 3 of a kind";
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Yellow, FirstValue = 4 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Yellow, FirstValue = 4 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 1 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 1 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 1 });
                        break;
                    }

                case 4:
                    {
                        Points = 6;
                        NumberInStraight = 4;
                        FirstDescription = "SMALL STRAIGHT";
                        SecondDescription = "4 numbers in a row,|in the same color";
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 1 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 2 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Any, FirstValue = 3 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 4 });
                        break;
                    }

                case 5:
                    {
                        Points = 8;
                        NumberInStraight = 5;
                        FirstDescription = "LARGE STRAIGHT";
                        SecondDescription = "5 numbers in a row,|in the same color";
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 1 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Any, FirstValue = 2 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 3 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 4 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Blue, FirstValue = 2, SecondValue = 5 });
                        break;
                    }

                case 6:
                    {
                        Points = 10;
                        FirstSet = 5;
                        FirstDescription = "YAHTZEE!";
                        SecondDescription = "5 of a kind, same|number and same color";
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Any, FirstValue = 2 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Yellow, FirstValue = 2, SecondValue = 5 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Yellow, FirstValue = 2 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Yellow, FirstValue = 2 });
                        SampleList.Add(new ComboCardInfo.SampleCardClass() { Color = EnumColor.Yellow, FirstValue = 2 });
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Can't find the deck of " + Deck);
                    }
            }
        }
    }
}