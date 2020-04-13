using System.Threading.Tasks;
namespace ThreeLetterFunCP.BeginningClasses
{
    public interface ICardsChosenProcesses
    {
        Task CardsChosenAsync(int howManyCards);
    }
}