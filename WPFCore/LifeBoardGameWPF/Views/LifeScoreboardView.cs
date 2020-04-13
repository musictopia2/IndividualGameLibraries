using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeBoardGameCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
namespace LifeBoardGameWPF.Views
{
    public class LifeScoreboardView : UserControl, IUIView
    {
        public LifeScoreboardView(LifeBoardGameGameContainer gameContainer)
        {
            ScoreBoardWPF score = new ScoreBoardWPF();

            score.AddColumn("Money", true, nameof(LifeBoardGamePlayerItem.MoneyEarned), useCurrency: true, rightMargin: 10);
            score.AddColumn("Loans", true, nameof(LifeBoardGamePlayerItem.Loans), useCurrency: true, rightMargin: 10);
            score.AddColumn("Stock 1", true, nameof(LifeBoardGamePlayerItem.FirstStock));
            score.AddColumn("Stock 2", true, nameof(LifeBoardGamePlayerItem.SecondStock));
            score.AddColumn("Career", true, nameof(LifeBoardGamePlayerItem.Career1), rightMargin: 10);
            score.AddColumn("Salary", true, nameof(LifeBoardGamePlayerItem.Salary), useCurrency: true, rightMargin: 10);
            score.AddColumn("Tiles", true, nameof(LifeBoardGamePlayerItem.TilesCollected));
            score.AddColumn("Car I.", true, nameof(LifeBoardGamePlayerItem.CarIsInsured), useTrueFalse: true);
            score.AddColumn("House I.", true, nameof(LifeBoardGamePlayerItem.HouseIsInsured), useTrueFalse: true);
            score.AddColumn("House N.", true, nameof(LifeBoardGamePlayerItem.HouseName));
            score.AddColumn("S Career", true, nameof(LifeBoardGamePlayerItem.Career2));
            score.LoadLists(gameContainer.PlayerList!);
            Content = score;
        }

        public Task TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        public Task TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
