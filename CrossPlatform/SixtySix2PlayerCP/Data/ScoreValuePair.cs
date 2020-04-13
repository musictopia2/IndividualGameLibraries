//i think this is the most common things i like to do
namespace SixtySix2PlayerCP.Data
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
