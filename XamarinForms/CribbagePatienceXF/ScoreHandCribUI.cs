using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.Exceptions;
using CribbagePatienceCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CribbagePatienceXF
{
    public class ScoreHandCribUI : BaseFrameXF
    {
        private Grid? _thisGrid;
        private ScoreHandCP? _tempMod;
        private DeckObservableDict<CribbageCard>? _cardList;

        public void Init(ScoreHandCP mod)
        {
            _tempMod = mod;
            _cardList = _tempMod.CardList;
            _cardList.CollectionChanged += CollectionChanged;
            _thisGrid = new Grid();
            Text = _tempMod.Text; // i think
            Grid parentGrid = new Grid();
            parentGrid.Children.Add(ThisDraw);
            parentGrid.Children.Add(_thisGrid);
            SetUpMarginsOnParentControl(_thisGrid);
            _thisGrid.Margin = new Thickness(_thisGrid.Margin.Left, _thisGrid.Margin.Top + 8, _thisGrid.Margin.Right, _thisGrid.Margin.Bottom); // not sure why did not work well this time (?)
            AddAutoColumns(_thisGrid, 3);
            AddAutoRows(_thisGrid, 2);
            HorizontalOptions = LayoutOptions.Fill; //i think
            Content = parentGrid;
            if (_cardList.Count > 0)
                PopulateList();
        }
        private void PopulateList()
        {
            if (_cardList!.Count == 0)
            {
                _thisGrid!.Children.Clear(); // needs to clear because no card list.  the cardlist works together with the scores
                return;
            }
            var ScoreStack = new StackLayout();
            AddControlToGrid(_thisGrid!, ScoreStack, 0, 2);
            Grid.SetRowSpan(ScoreStack, 2);
            if (_cardList.Count != 4)
                throw new BasicBlankException("Must have 4 cards for this control, not " + _cardList.Count);
            foreach (var ThisItem in _tempMod!.Scores)
            {
                var ThisLabel = GetDefaultLabel();
                ThisLabel.Margin = new Thickness(5, 0, 0, 0);
                ThisLabel.Text = ThisItem.Description + " - " + ThisItem.Points;
                ScoreStack.Children.Add(ThisLabel);
            }
            int x;
            int y;
            int z = 0;

            for (y = 1; y <= 2; y++)
            {
                for (x = 1; x <= 2; x++)
                {
                    var thisCard = _cardList[z];
                    var thisGraphics = new DeckOfCardsXF<CribbageCard>();
                    thisGraphics.SendSize(ts.TagUsed, thisCard);
                    AddControlToGrid(_thisGrid!, thisGraphics, y - 1, x - 1);
                    z += 1;
                }
            }
        }
        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                PopulateList();
                return;
            }
            throw new BasicBlankException("The only collection action support is reset");
        }
    }
}
