using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using LifeBoardGameCP.Data;
namespace LifeBoardGameXF.Views
{
    public class LifeScoreboardView : CustomControlBase
    {
        public LifeScoreboardView(LifeBoardGameGameContainer gameContainer)
        {
            ScoreBoardXF score = new ScoreBoardXF();
            score.AddColumn("Money", true, nameof(LifeBoardGamePlayerItem.MoneyEarned), useCurrency: true, rightMargin: 10);
            score.AddColumn("Salary", true, nameof(LifeBoardGamePlayerItem.Salary), useCurrency: true, rightMargin: 10);
            score.AddColumn("Stock 1", false, nameof(LifeBoardGamePlayerItem.FirstStock));
            score.AddColumn("Stock 2", false, nameof(LifeBoardGamePlayerItem.SecondStock));
            score.AddColumn("Career", true, nameof(LifeBoardGamePlayerItem.Career1), rightMargin: 10);
            score.AddColumn("Tiles", false, nameof(LifeBoardGamePlayerItem.TilesCollected));
            score.AddColumn("S Career", true, nameof(LifeBoardGamePlayerItem.Career2));
            score.AddColumn("Car I.", false, nameof(LifeBoardGamePlayerItem.CarIsInsured), useTrueFalse: true);
            score.AddColumn("House N.", true, nameof(LifeBoardGamePlayerItem.HouseName));
            score.AddColumn("House I.", false, nameof(LifeBoardGamePlayerItem.HouseIsInsured), useTrueFalse: true);
            score.AddColumn("Loans", true, nameof(LifeBoardGamePlayerItem.Loans), useCurrency: true, rightMargin: 10);
            score.LoadLists(gameContainer.PlayerList!);
            Content = score; //could be iffy.
        }


    }
}
