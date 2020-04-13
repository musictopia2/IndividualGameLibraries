using BasicGameFrameworkLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinesweeperCP.Data
{
    [SingletonGame]
    public class LevelClass
    {
        public EnumLevel Level { get; set; } = EnumLevel.Easy; //default to easy.
    }
}
