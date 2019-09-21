using System.Collections.Generic;
namespace DominosMexicanTrainCP
{
    public class SendPlay
    {
        public int Deck { get; set; }
        public int Section { get; set; }
    }
    public class SavedTrain
    {
        public int Satisfy { get; set; }
        public MexicanDomino? CenterDomino { get; set; }
        public Dictionary<int, TrainInfo> TrainList { get; set; } = new Dictionary<int, TrainInfo>();
    }
}