using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using BaseSolitaireClassesCP.GraphicsViewModels;
using BasicGameFramework.BasicDrawables.Dictionary;
using BaseSolitaireClassesCP.Cards;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
using System.Windows.Media;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using SkiaSharp;
namespace BeleaguredCastleWPF
{
    public class SinglePileUI : UserControl
    {
        //decided to not worry about reverse.

        private readonly bool _isReversed = true; //experimenting.
        private DeckObservableDict<SolitaireCard>? _cardList;
        private SolitairePilesCP? _mainMod;
        private Grid? _thisGrid;
        private PileInfoCP? _thisMod;
        private SolitaireCard? _currentCard;
        private float _widthUsed;

        public void Init(PileInfoCP singlePile, SolitairePilesCP main)
        {
            MouseUp += SinglePileUI_MouseUp;
            _mainMod = main;
            _thisMod = singlePile;
            _thisMod.PropertyChanged += PropertyChanged;
            _cardList = singlePile.CardList;
            _cardList.CollectionChanged += CollectionChanged;
            Background = Brushes.Transparent; //this was required too.
            _thisGrid = new Grid();
            Content = _thisGrid;
            var tempCard = GetNewCard(new SolitaireCard());
            _widthUsed = tempCard.ObjectSize.Width; //needs this so we know.
            PopulateControls();
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PileInfoCP.IsSelected))
            {
                if (_currentCard == null)
                    return;
                _currentCard.IsSelected = _thisMod!.IsSelected;
            }

        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateControls();
        }


        private void SinglePileUI_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_mainMod == null)
                return;
            if (_mainMod.ColumnCommand.CanExecute(_thisMod!) == false)
                return;
            _mainMod.ColumnCommand.Execute(_thisMod!);
        }

        private DeckOfCardsWPF<SolitaireCard> GetNewCard(SolitaireCard thisCard)
        {
            var output = new DeckOfCardsWPF<SolitaireCard>();
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
            totals.Times(x => AddPixelColumn(_thisGrid, (int) pixels));
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
