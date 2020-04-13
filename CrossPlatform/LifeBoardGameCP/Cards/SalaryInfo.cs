using LifeBoardGameCP.Data;
namespace LifeBoardGameCP.Cards
{
    public class SalaryInfo : LifeBaseCard
    {
        public SalaryInfo()
        {
            CardCategory = EnumCardCategory.Salary;
        }
        private EnumPayScale _whatGroup;
        public EnumPayScale WhatGroup
        {
            get { return _whatGroup; }
            set
            {
                if (SetProperty(ref _whatGroup, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private decimal _payCheck;
        public decimal PayCheck
        {
            get { return _payCheck; }
            set
            {
                if (SetProperty(ref _payCheck, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private decimal _taxesDue;
        public decimal TaxesDue
        {
            get { return _taxesDue; }
            set
            {
                if (SetProperty(ref _taxesDue, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}