using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using HeapSolitaireCP;
using SkiaSharp.Views.Forms;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace HeapSolitaireXF
{
    public class SinglePileUI : ContentView
    {
        private DeckObservableDict<HeapSolitaireCardInfo>? _cardList;
        private BasicPileInfo<HeapSolitaireCardInfo>? _privateMod;
        private BasicMultiplePilesCP<HeapSolitaireCardInfo>? _mainMod;
        private HeapSolitaireCardInfo? _currentCard;
        private Grid? _thisGrid;
        private int pixels;
        private SKCanvasView? _canvas;
        private DeckOfCardsXF<HeapSolitaireCardInfo> FindControl(HeapSolitaireCardInfo? thisCard)
        {
            foreach (DeckOfCardsXF<HeapSolitaireCardInfo>? thisChild in _thisGrid!.Children)
            {
                if (thisChild!.BindingContext.Equals(thisCard))
                    return thisChild;
            }
            throw new BasicBlankException("Card Not Found");
        }
        private DeckOfCardsXF<HeapSolitaireCardInfo> GetNewCard(HeapSolitaireCardInfo thisCard)
        {
            var output = new DeckOfCardsXF<HeapSolitaireCardInfo>();
            output.SendSize(ts.TagUsed, thisCard);
            return output; //hopefully this simple.
        }
        public void Init(HeapSolitaireViewModel thisMod, BasicPileInfo<HeapSolitaireCardInfo> thisPile)
        {
            _mainMod = thisMod.Waste1;
            _privateMod = thisPile;
            _cardList = _privateMod.ObjectList;
            var tempDeck = GetNewCard(new HeapSolitaireCardInfo());
            pixels = (int)tempDeck.ObjectSize.Width / 4;
            _cardList.CollectionChanged += CollectionChanged;
            _canvas = new SKCanvasView();
            _canvas.EnableTouchEvents = true;
            _canvas.Touch += Touch;
            Grid grid = new Grid();
            _privateMod.PropertyChanged += TempPropertyChanged;
            _thisGrid = new Grid();
            _thisGrid.InputTransparent = true;
            grid.Children.Add(_thisGrid);
            grid.Children.Add(_canvas);
            Content = grid;
            PopulateControls();
        }

        private void Touch(object sender, SKTouchEventArgs e)
        {
            if (_mainMod == null)
                return;
            if (_mainMod.PileCommand.CanExecute(_privateMod!) == false)
                return;
            _mainMod.PileCommand.Execute(_privateMod!);
        }

        private void TempPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(BasicPileInfo<HeapSolitaireCardInfo>.IsSelected))
            {
                if (_currentCard == null)
                    return;
                _currentCard.IsSelected = _privateMod!.IsSelected;
            }
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                PopulateControls();
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count != 1)
                    throw new BasicBlankException("Only One Item Can Be Removed");
                var thisCard = e.OldItems[0] as HeapSolitaireCardInfo;
                var thisGraphics = FindControl(thisCard);
                _thisGrid!.Children.Remove(thisGraphics);
                if (_cardList!.Count > 0)
                {
                    _currentCard = _cardList.Last();
                    _currentCard.IsSelected = true;
                    return;
                }
                _thisGrid.Children.Clear();
            }
        }

        private void PopulateControls()
        {
            _thisGrid!.Children.Clear();
            _thisGrid.ColumnDefinitions.Clear();
            if (_cardList!.Count == 0)
                return;
            int times = _cardList.Count + 3;
            times.Times(x =>
            {
                AddPixelColumn(_thisGrid, pixels);
            });
            int y = 0;
            _cardList.ForEach(thisCard =>
            {
                var thisGraphics = GetNewCard(thisCard);
                AddControlToGrid(_thisGrid, thisGraphics, 0, y);
                Grid.SetColumnSpan(thisGraphics, 4);
                y++;
            });
            _currentCard = _cardList.Last();
        }
    }
}