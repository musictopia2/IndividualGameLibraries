namespace LifeBoardGameCP.Data
{
    public enum EnumTypesOfCars
    {
        Minivan, Car
    }
    public enum EnumCareerType
    {
        None,
        Doctor,
        SalesPerson,
        ComputerConsultant,
        Teacher,
        Accountant,
        Athlete,
        Artist,
        Entertainer,
        PoliceOfficer
    }
    public enum EnumHouseType
    {
        None = 0,
        BeachHouse = 1,
        CozyCondo = 2,
        DutchColonial = 3,
        FarmHouse = 4,
        LogCabin = 5,
        MobileHome = 6,
        SplitLevel = 7,
        Tudor = 8,
        Victorian = 9
    }
    public enum EnumGender
    {
        None = 0,
        Boy = 1,
        Girl = 2
    }
    public enum EnumPayScale
    {
        DarkBlueGroup = 1,
        RedGroup = 2,
        GreenGroup = 3,
        YellowGroup = 4
    }
    public enum EnumTurnInfo //no need for miss next turn since that is done via interfaces.
    {
        NormalTurn = 0,
        SpinAgain = 1
    }
    public enum EnumWhatStatus //this for sure is still needed.
    {
        None = 0,
        NeedToSpin = 1,
        NeedToChooseSpace = 2, // this means there is a choice of 2 spaces.
        NeedChooseFirstOption = 3,
        NeedChooseFirstCareer = 4,
        NeedChooseHouse = 5,
        NeedSellBuyHouse = 6,
        NeedNight = 7,
        NeedTradeSalary = 8,
        NeedChooseStock = 9,
        NeedReturnStock = 10,
        NeedNewCareer = 11,
        NeedToEndTurn = 12,
        NeedFindSellPrice = 13,
        NeedChooseGender = 16,
        NeedChooseSalary = 17,
        NeedChooseRetirement = 18,
        NeedStealTile = 19,
        NeedSellHouse = 20,
        MakingMove = 21, // so when repainting and its animating move, will not have the borders anymore.
        LastSpin = 22 // this means it already spinned.
    }
    public enum EnumFinal
    {
        None,
        CountrySideAcres,
        MillionaireEstates
    }
    public enum EnumStart
    {
        None,
        College,
        Career,
    }
    public enum EnumInsuranceType
    {
        NoInsurance = 0,
        NeedHouse = 1,
        NeedCar = 2
    }
    public enum EnumActionType
    {
        CollectPayMoney = 1,
        GetPaid = 2,
        GotBabyBoy = 3,
        GotBabyGirl = 4,
        HadTwins = 5,
        WillMissTurn = 6,
        ObtainLifeTile = 7,
        StartCareer = 8,
        GetMarried = 9,
        AttendNightSchool = 10,
        FindNewJob = 11,
        PayTaxes = 12,
        StockBoomed = 13,
        StockCrashed = 14,
        TradeSalary = 15,
        SpinAgainIfBehind = 16,
        WillRetire = 17,
        MayBuyHouse = 18,
        MaySellHouse = 19
    }
    public enum EnumViewCategory
    {
        StartGame = 1,
        SpinAfterHouse = 2,
        AfterFirstSetChoices = 3,
        EndGame = 4
    }
    public enum EnumCardCategory
    {
        None, Career, House, Salary, Stock
    }
    //not sure if we need this anymore (?)

    //public enum EnumVisibleCategory
    //{
    //    GameBoard = 1, Spinner, SubmitOption, Gender, Scoreboard, PopUp, CardList, SpinButton //xamarin forms needs to try spin button.
    //}
}
