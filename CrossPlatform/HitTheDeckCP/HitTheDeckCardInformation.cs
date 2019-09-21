using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace HitTheDeckCP
{
    public class HitTheDeckCardInformation : SimpleDeckObject, IDeckObject, IComparable<HitTheDeckCardInformation>
    {
        public HitTheDeckCardInformation()
        {
            DefaultSize = new SKSize(55, 72);
        }

        private string _Color = cs.Transparent;

        public string Color
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

        private int _FirstSort;

        public int FirstSort
        {
            get { return _FirstSort; }
            set
            {
                if (SetProperty(ref _FirstSort, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private EnumTypeList _CardType;

        public EnumTypeList CardType
        {
            get { return _CardType; }
            set
            {
                if (SetProperty(ref _CardType, value))
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
        private EnumInstructionList _Instructions;

        public EnumInstructionList Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _AnyColor;

        public bool AnyColor
        {
            get { return _AnyColor; }
            set
            {
                if (SetProperty(ref _AnyColor, value))
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