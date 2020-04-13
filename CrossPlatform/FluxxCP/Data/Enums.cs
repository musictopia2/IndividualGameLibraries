namespace FluxxCP.Data
{
    public enum EnumDirection
    {
        Left = 0,
        Right = 1
    }
    public enum EnumCardType
    {
        Rule = 1,
        Keeper = 2,
        Goal = 3,
        Action = 4
    }
    public enum EnumRuleText
    {
        BasicRule = 1,
        Play2 = 2,
        Play3 = 3,
        Play4 = 4,
        PlayAll = 5,
        KeeperLimit2 = 6,
        KeeperLimit3 = 7,
        KeeperLimit4 = 8,
        Draw2 = 9,
        Draw3 = 10,
        Draw4 = 11,
        Draw5 = 12,
        HandLimit0 = 13,
        HandLimit1 = 14,
        HandLimit2 = 15,
        NoHandBonus = 16,
        PoorBonus = 17,
        RichBonus = 18,
        ReverseOrder = 19,
        FirstPlayRandom = 20,
        Inflation = 21,
        DoubleAgenda = 22
    }
    public enum EnumRuleCategory
    {
        None = 0,
        Hand = 1,
        Keeper = 2,
        Play = 3,
        Draw = 4,
        Basic = 5
    }
    public enum EnumRuleBonus
    {
        None = 0,
        NoHand = 1,
        PoorBonus = 2,
        RichBonus = 3
    }
    public enum EnumKeeper
    {
        Milk = 23,
        TheRocket = 24,
        TheMoon = 25,
        War = 26,
        Television = 27,
        TheToaster = 28,
        Money = 29,
        Love = 30,
        Dreams = 31,
        Peace = 32,
        Bread = 33,
        TheSun = 34,
        Cookies = 35,
        Time = 36,
        TheBrain = 37,
        Death = 38,
        Sleep = 39,
        Chocolate = 40
    }
    public enum EnumGoalRegular
    {
        Toast = 41,
        Keepers = 42, // think
        TimeIsMoney = 43,
        BedTime = 44,
        AllYouNeedIsLove = 45,
        PeaceNoWar = 46,
        BakedGoods = 47,
        DreamLand = 48,
        HeartsAndMinds = 49,
        MilkAndCookies = 50,
        RocketToTheMoon = 51,
        Hippyism = 52,
        NightAndDay = 53,
        SquishyChocolate = 54,
        RocketScience = 55,
        WinningTheLottery = 56,
        TheAppliances = 57,
        TheBrainNoTV = 58,
        DeathByChocolate = 59,
        ChocolateCookies = 60,
        ChocolateMilk = 61,
        CardsInHand = 62,
        WarDeath = 63
    }
    public enum EnumSpecialGoalSpecial
    {
        None = 0,
        Keepers = 1,
        Hand = 2
    }
    public enum EnumActionMain
    {
        TrashANewRule = 64, //okay
        TakeAnotherTurn = 65, //okay
        TradeHands = 66, //okay
        TrashAKeeper = 67, //okay
        ExchangeKeepers = 68,
        StealAKeeper = 69, //okay
        UseWhatYouTake = 70,//
        LetsDoThatAgain = 71, //okay
        ScrambleKeepers = 72, //most likely okay
        RulesReset = 73, //okay
        EmptyTheTrash = 74, //okay
        Draw3Play2OfThem = 75,
        Draw2AndUseEm = 76, //okay
        EverybodyGets1 = 77, //okay
        DiscardDraw = 78, //okay
        LetsSimplify = 79, //okay
        RotateHands = 80, //okay
        NoLimits = 81, //okay
        Taxation = 82, //okay
        Jackpot = 83 //okay
    }
    public enum EnumActionScreen
    {
        None = 0,
        ActionScreen = 1,
        KeeperScreen = 2, // would be better each time to do a select/case to figure out which controls to load up
        OtherPlayer = 3 // for taxation; the other players chooses a card from the main screen.  still a 2 parter
    }
    public enum EnumEndTurnStatus
    {
        Successful = 0, // this means it can end turn
        Hand = 1,
        Play = 2,
        Keeper = 3,
        Goal = 4
    }
    public enum EnumKeeperSection
    {
        None, Trash, Steal, Exchange
    }
    public enum EnumActionCategory
    {
        None, Rules, Directions, DoAgain, TradeHands, UseTake, Everybody1, DrawUse, FirstRandom
    }
    public enum EnumKeeperVisibleCategory
    {
        Close = 1, Actions
    }
}