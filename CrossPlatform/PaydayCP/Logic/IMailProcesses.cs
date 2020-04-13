using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using PaydayCP.Cards;
using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    public interface IMailProcesses
    {
        Task ProcessMailAsync();
        void PopulateMails();
        void SetUpMail();
        Task ReshuffleMailAsync(DeckRegularDict<MailCard> list);
    }
}