using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicControlsAndWindowsCore.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using PokerCP;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace PokerWPF
{
    public class HandUI : UserControl, INewCard
    {
        private CustomBasicCollection<DisplayCard>? _thisList;
        private Grid? _thisGrid;
        private PokerViewModel? _thisMod;
        private DeckOfCardsWPF<PokerCardInfo>? FindControl(int index)
        {
            foreach (UIElement? thisCon in _thisGrid!.Children)
            {
                if (Grid.GetRow(thisCon) == 0 && Grid.GetColumn(thisCon) == index)
                    return thisCon as DeckOfCardsWPF<PokerCardInfo>;
            }
            throw new BasicBlankException("No control found");
        }
        private Binding GetCommandBinding(string path)
        {
            Binding ThisBind = new Binding(path);
            ThisBind.Source = _thisMod;
            return ThisBind;
        }

        private void PopulateControls()
        {
            _thisGrid!.Children.Clear(); // i think
            if (_thisList!.Count == 0)
                return;
            if (_thisList.Count != 5)
                throw new BasicBlankException("Must have 5 cards for poker hand.");
            int x = 0;
            foreach (var ThisPoker in _thisList)
            {
                var ThisGraphics = GetNewCard(ThisPoker);
                GridHelper.AddControlToGrid(_thisGrid, ThisGraphics, 0, x);
                var ThisLabel = SharedWindowFunctions.GetDefaultLabel();
                ThisLabel.FontSize = 20; // can always be adjusted as needed
                ThisLabel.HorizontalAlignment = HorizontalAlignment.Center;
                ThisLabel.FontWeight = FontWeights.Bold;
                ThisLabel.DataContext = ThisPoker;
                ThisLabel.SetBinding(TextBlock.TextProperty, new Binding(nameof(DisplayCard.Text)));
                GridHelper.AddControlToGrid(_thisGrid, ThisLabel, 1, x);
                x += 1;
            }
        }
        private DeckOfCardsWPF<PokerCardInfo> GetNewCard(DisplayCard thisPoker)
        {
            DeckOfCardsWPF<PokerCardInfo> thisCard = new DeckOfCardsWPF<PokerCardInfo>();
            thisCard.SendSize(ts.TagUsed, thisPoker.CurrentCard);
            var thisBind = GetCommandBinding(nameof(PokerViewModel.HoldUnHoldCardCommand));
            thisCard.SetBinding(DeckOfCardsWPF<PokerCardInfo>.CommandProperty, thisBind);
            thisCard.CommandParameter = thisPoker;
            return thisCard;
        }

        private void ThisList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PopulateControls();
        }
        void INewCard.NewCard(int index)
        {
            if (_thisList!.Count == 0)
                return;
            var newCard = _thisList[index];
            var thisControl = FindControl(index);
            thisControl!.DataContext = newCard.CurrentCard; //hopefully this is enough (?)
        }
        public void Init(PokerViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisGrid = new Grid();
            GridHelper.AddAutoColumns(_thisGrid, 5);
            GridHelper.AddAutoRows(_thisGrid, 2);
            _thisList = thisMod.PokerList;
            _thisList.CollectionChanged += ThisList_CollectionChanged;
            Content = _thisGrid;
            PopulateControls();
        }
    }
}