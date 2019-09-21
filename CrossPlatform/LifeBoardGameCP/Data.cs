namespace LifeBoardGameCP
{
    public class SpinnerPositionData
    {
        public int ChangePositions { get; set; }
        public bool CanBetween { get; set; }
        public int HighSpeedUpTo { get; set; } // this is how long it will do highspeed (so it can vary)
    }
    public interface ISpinnerCanvas // i think should use this.
    {
        void Repaint();
        int Position { get; set; }
        int HighSpeedPhase { get; set; }
    }
    public class TileInfo
    {
        public decimal AmountReceived { get; set; }
        public string Description { get; set; } = "";
    }
    public class ShortcutInfo
    {
        public int FromSpace { get; set; }
        public int ToSpace { get; set; }
    }
    public class SpaceInfo
    {
        public bool IsOptional;
        public bool TimesRoll;
        public EnumCareerType CareerSpace;
        public EnumActionType ActionInfo;
        public EnumInsuranceType WhatInsurance;
        public decimal AmountReceived;
        public string Description = "";
        public bool GetLifeTile;
        public bool PerKid;
    }
}