namespace ClueBoardGameCP.Data
{
    public class PredictionInfo
    {
        public string RoomName { get; set; } = "";
        public string WeaponName { get; set; } = "";
        public string CharacterName { get; set; } = ""; // not sure if it needs to be bindable (?)
    }
}