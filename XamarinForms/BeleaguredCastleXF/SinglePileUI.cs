using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.GraphicsViewModels;
using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using SkiaSharp.Views.Forms;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BeleaguredCastleXF
{
    public class SinglePileUI : ContentView
    {
        private readonly bool _isReversed = true; //experimenting.
        private DeckObservableDict<SolitaireCard>? _cardList;
        private SolitairePilesCP? _mainMod;
        private Grid? _thisGrid;
        private PileInfoCP? _thisMod;
        private SolitaireCard? _currentCard;
        private float _widthUsed;

        private SKCanvasView? _thisControl;


        public void Init(PileInfoCP singlePile, SolitairePilesCP main)
        {

            Grid otherGrid = new Grid();

            _mainMod = main;
            _thisMod = singlePile; //hopefully property change just works right (?)
            _thisMod.PropertyChanged += SinglePileUI_PropertyChanged;

            _cardList = singlePile.CardList;
            _cardList.CollectionChanged += CollectionChanged;
            _thisGrid = new Grid();
            _thisGrid.InputTransparent = true;
            otherGrid.Children.Add(_thisGrid);
            _thisControl = new SKCanvasView();
            _thisControl.EnableTouchEvents = true;
            _thisControl.Touch += TouchEvent;
            otherGrid.Children.Add(_thisControl);
            Content = otherGrid;
            var tempCard = GetNewCard(new SolitaireCard());
            _widthUsed = tempCard.ObjectSize.Width; //needs this so we know.
            PopulateControls();
        }

        private void SinglePileUI_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PileInfoCP.IsSelected))
            {
                if (_currentCard == null)
                    return;
                _currentCard.IsSelected = _thisMod!.IsSelected;
            }
        }
        private void TouchEvent(object sender, SKTouchEventArgs e)
        {
            if (_mainMod == null)
                return;
            if (_mainMod.ColumnCommand.CanExecute(_thisMod!) == false)
                return;
            _mainMod.ColumnCommand.Execute(_thisMod!);
        }
        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateControls();
        }
        private DeckOfCardsXF<SolitaireCard> GetNewCard(SolitaireCard thisCard)
        {
            var output = new DeckOfCardsXF<SolitaireCard>();
            output.SendSize(ts.TagUsed, thisCard);
            return output;
        }
        private void PopulateControls()
        {
            _thisGrid!.ColumnDefinitions.Clear();
            _thisGrid.Children.Clear();
            if (_cardList!.Count == 0)
                return;
            var pixels = _widthUsed / 4;
            int totals = _cardList.Count + 3;
            totals.Times(x => AddPixelColumn(_thisGrid, (int)pixels));
            int y, m;
            if (_isReversed)
            {
                y = _cardList.Count;
                //d = -1;
                m = 0;
            }
            else
            {
                y = 0;
                //d = 1;
                m = _cardList.Count;
            }
            _cardList.ForEach(thisCard =>
            {
                var thisGraphics = GetNewCard(thisCard);
                AddControlToGrid(_thisGrid, thisGraphics, 0, y);
                Grid.SetColumnSpan(thisGraphics, 4);
                if (_isReversed)
                    y--;
                else
                    y++;
            });
            if (_isReversed)
                _currentCard = _cardList.Last();
            else
                _currentCard = _cardList.First();
            _currentCard.IsSelected = _thisMod!.IsSelected;
        }
    }
}
