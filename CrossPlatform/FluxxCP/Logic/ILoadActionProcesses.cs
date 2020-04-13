using System;
using System.Collections.Generic;
using System.Text;

namespace FluxxCP.Logic
{
    public interface ILoadActionProcesses
    {
        void LoadFirstRandom();
        void LoadRules();
        void LoadDoAgainCards();
        void LoadDirections();
        void LoadTradeHands();
        void LoadUseTake();
        void LoadEverybodyGetsOne();
        void LoadDrawUse();

    }
}
