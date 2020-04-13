using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using YahtzeeHandsDownCP.Data;

namespace YahtzeeHandsDownCP.Cards
{
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
