using BasicGameFrameworkLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSolitaireCP.Data
{
    [SingletonGame]
    public class LevelClass
    {
        public int LevelChosen { get; set; } = 1;
    }
}
