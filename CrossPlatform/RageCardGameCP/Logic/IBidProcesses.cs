using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RageCardGameCP.Logic
{
    public interface IBidProcesses
    {
        Task ProcessBidAsync();
        Task LoadBiddingScreenAsync();

    }
}
