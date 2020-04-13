using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using HeapSolitaireCP.Data;
using HeapSolitaireCP.ViewModels;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //since i use the grid a lot too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace HeapSolitaireWPF
{
    public class SinglePileUI : UserControl
    {
        private DeckObservableDict<HeapSolitaireCardInfo>? _cardList;
        private BasicPileInfo<HeapSolitaireCardInfo>? _privateMod;
        private BasicMultiplePilesCP<HeapSolitaireCardInfo>? _mainMod;
        private HeapSolitaireCardInfo? _currentCard;
        private Grid? _thisGrid;
        private int _pixels;
        private DeckOfCardsWPF<HeapSolitaireCardInfo> FindControl(HeapSolitaireCardInfo? thisCard)
        {
            foreach (DeckOfCardsWPF<HeapSolitaireCardInfo>? thisChild in _thisGrid!.Children)
            {
                if (thisChild!.DataContext.Equals(thisCard))
                    return thisChild;
            }
            throw new BasicBlankException("Card Not Found");
        }
        private DeckOfCardsWPF<HeapSolitaireCardInfo> GetNewCard(HeapSolitaireCardInfo thisCard)
        {
            var output = new DeckOfCardsWPF<HeapSolitaireCardInfo>();
            output.SendSize(ts.TagUsed, thisCard);
            return output; //hopefully this simple.
        }
        public void Init(HeapSolitaireMainViewModel thisMod, BasicPileInfo<HeapSolitaireCardInfo> thisPile)
        {
            _mainMod = thisMod.Waste1;
            _privateMod = thisPile;
            _cardList = _privateMod.ObjectList;
            var tempDeck = GetNewCard(new HeapSolitaireCardInfo());
            _pixels = (int)tempDeck.ObjectSize.Width / 4;
            _cardList.CollectionChanged += CollectionChanged;
            Background = Brushes.Transparent; //learned from old version of belegured castle that if not set to transparent, clicking does not work.
            MouseUp += SinglePileUI_MouseUp;
            _privateMod.PropertyChanged += PropertyChanged;
            _thisGrid = new Grid();
            Content = _thisGrid;
            PopulateControls();
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(BasicPileInfo<HeapSolitaireCardInfo>.IsSelected))
            {
                if (_currentCard == null)
                    return;
                _currentCard.IsSelected = _privateMod!.IsSelected;
            }
        }

        private void SinglePileUI_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_mainMod == null)
                return;
            if (_mainMod.PileCommand.CanExecute(_privateMod!) == false)
                return;
            _mainMod.PileCommand.Execute(_privateMod!);
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
                AddPixelColumn(_thisGrid, _pixels);
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
