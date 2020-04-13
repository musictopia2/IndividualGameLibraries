using MinesweeperCP.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinesweeperCP.ViewModels
{
    public interface ILevelVM
    {
        EnumLevel LevelChosen { get; set; }
        int HowManyMinesNeeded { get; set; }
    }
}
