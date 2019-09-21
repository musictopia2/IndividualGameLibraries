using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
namespace CandylandCP
{
    public class CandylandCardData : SimpleDeckObject, IDeckObject
    {
        public CandylandCardData()
        {
            DefaultSize = new SKSize(150, 84);
        }
        private EnumCandyLandType _WhichCard = EnumCandyLandType.None;
        public EnumCandyLandType WhichCard
        {
            get
            {
                return _WhichCard;
            }
            set
            {
                if (SetProperty(ref _WhichCard, value) == true)
                {
                }
            }
        }
        private int _HowMany = 1; // defaults to 1
        public int HowMany
        {
            get
            {
                return _HowMany;
            }
            set
            {
                if (SetProperty(ref _HowMany, value) == true)
                {
                }
            }
        }
        private static int FindIndex(int chosen)
        {
            if (chosen < 7)
                return 1;
            if (chosen < 11)
                return 2;
            if (chosen < 17)
                return 3;
            if (chosen < 21)
                return 4;
            if (chosen < 27)
                return 5;
            if (chosen < 31)
                return 6;
            if (chosen < 37)
                return 7;
            if (chosen < 41)
                return 8;
            if (chosen < 47)
                return 9;
            if (chosen < 51)
                return 10;
            if (chosen < 57)
                return 11;
            if (chosen < 61)
                return 12;
            if (chosen == 61)
                return 13;
            if (chosen == 62)
                return 14;
            if (chosen == 63)
                return 15;
            if (chosen == 64)
                return 16;
            if (chosen == 65)
                return 17;
            if (chosen == 66)
                return 18;
            throw new BasicBlankException("Cannot find index for " + chosen);
        }
        public override string ToString()
        {
            if (HowMany > 0)
                return $"Deck: {Deck}. {HowMany} Of {WhichCard.ToString()} ";
            else
                return $"Deck: {Deck}.  {WhichCard.ToString()}";
        }
        public void Populate(int chosen)
        {
            if (chosen == 0)
                throw new BasicBlankException("Item chosen cannot be 0");
            Deck = chosen;
            int Index = FindIndex(chosen);
            switch (Index)
            {
                case 1:
                    HowMany = 1;
                    WhichCard = EnumCandyLandType.IsBlue;
                    break;
                case 2:
                    HowMany = 2;
                    WhichCard = EnumCandyLandType.IsBlue;
                    break;
                case 3:
                    HowMany = 1;
                    WhichCard = EnumCandyLandType.IsGreen;
                    break;
                case 4:
                    HowMany = 2;
                    WhichCard = EnumCandyLandType.IsGreen;
                    break;
                case 5:
                    HowMany = 1;
                    WhichCard = EnumCandyLandType.IsOrange;
                    break;
                case 6:
                    HowMany = 2;
                    WhichCard = EnumCandyLandType.IsOrange;
                    break;
                case 7:
                    HowMany = 1;
                    WhichCard = EnumCandyLandType.IsPurple;
                    break;
                case 8:
                    HowMany = 2;
                    WhichCard = EnumCandyLandType.IsPurple;
                    break;
                case 9:
                    HowMany = 1;
                    WhichCard = EnumCandyLandType.IsRed;
                    break;
                case 10:
                    HowMany = 2;
                    WhichCard = EnumCandyLandType.IsRed;
                    break;
                case 11:
                    HowMany = 1;
                    WhichCard = EnumCandyLandType.IsYellow;
                    break;
                case 12:
                    HowMany = 2;
                    WhichCard = EnumCandyLandType.IsYellow;
                    break;
                case 13:
                    WhichCard = EnumCandyLandType.IsAngel;
                    break;
                case 14:
                    WhichCard = EnumCandyLandType.IsFairy;
                    break;
                case 15:
                    WhichCard = EnumCandyLandType.IsGirl;
                    break;
                case 16:
                    WhichCard = EnumCandyLandType.IsGuard;
                    break;
                case 17:
                    WhichCard = EnumCandyLandType.IsMagic;
                    break;
                case 18:
                    WhichCard = EnumCandyLandType.IsTree;
                    break;
                default:
                    break;
            }
        }
        public void Reset() { }
    }
}