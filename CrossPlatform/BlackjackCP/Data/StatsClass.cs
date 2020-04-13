using BasicGameFrameworkLibrary.Attributes;

namespace BlackjackCP.Data
{
    [SingletonGame]
    public class StatsClass
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
    }
}