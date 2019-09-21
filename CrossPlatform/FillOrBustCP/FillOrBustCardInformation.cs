using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
namespace FillOrBustCP
{
    public class FillOrBustCardInformation : SimpleDeckObject, IDeckObject
    {
        private EnumCardStatusList _Status = EnumCardStatusList.Unknown;
        public EnumCardStatusList Status
        {
            get
            {
                return _Status;
            }

            set
            {
                if (SetProperty(ref _Status, value) == true)
                {
                }
            }
        }// this has to be bindable

        private int _Value;
        public int Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (SetProperty(ref _Value, value) == true)
                {
                }
            }
        }
        public int BonusAmount;
        public int FillsRequired;
        public bool IsOptional;
        public bool AddToScore;
        private void CalculateBindings()
        {
            // i think needs to be here.  so i run something after it figures out the card
            // this will also calculate out the value as well
            if (Value > 0)
                return;
            if (BonusAmount > 0)
                Value = BonusAmount;
            else if (IsOptional == true && FillsRequired == 1)
                Value = 2500;
        }
        public FillOrBustCardInformation()
        {
            DefaultSize = new SKSize(107, 135);
        }
        private int FindDeck(int deck)
        {
            if (deck < 13)
                return 1;
            if (deck < 23)
                return 2;
            if (deck < 31)
                return 3;
            if (deck < 37)
                return 4;
            if (deck < 39)
                return 5;
            if (deck < 43)
                return 6;
            if (deck < 51)
                return 7;
            if (deck < 55)
                return 8;
            throw new BasicBlankException("Cannot find one for " + deck);
        }
        public void Populate(int chosen)
        {
            int index = FindDeck(chosen);
            Status = EnumCardStatusList.None; //has to prove otherwise.
            switch (index)
            {
                case 1:
                    BonusAmount = 300;
                    break;
                case 2:
                    BonusAmount = 400;
                    break;
                case 3:
                    BonusAmount = 500;
                    break;
                case 4:
                    BonusAmount = 1000;
                    FillsRequired = 1;
                    break;
                case 5:
                    BonusAmount = 0;
                    FillsRequired = 2;
                    AddToScore = true;
                    Status = EnumCardStatusList.DoubleTrouble;
                    break;
                case 6:
                    Status = EnumCardStatusList.MustBust;
                    break;
                case 7:
                    Status = EnumCardStatusList.NoDice;
                    break;
                case 8:
                    IsOptional = true;
                    FillsRequired = 1;
                    AddToScore = true;
                    break;
                default:
                    throw new BasicBlankException($"None For {index}");
            }
            Deck = chosen; //i think
            CalculateBindings(); //later as well.
        }
        public void Reset() { }
    }
}