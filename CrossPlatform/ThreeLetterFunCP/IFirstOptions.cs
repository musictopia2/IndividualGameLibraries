using System.Threading.Tasks;
namespace ThreeLetterFunCP
{
    public interface IFirstOptions
    {
        Task ChoseAdvancedOptions(bool IsEasy, bool ShortGame);
        Task CardsChosenAsync(int HowManyCards);
        Task BeginningOptionSelectedAsync(EnumFirstOption FirstOption);
    }
}