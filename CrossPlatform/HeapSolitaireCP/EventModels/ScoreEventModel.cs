using System;
using System.Collections.Generic;
using System.Text;

namespace HeapSolitaireCP.EventModels
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
