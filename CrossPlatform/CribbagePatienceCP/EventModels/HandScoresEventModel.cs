using CommonBasicStandardLibraries.CollectionClasses;
using CribbagePatienceCP.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CribbagePatienceCP.EventModels
{
    public class HandScoresEventModel
    {
        public CustomBasicList<ScoreHandCP> HandScores;
        public HandScoresEventModel(CustomBasicList<ScoreHandCP> handScores)
        {
            HandScores = handScores;
        }
    }
}
