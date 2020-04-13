using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.ColorCards;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
namespace DutchBlitzCP.Cards
{
    public class DutchBlitzCardInformation : SimpleDeckObject, IDeckObject, IColorCard, IComparable<DutchBlitzCardInformation>
    {
        public DutchBlitzCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
        private string _display = "";

        public string Display
        {
            get { return _display; }
            set
            {
                if (SetProperty(ref _display, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private EnumColorTypes _color;

        public EnumColorTypes Color
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

        private int _cardValue;

        public int CardValue
        {
            get { return _cardValue; }
            set
            {
                if (SetProperty(ref _cardValue, value))
                {
                    //can decide what to do when property changes
                    Display = CardValue.ToString();
                }

            }
        }
        public int Player { get; set; }
        public int ReadMainValue => CardValue;
        public EnumColorTypes GetColor => Color;
        public void Populate(int chosen)
        {
            bool doubles = DutchBlitzDeckCount.DoubleDeck;
            Deck = chosen;
            int x;
            int y;
            int z;
            int q = 0;
            int maxs;
            int r;
            if (doubles == true)
                maxs = 2;
            else
                maxs = 1;
            for (x = 1; x <= 4; x++)
            {
                var loopTo = maxs;
                for (r = 1; r <= loopTo; r++)
                {
                    for (y = 1; y <= 10; y++)
                    {
                        for (z = 1; z <= 4; z++)
                        {
                            q += 1;
                            if (q == Deck)
                            {
                                Color = (EnumColorTypes)x;
                                CardValue = y;
                                return;
                            }
                        }
                    }
                }
            }
            throw new BasicBlankException("Sorry; cannot find the deck " + Deck);
        }
        public void Reset() { }

        int IComparable<DutchBlitzCardInformation>.CompareTo(DutchBlitzCardInformation other)
        {
            return 0; //no sorting on this game.
        }
    }
}
