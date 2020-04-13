using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using MilkRunCP.Data;
using SkiaSharp;
using System;
namespace MilkRunCP.Cards
{
    public class MilkRunCardInformation : SimpleDeckObject, IDeckObject, IComparable<MilkRunCardInformation>
    {
        public MilkRunCardInformation()
        {
            DefaultSize = new SKSize(55, 72);
        }
        private EnumMilkType _milkCategory;
        public EnumMilkType MilkCategory
        {
            get
            {
                return _milkCategory;
            }
            set
            {
                if (SetProperty(ref _milkCategory, value) == true)
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
        private EnumCardCategory _cardCategory;
        public EnumCardCategory CardCategory
        {
            get
            {
                return _cardCategory;
            }

            set
            {
                if (SetProperty(ref _cardCategory, value) == true)
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
