namespace LifeBoardGameCP.Data
{
    public class SpaceInfo
    {
        public bool IsOptional { get; set; }
        public bool TimesRoll { get; set; }
        public EnumCareerType CareerSpace { get; set; }
        public EnumActionType ActionInfo { get; set; }
        public EnumInsuranceType WhatInsurance { get; set; }
        public decimal AmountReceived { get; set; }
        public string Description { get; set; } = "";
        public bool GetLifeTile { get; set; }
        public bool PerKid { get; set; }
    }
}