using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommonInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FiveCrownsCP
{
    public class FiveCrownsCardInformation : SimpleDeckObject, IDeckObject, IRummmyObject<EnumSuitList, EnumColorList>, IComparable<FiveCrownsCardInformation>
    {
        public FiveCrownsCardInformation()
        {
            DefaultSize = new SkiaSharp.SKSize(55, 72);
        }
        int IRummmyObject<EnumSuitList, EnumColorList>.GetSecondNumber => (int)CardValue;

        int ISimpleValueObject<int>.ReadMainValue => (int)CardValue;

        private FiveCrownsMainGameClass? _mainGame;
        public bool IsObjectWild
        {
            get
            {
                if (_mainGame == null)
                    _mainGame = Resolve<FiveCrownsMainGameClass>();
                return CardType == EnumCardTypeList.Joker || _mainGame.SaveRoot!.UpTo == (int)CardValue;
            }
        }
        bool IIgnoreObject.IsObjectIgnored => false;
        EnumSuitList ISuitObject<EnumSuitList>.GetSuit => Suit;
        EnumColorList IColorObject<EnumColorList>.GetColor => ColorSuit;

        private EnumSuitList _Suit = EnumSuitList.None;
        public EnumSuitList Suit
        {
            get
            {
                return _Suit;
            }

            set
            {
                if (SetProperty(ref _Suit, value) == true)
                {

                }
            }
        }
        public EnumSuitList OriginalSuit { get; set; } = EnumSuitList.None;

        private EnumCardValueList _CardValue;
        public EnumCardValueList CardValue
        {
            get
            {
                return _CardValue;
            }

            set
            {
                if (SetProperty(ref _CardValue, value) == true)
                {

                }
            }
        }
        private EnumCardTypeList _CardType;
        public EnumCardTypeList CardType
        {
            get
            {
                return _CardType;
            }

            set
            {
                if (SetProperty(ref _CardType, value) == true)
                {
                }
            }
        }
        private EnumColorList _ColorSuit;
        public EnumColorList ColorSuit
        {
            get
            {
                return _ColorSuit;
            }

            set
            {
                if (SetProperty(ref _ColorSuit, value) == true)
                {
                }
            }
        }
        private int _Points;
        public int Points
        {
            get
            {
                return _Points;
            }

            set
            {
                if (SetProperty(ref _Points, value) == true)
                {
                }
            }
        }
        public void Populate(int chosen)
        {
            int newItem;
            if (chosen > 58)
                newItem = chosen - 58;
            else
                newItem = chosen;
            if (newItem >= 56)
            {
                Deck = chosen;
                CardType = EnumCardTypeList.Joker;
                CardValue = EnumCardValueList.Joker;
                return;
            }
            int x;
            int y;
            int z = 0;
            for (x = 1; x <= 5; x++)
            {
                for (y = 3; y <= 13; y++)
                {
                    z += 1;
                    if (z == newItem)
                    {

                        Suit = (EnumSuitList)x;
                        Deck = chosen;
                        OriginalSuit = Suit; //this too.
                        CardValue = (EnumCardValueList)y;
                        ColorSuit = Suit switch
                        {
                            EnumSuitList.Clubs => EnumColorList.Green,
                            EnumSuitList.Diamonds => EnumColorList.Blue,
                            EnumSuitList.Spades => EnumColorList.Black,
                            EnumSuitList.Hearts => EnumColorList.Red,
                            EnumSuitList.Stars => EnumColorList.Yellow,
                            _ => throw new BasicBlankException("Not supported"),
                        };
                        return;
                    }
                }
            }
            throw new BasicBlankException($"Nothing found for chosen {chosen}");
        }
        public void Reset()
        {
            Suit = OriginalSuit; //i think.
        }
        int IComparable<FiveCrownsCardInformation>.CompareTo(FiveCrownsCardInformation other)
        {
            if (IsObjectWild == true && other.IsObjectWild == false)
                return 1;
            else if (IsObjectWild == false && other.IsObjectWild == true)
                return -1; //could be opposite but not sure yet
            if (ColorSuit != other.ColorSuit)
                return ColorSuit.CompareTo(other.ColorSuit);
            return CardValue.CompareTo(other.CardValue);
        }
    }
}