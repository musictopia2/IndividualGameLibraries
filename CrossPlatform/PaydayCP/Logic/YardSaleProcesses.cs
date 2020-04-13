using BasicGameFrameworkLibrary.Attributes;
using PaydayCP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class YardSaleProcesses : IYardSaleProcesses
    {
        private readonly PaydayGameContainer _gameContainer;
        private readonly IDealProcesses _dealProcesses;

        public YardSaleProcesses(PaydayGameContainer gameContainer, IDealProcesses dealProcesses)
        {
            _gameContainer = gameContainer;
            _dealProcesses = dealProcesses;
        }

        async Task IYardSaleProcesses.FinishYardSaleAsync()
        {
            await FinishYardSaleAsync();
        }

        private async Task FinishYardSaleAsync()
        {
            _gameContainer.SaveRoot!.YardSaleDealCard = _gameContainer.SaveRoot.DealListLeft.First();
            _gameContainer.SaveRoot.DealListLeft.RemoveFirstItem();
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        async Task IYardSaleProcesses.ProcessYardSaleAsync()
        {
            await _dealProcesses.ProcessDealAsync(true);
            if (_gameContainer.SaveRoot!.DealListLeft.Count == 0)
            {
                bool rets;
                rets = await _dealProcesses.ProcessShuffleDealsAsync();
                if (rets == false)
                    return;
                if (_gameContainer.BasicData!.MultiPlayer == true)
                {
                    await _gameContainer.Network!.SendAllAsync("finishyardsale");
                }
            }
            await FinishYardSaleAsync();
        }
    }
}
