namespace Pinochle2PlayerCP
{
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