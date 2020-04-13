using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using FiveCrownsCP.Data;
using FiveCrownsCP.Logic;
using SkiaSharp;
using System;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FiveCrownsCP.Cards
{
    public class FiveCrownsCardInformation : SimpleDeckObject, IDeckObject, IRummmyObject<EnumSuitList, EnumColorList>, IComparable<FiveCrownsCardInformation>
    {
        public FiveCrownsCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
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
                return CardType == EnumCardTypeList.Joker || _mainGame.SaveRoot!.UpTo == (int) CardValue;
            }
        }
        bool IIgnoreObject.IsObjectIgnored => false;
        EnumSuitList ISuitObject<EnumSuitList>.GetSuit => Suit;
        EnumColorList IColorObject<EnumColorList>.GetColor => ColorSuit;

        private EnumSuitList _suit = EnumSuitList.None;
        public EnumSuitList Suit
        {
            get
            {
                return _suit;
            }

            set
            {
                if (SetProperty(ref _suit, value) == true)
                {

                }
            }
        }
        public EnumSuitList OriginalSuit { get; set; } = EnumSuitList.None;

        private EnumCardValueList _cardValue;
        public EnumCardValueList CardValue
        {
            get
            {
                return _cardValue;
            }

            set
            {
                if (SetProperty(ref _cardValue, value) == true)
                {

                }
            }
        }
        private EnumCardTypeList _cardType;
        public EnumCardTypeList CardType
        {
            get
            {
                return _cardType;
            }

            set
            {
                if (SetProperty(ref _cardType, value) == true)
                {
                }
            }
        }
        private EnumColorList _colorSuit;
        public EnumColorList ColorSuit
        {
            get
            {
                return _colorSuit;
            }

            set
            {
                if (SetProperty(ref _colorSuit, value) == true)
                {
                }
            }
        }
        private int _points;
        public int Points
        {
            get
            {
                return _points;
            }

            set
            {
                if (SetProperty(ref _points, value) == true)
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
