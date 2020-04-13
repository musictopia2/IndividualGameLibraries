using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    public interface IChoosePlayerProcesses
    {
        Task ProcessChosenPlayerAsync();
        void LoadPlayerList(); //i think something will call the process to load the player list.
    }
}