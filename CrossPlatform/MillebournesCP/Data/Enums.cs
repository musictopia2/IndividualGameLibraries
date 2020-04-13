namespace MillebournesCP.Data
{
    public enum EnumCardCategories
    {
        Miles = 1,
        Hazard = 2,
        Speed = 3,
        Remedy = 4,
        EndLimit = 5,
        Safety = 6
    }
    public enum EnumCompleteCategories
    {
        None = 0, // to be compatible with phone.
        Accident = 1,
        OutOfGas = 2,
        FlatTire = 3,
        SpeedLimit = 4,
        Stop = 5,
        Repairs = 6,
        Gasoline = 7,
        Spare = 8,
        EndOfLimit = 9,
        Roll = 10,
        DrivingAce = 11,
        ExtraTank = 12,
        PunctureProof = 13,
        RightOfWay = 14,
        Distance25 = 15,
        Distance50 = 16,
        Distance75 = 17,
        Distance100 = 18,
        Distance200 = 19
    }
    public enum EnumHazardType
    {
        StopSign = 1,
        Accident = 2,
        OutOfGas = 3,
        FlatTire = 4,
        None = 5
    }
    public enum EnumPileType
    {
        Miles = 1,
        Speed = 2,
        Hazard = 3,
        Safety = 4,
        None = 0 // this means nothing was clicked
    }
}
