namespace PaydayCP.Data
{
    public enum EnumMailType
    {
        Bill = 1,
        Charity = 2,
        MadMoney = 3,
        MonsterCharge = 4,
        MoveAhead = 5,
        PayNeighbor = 6
    }
    public enum EnumStatus
    {
        None = 0, // so it will be compatible
        Starts = 1,
        ChooseLottery = 2,
        RollLottery = 3,
        RollRadio = 4,
        ChoosePlayer = 5,
        ChooseDeal = 6,
        ChooseBuy = 7,
        MakeMove = 8,
        EndingTurn = 9,
        DealOrBuy = 10,
        //ProcessMove = 11,
        RollCharity,
        ViewMail, // this is needed in order to trigger the other part (since its tied to gamestatus)
        ViewYardSale // so it can trigger to show the dice not visible.
    } //hopefully no need for processmove for status now.
    public enum EnumDay
    {
        Mail = 1,
        SweepStakes = 2,
        Deal = 3,
        Buyer = 4,
        Lottery = 5,
        YardSale = 6,
        ShoppingSpree = 7,
        SkiWeekEnd = 8,
        HappyBirthday = 9,
        CharityConcert = 10,
        RadioContest = 11,
        Food = 12,
        WalkForCharity = 13
    }
    public enum EnumCardCategory
    {
        None = 0, // to support phones
        Deal = 1,
        Mail = 2
    }
}