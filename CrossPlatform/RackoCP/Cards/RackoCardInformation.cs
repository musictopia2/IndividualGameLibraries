using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using SkiaSharp;
using System;
namespace RackoCP.Cards
{
    public class RackoCardInformation : SimpleDeckObject, IDeckObject, IRummmyObject<EnumSuitList, EnumSuitList>, IComparable<RackoCardInformation>
    {
        public RackoCardInformation()
        {
            DefaultSize = new SKSize(200, 35);
        }
        int IRummmyObject<EnumSuitList, EnumSuitList>.GetSecondNumber => 0;
        int ISimpleValueObject<int>.ReadMainValue => Value;
        bool IWildObject.IsObjectWild => false;
        bool IIgnoreObject.IsObjectIgnored => false;
        EnumSuitList ISuitObject<EnumSuitList>.GetSuit => EnumSuitList.None;
        EnumSuitList IColorObject<EnumSuitList>.GetColor => EnumSuitList.None;
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
        private bool _willKeep; //only for computer ai.
        public bool WillKeep
        {
            get { return _willKeep; }
            set
            {
                if (SetProperty(ref _willKeep, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public void Populate(int chosen)
        {
            Value = chosen;
            Deck = chosen; //this simple.
        }
        public void Reset() { }

        int IComparable<RackoCardInformation>.CompareTo(RackoCardInformation other)
        {
            return 0; //needed for autoresume.
        }
    }
}