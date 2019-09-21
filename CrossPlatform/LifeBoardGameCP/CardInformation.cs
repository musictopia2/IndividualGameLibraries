using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using SkiaSharp;
using System.Collections.Generic;
namespace LifeBoardGameCP
{
    public class LifeBaseCard : SimpleDeckObject, IDeckObject //can't be abstract or can't autosave cards.
    {
        public LifeBaseCard()
        {
            DefaultSize = new SKSize(80, 100);
        }
        private EnumCardCategory _CardCategory;
        public EnumCardCategory CardCategory
        {
            get { return _CardCategory; }
            set
            {
                if (SetProperty(ref _CardCategory, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public void Populate(int chosen) { }
        public void Reset() { }
    }
    public class CareerInfo : LifeBaseCard
    {
        public CareerInfo()
        {
            CardCategory = EnumCardCategory.Career;
        }
        private EnumCareerType _Career;
        public EnumCareerType Career
        {
            get { return _Career; }
            set
            {
                if (SetProperty(ref _Career, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _Title = "";
        public string Title
        {
            get { return _Title; }
            set
            {
                if (SetProperty(ref _Title, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumPayScale _Scale1;
        public EnumPayScale Scale1
        {
            get { return _Scale1; }
            set
            {
                if (SetProperty(ref _Scale1, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumPayScale _Scale2;
        public EnumPayScale Scale2
        {
            get { return _Scale2; }
            set
            {
                if (SetProperty(ref _Scale2, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _DegreeRequired;
        public bool DegreeRequired
        {
            get { return _DegreeRequired; }
            set
            {
                if (SetProperty(ref _DegreeRequired, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _Description = "";
        public string Description
        {
            get { return _Description; }
            set
            {
                if (SetProperty(ref _Description, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
    public class HouseInfo : LifeBaseCard
    {
        public HouseInfo()
        {
            CardCategory = EnumCardCategory.House;
        }
        private EnumHouseType _HouseCategory;
        public EnumHouseType HouseCategory
        {
            get { return _HouseCategory; }
            set
            {
                if (SetProperty(ref _HouseCategory, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public Dictionary<int, decimal> SellingPrices { get; set; } = new Dictionary<int, decimal>();
        private string _Title = "";
        public string Title
        {
            get { return _Title; }
            set
            {
                if (SetProperty(ref _Title, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _Description = "";
        public string Description
        {
            get { return _Description; }
            set
            {
                if (SetProperty(ref _Description, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private decimal _HousePrice;
        public decimal HousePrice
        {
            get { return _HousePrice; }
            set
            {
                if (SetProperty(ref _HousePrice, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private decimal _InsuranceCost;
        public decimal InsuranceCost
        {
            get { return _InsuranceCost; }
            set
            {
                if (SetProperty(ref _InsuranceCost, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
    public class SalaryInfo : LifeBaseCard
    {
        public SalaryInfo()
        {
            CardCategory = EnumCardCategory.Salary;
        }
        private EnumPayScale _WhatGroup;
        public EnumPayScale WhatGroup
        {
            get { return _WhatGroup; }
            set
            {
                if (SetProperty(ref _WhatGroup, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private decimal _PayCheck;
        public decimal PayCheck
        {
            get { return _PayCheck; }
            set
            {
                if (SetProperty(ref _PayCheck, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private decimal _TaxesDue;
        public decimal TaxesDue
        {
            get { return _TaxesDue; }
            set
            {
                if (SetProperty(ref _TaxesDue, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
    public class StockInfo : LifeBaseCard
    {
        public StockInfo()
        {
            CardCategory = EnumCardCategory.Stock;
        }
        private int _Value;
        public int Value
        {
            get { return _Value; }
            set
            {
                if (SetProperty(ref _Value, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}