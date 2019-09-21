using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.ColorCards;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
namespace DutchBlitzCP
{
    public class DutchBlitzCardInformation : SimpleDeckObject, IDeckObject, IColorCard, IComparable<DutchBlitzCardInformation>
    {
        public DutchBlitzCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
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

        private EnumColorTypes _Color;

        public EnumColorTypes Color
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

        private int _CardValue;

        public int CardValue
        {
            get { return _CardValue; }
            set
            {
                if (SetProperty(ref _CardValue, value))
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