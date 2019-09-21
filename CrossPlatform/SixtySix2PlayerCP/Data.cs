namespace SixtySix2PlayerCP
{
    public enum EnumMarriage
    {
        None = 0,
        Regular = 20,
        Trumps = 40
    }
    public class ScoreValuePair
    {
        public string Description { get; set; }
        public int Score { get; set; }

        public ScoreValuePair(string desc, int points)
        {
            Description = desc;
            Score = points;
        }
    }
}