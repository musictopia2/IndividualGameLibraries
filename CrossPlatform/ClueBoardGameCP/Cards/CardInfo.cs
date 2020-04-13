using ClueBoardGameCP.Data;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;

namespace ClueBoardGameCP.Cards
{
    public class CardInfo : MainInfo
    {
        public CardInfo()
        {
            DefaultSize = new SKSize(55, 72);
        }
        public EnumCardType WhatType { get; set; }
        private EnumCardValues _cardValue;
        public EnumCardValues CardValue
        {
            get { return _cardValue; }
            set
            {
                if (SetProperty(ref _cardValue, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public int Number { get; set; }
        public override void Populate(int chosen)
        {
            throw new BasicBlankException("You have to use the global function so we can get the name.");
        }
        public override void Reset() { }
    }
}
