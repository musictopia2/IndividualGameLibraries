namespace AccordianSolitaireCP.EventModels
{
    public class ScoreEventModel
    {
        public int Score { get; set; }
        public ScoreEventModel(int score)
        {
            Score = score;
        }
    }
}