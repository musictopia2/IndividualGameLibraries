using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using HitTheDeckCP.Data;
using SkiaSharp;
using System;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace HitTheDeckCP.Cards
{
    public class HitTheDeckCardInformation : SimpleDeckObject, IDeckObject, IComparable<HitTheDeckCardInformation>
    {
        public HitTheDeckCardInformation()
        {
            DefaultSize = new SKSize(55, 72);
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

        private int _firstSort;

        public int FirstSort
        {
            get { return _firstSort; }
            set
            {
                if (SetProperty(ref _firstSort, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private EnumTypeList _cardType;

        public EnumTypeList CardType
        {
            get { return _cardType; }
            set
            {
                if (SetProperty(ref _cardType, value))
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
        private EnumInstructionList _instructions;

        public EnumInstructionList Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _anyColor;

        public bool AnyColor
        {
            get { return _anyColor; }
            set
            {
                if (SetProperty(ref _anyColor, value))
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
        private int _number;

        public int Number
        {
            get { return _number; }
            set
            {
                if (SetProperty(ref _number, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string GetColor(int value)
        {
            if (value == 1)
                return cs.Yellow;
            if (value == 2)
                return cs.Blue;
            if (value == 3)
                return cs.Red;
            if (value == 4)
                return cs.Green;
            throw new BasicBlankException("There are only 4 colors in this game");
        }
        private void PopulateCardType()
        {
            CardType = Instructions switch
            {
                EnumInstructionList.PlayNumber => EnumTypeList.Number,
                EnumInstructionList.PlayColor => EnumTypeList.Color,
                EnumInstructionList.Flip => EnumTypeList.Flip,
                EnumInstructionList.Cut => EnumTypeList.Cut,
                EnumInstructionList.RandomDraw => EnumTypeList.Draw4,
                _ => EnumTypeList.Regular,
            };
        }
        public void Populate(int chosen)
        {
            int x;
            int y;
            int z = 0;
            int q;
            for (q = 1; q <= 4; q++)
            {
                for (x = 1; x <= 5; x++)
                {
                    for (y = 1; y <= 4; y++)
                    {
                        z += 1;
                        if (chosen == z)
                        {
                            Deck = chosen;
                            Color = GetColor(y);
                            FirstSort = y;
                            Number = x;
                            Instructions = EnumInstructionList.None;
                            Points = Number;
                            PopulateCardType();
                            return;
                        }
                    }
                }
            }
            for (q = 1; q <= 5; q++)
            {
                for (y = 1; y <= 4; y++)
                {
                    z += 1;
                    if (chosen == z)
                    {
                        Deck = chosen;
                        Color = GetColor(y);
                        FirstSort = y;
                        Number = 0;
                        Points = 10;
                        if (q < 3)
                            Instructions = EnumInstructionList.Cut;
                        else if (q == 3)
                            Instructions = EnumInstructionList.Flip;
                        else if (q == 4)
                        {
                            Instructions = EnumInstructionList.PlayColor;
                            AnyColor = true;
                        }
                        else if (q == 5)
                        {
                            Instructions = EnumInstructionList.RandomDraw;
                            AnyColor = true;
                        }

                        PopulateCardType();
                        return;
                    }
                }
            }
            for (q = 1; q <= 2; q++)
            {
                for (y = 1; y <= 5; y++)
                {
                    z += 1;
                    if (chosen == z)
                    {
                        Deck = chosen;
                        Number = y;
                        FirstSort = 0; // i think because no color
                        Points = 10;
                        Instructions = EnumInstructionList.PlayNumber;
                        AnyColor = true;
                        PopulateCardType();
                        return;
                    }
                }
            }
            throw new BasicBlankException("not found.  rethink");
        }
        public void Reset() { }
        int IComparable<HitTheDeckCardInformation>.CompareTo(HitTheDeckCardInformation other)
        {
            if (FirstSort != other.FirstSort)
                return FirstSort.CompareTo(other.FirstSort);
            if (Number != other.Number)
                return Number.CompareTo(other.Number);
            return Instructions.CompareTo(other.Instructions);
        }
    }
}
