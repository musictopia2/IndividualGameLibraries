using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
namespace FlinchCP.Cards
{
    public class FlinchCardInformation : SimpleDeckObject, IDeckObject, IColorCard, IComparable<FlinchCardInformation>
    {
        public FlinchCardInformation()
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

        private int _number;

        public int Number
        {
            get { return _number; }
            set
            {
                if (SetProperty(ref _number, value))
                {
                    //can decide what to do when property changes
                    Display = Number.ToString();
                }

            }
        }
        public EnumColorTypes Color { get; set; } = EnumColorTypes.Blue;
        int ISimpleValueObject<int>.ReadMainValue => Number;
        EnumColorTypes IColorObject<EnumColorTypes>.GetColor => EnumColorTypes.Blue; //all are blue
        public void Populate(int chosen)
        {
            int z = 0;
            for (int x = 1; x <= 15; x++)
            {
                for (int y = 1; y <= 12; y++)
                {
                    z += 1;
                    if (chosen == z)
                    {
                        Deck = chosen;
                        Number = x;
                        //Color = EnumColorTypes.Blue;
                        return;
                    }
                }
            }
            throw new BasicBlankException("Not Found.  Rethink");

        }
        public void Reset() { }
        int IComparable<FlinchCardInformation>.CompareTo(FlinchCardInformation other)
        {
            return Number.CompareTo(other.Number);
        }
    }
}
