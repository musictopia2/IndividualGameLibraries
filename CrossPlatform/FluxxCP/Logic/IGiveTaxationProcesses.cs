using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using FluxxCP.Cards;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IGiveTaxationProcesses
    {
        Task GiveCardsForTaxationAsync(IDeckDict<FluxxCardInformation> list);
    }
}
