using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System;
using System.Linq;
namespace RummyDiceCP
{
    public class RummyDiceInfo : ObservableObject, IRummmyObject<EnumColorType, EnumColorType>,
        IBasicDice<int>, IGenerateDice<int>, IComparable<RummyDiceInfo>, ISelectableObject
    {
        public int HeightWidth { get; set; } = 44;
        public int Value { get; set; }
        public int Index { get; set; }
        private bool _Visible = true; //has to be proven false.
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

        private bool _IsSelected;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (SetProperty(ref _IsSelected, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (SetProperty(ref _Visible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private EnumColorType _Color;

        public EnumColorType Color
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
        public bool IsWild { get; set; }
        CustomBasicList<int> IGenerateDice<int>.GetPossibleList //decided to make it twice as likely to get 11 or 12 now.  also means we don't need the extra 2 wilds anymore.
        {
            get
            {
                CustomBasicList<int> output = new CustomBasicList<int>();
                //first 44 is just as likely.
                var temps = Enumerable.Range(1, 44);
                output.AddRange(temps);
                output.AddRange(temps);
                temps = Enumerable.Range(45, 8);
                output.AddRange(temps);
                temps = new CustomBasicList<int> { 11, 22, 33, 44 };
                output.AddRange(temps);
                return output;
            }
        }

        int IRummmyObject<EnumColorType, EnumColorType>.GetSecondNumber => 0;

        int ISimpleValueObject<int>.ReadMainValue => Value;

        bool IWildObject.IsObjectWild => IsWild;

        bool IIgnoreObject.IsObjectIgnored => false;

        EnumColorType ISuitObject<EnumColorType>.GetSuit => Color;

        EnumColorType IColorObject<EnumColorType>.GetColor => Color;

        public void Populate(int chosen)
        {
            if (chosen == 0)
                throw new BasicBlankException("Cannot choose 0");
            //Index = Chosen; //hopefully this works doing it this way.
            int z = 0;

            for (int x = 1; x <= 4; x++)
            {
                for (int y = 1; y <= 11; y++)
                {
                    z++;
                    if (z == chosen)
                    {
                        Index = chosen;
                        Color = x.ToEnum<EnumColorType>();
                        if (y == 11)
                        {
                            IsWild = true;
                            Value = 0;
                            Display = "W";
                        }
                        else
                        {
                            IsWild = false;
                            Value = y;
                            Display = Value.ToString();
                        }
                        return;
                    }
                }
            }


            for (int x = 1; x <= 4; x++)
            {
                for (int y = 11; y <= 12; y++)
                {
                    z++;
                    if (z == chosen)
                    {
                        Index = chosen;
                        Color = x.ToEnum<EnumColorType>();
                        Value = y;
                        Display = Value.ToString();
                        IsWild = false;
                        return;
                    }
                }
            }
            throw new BasicBlankException($"Nothing found for {chosen}");
        }
        int IComparable<RummyDiceInfo>.CompareTo(RummyDiceInfo other) //probably important to sort the dice this time.
        {
            if (IsWild.Equals(other.IsWild) == false)
                return IsWild.CompareTo(other.IsWild);
            if (Value.Equals(other.Value) == false)
                return Value.CompareTo(other.Value);
            return Color.CompareTo(other.Color);
        }
    }
}