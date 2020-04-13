using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnoCP.Logic
{
    public interface ISayUnoProcesses
    {
        Task ProcessUnoAsync(bool saiduno);
    }
}
