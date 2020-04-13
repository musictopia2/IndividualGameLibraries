using System.Collections.Generic;

namespace DominosMexicanTrainCP.Data
{
    public class SavedTrain
    {
        public int Satisfy { get; set; }
        public MexicanDomino? CenterDomino { get; set; }
        public Dictionary<int, TrainInfo> TrainList { get; set; } = new Dictionary<int, TrainInfo>();
    }
}