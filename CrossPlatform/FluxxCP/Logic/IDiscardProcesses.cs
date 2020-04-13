using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
using FluxxCP.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IDiscardProcesses
    {
        Task DiscardFromHandAsync(CustomBasicList<int> list);
        Task DiscardFromHandAsync(FluxxCardInformation thisCard);
        Task DiscardFromHandAsync(IDeckDict<FluxxCardInformation> list);
        Task DiscardFromHandAsync(int deck);

        Task DiscardGoalAsync(int deck);
        Task DiscardGoalAsync(GoalCard thisCard);
        Task DiscardKeeperAsync(int deck);
        Task DiscardKeeperAsync(FluxxCardInformation thisCard);
        Task DiscardKeepersAsync(IDeckDict<FluxxCardInformation> list);

        Task DiscardKeepersAsync(CustomBasicList<int> list);

        //this means something else has to do animate play.




    }
}
