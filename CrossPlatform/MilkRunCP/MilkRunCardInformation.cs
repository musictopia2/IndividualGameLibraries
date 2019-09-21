using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
namespace MilkRunCP
{
    public class MilkRunCardInformation : SimpleDeckObject, IDeckObject, IComparable<MilkRunCardInformation>
    {
        public MilkRunCardInformation()
        {
            DefaultSize = new SKSize(55, 72);
        }
        private EnumMilkType _MilkCategory;
        public EnumMilkType MilkCategory
        {
            get
            {
                return _MilkCategory;
            }
            set
            {
                if (SetProperty(ref _MilkCategory, value) == true)
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
        private EnumCardCategory _CardCategory;
        public EnumCardCategory CardCategory
        {
            get
            {
                return _CardCategory;
            }

            set
            {
                if (SetProperty(ref _CardCategory, value) == true)
                {
                }
            }
        }
        public void Populate(int chosen)
        {
            //populating the card.
            if (chosen <= 61)
                ModifyData(chosen, EnumMilkType.Strawberry);
            else
                ModifyData(chosen, EnumMilkType.Chocolate);
        }
        private void ModifyData(int chosen, EnumMilkType tempMilk)
        {
            MilkCategory = tempMilk;
            Deck = chosen;
            int tempDeck;
            if (Deck <= 61)
                tempDeck = chosen;
            else
                tempDeck = chosen - 61;
            int c;
            int z = 0;
            for (int x = 1; x <= 12; x++)
            {
                c = GetPointFreq(x);
                for (int y = 1; y <= c; y++)
                {
                    z++;
                    if (z == tempDeck)
                    {
                        CardCategory = EnumCardCategory.Points;
                        Points = x;
                        return;
                    }
                }
            }
            for (int x = 1; x <= 7; x++)
            {
                z++;
                if (z == tempDeck)
                {
                    CardCategory = EnumCardCategory.Go;
                    return;
                }
            }
            for (int x = 1; x <= 3; x++)
            {
                z++;
                if (z == tempDeck)
                {
                    CardCategory = EnumCardCategory.Stop;
                    return;
                }
            }
            for (int x = 1; x <= 3; x++)
            {
                z++;
                if (z == tempDeck)
                {
                    CardCategory = EnumCardCategory.Joker;
                    return;
                }
            }
            throw new BasicBlankException("cannot find deck.  rethink");
        }
        private int GetPointFreq(int upTo)
        {
            if (upTo <= 3)
                return 3;
            if (upTo <= 8)
                return 6;
            if (upTo <= 10)
                return 3;
            if (upTo == 11 || upTo == 12)
                return 2;
            return 0;
        }
        public void Reset() { }
        int IComparable<MilkRunCardInformation>.CompareTo(MilkRunCardInformation other)
        {
            if (MilkCategory != other.MilkCategory)
                return MilkCategory.CompareTo(other.MilkCategory);
            if (CardCategory != other.CardCategory)
                return CardCategory.CompareTo(other.CardCategory);
            return Points.CompareTo(other.Points);
        }
    }
}