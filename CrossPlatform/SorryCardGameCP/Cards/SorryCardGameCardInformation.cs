using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SorryCardGameCP.Data;
using System;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;

namespace SorryCardGameCP.Cards
{
    public class SorryCardGameCardInformation : SimpleDeckObject, IDeckObject, IComparable<SorryCardGameCardInformation>
    {
        private EnumCategory _category = EnumCategory.Blank;
        public EnumCategory Category
        {
            get { return _category; }
            set
            {
                if (SetProperty(ref _category, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumSorry _sorry = EnumSorry.Blank;
        public EnumSorry Sorry
        {
            get { return _sorry; }
            set
            {
                if (SetProperty(ref _sorry, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                if (SetProperty(ref _value, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _color = cs.Transparent;
        public string Color
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
        public SorryCardGameCardInformation()
        {
            DefaultSize = new SKSize(66, 80);
        }
        public void Populate(int chosen)
        {
            Deck = chosen;
            Sorry = EnumSorry.None;
            if (chosen <= 5)
            {
                Value = 0;
                Category = EnumCategory.Regular;
            }
            else if (chosen <= 10)
            {
                Value = 1;
                Category = EnumCategory.Regular;
            }
            else if (chosen <= 15)
            {
                Value = 2;
                Category = EnumCategory.Regular;
            }
            else if (chosen <= 22)
            {
                Value = 3;
                Category = EnumCategory.Regular;
            }
            else if (chosen <= 31)
            {
                Value = 4;
                Category = EnumCategory.Regular;
            }
            else if (chosen <= 40)
            {
                Value = 5;
                Category = EnumCategory.Regular;
            }
            else if (chosen <= 47)
            {
                Value = 6;
                Category = EnumCategory.Regular;
            }
            else if (chosen <= 54)
            {
                Value = 7;
                Category = EnumCategory.Regular;
            }
            else if (chosen <= 59)
            {
                Value = 10;
                Category = EnumCategory.Regular;
            }
            else if (chosen <= 61)
            {
                Category = EnumCategory.Sorry;
                Sorry = EnumSorry.Regular;
            }
            else if (chosen <= 63)
            {
                Category = EnumCategory.Sorry;
                Sorry = EnumSorry.Dont;
            }
            else if (chosen <= 67)
            {
                Category = EnumCategory.Sorry;
                Sorry = EnumSorry.At21;
            }
            else if (chosen <= 73)
                Category = EnumCategory.Take2;
            else if (chosen <= 79)
                Category = EnumCategory.Play2;
            else if (chosen <= 83)
                Category = EnumCategory.Switch;
            else if (chosen <= 85)
            {
                Category = EnumCategory.Slide;
                Value = 10;
            }
            else if (chosen <= 88)
            {
                Category = EnumCategory.Slide;
                Value = 15;
            }
            else if (chosen == 89)
            {
                Category = EnumCategory.Slide;
                Value = 20;
            }
            else if (chosen <= 91)
            {
                Category = EnumCategory.Slide;
                Value = 21;
            }
            else
                throw new BasicBlankException("Card not found when populating.  Rethink");
        }
        public void Reset() { }
        int IComparable<SorryCardGameCardInformation>.CompareTo(SorryCardGameCardInformation other)
        {
            if (Category != other.Category)
                return Category.CompareTo(other.Category);
            if (Sorry != other.Sorry)
                return Sorry.CompareTo(other.Sorry);
            return Value.CompareTo(other.Value);
        }
    }
}
