namespace BattleshipCP
{
    public enum EnumStatusList
    {
        None = 0, // because at the very beginning, can't do anything.
        PlacingShips = 1, // this means the players are placing ships
        InGame = 2 // this means its in the game
    }
    public enum EnumShipList
    {
        None = 0,
        Carrier = 1,
        BattleShip = 2,
        Cruiser = 3,
        Submarine = 4,
        Destroyer = 5
    }
    public enum EnumWhatHit
    {
        None = 0,
        Hit = 1,
        Miss = 2
    }
}