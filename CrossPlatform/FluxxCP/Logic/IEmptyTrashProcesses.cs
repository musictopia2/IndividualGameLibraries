using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using FluxxCP.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IEmptyTrashProcesses
    {
        Task EmptyTrashAsync();
        Task FinishEmptyTrashAsync(IEnumerableDeck<FluxxCardInformation> cardList);

    }
}
