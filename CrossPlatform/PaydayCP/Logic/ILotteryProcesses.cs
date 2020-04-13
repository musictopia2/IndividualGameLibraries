using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    public interface ILotteryProcesses
    {

        void LoadLotteryList();
        bool CanStartLotteryProcess(); //if returns false, then won't even consider lottery.

        //bool CanContinueLotteryProcess();

        //everything to do with the lottery needs to be here.


        Task ProcessLotteryAsync();

        Task RollLotteryAsync(); //this means its rolling for lottery.

    }
}
