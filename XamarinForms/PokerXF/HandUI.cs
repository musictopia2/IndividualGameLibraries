using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using PokerCP;
using System.Collections.Specialized;
using Xamarin.Forms;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
namespace PokerXF
{
    public class HandUI : ContentView, INewCard
    {
        private CustomBasicCollection<DisplayCard>? _thisList;
        private Grid? _thisGrid;
        private PokerViewModel? _thisMod;
        private DeckOfCardsXF<PokerCardInfo>? FindControl(int index)
        {
            foreach (View? thisCon in _thisGrid!.Children)
            {
                if (Grid.GetRow(thisCon) == 0 && Grid.GetColumn(thisCon) == index)
                    return thisCon as DeckOfCardsXF<PokerCardInfo>;
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
            foreach (var thisPoker in _thisList)
            {
                var thisGraphics = GetNewCard(thisPoker);
                GridHelper.AddControlToGrid(_thisGrid, thisGraphics, 0, x);
                var thisLabel = SharedPageFunctions.GetDefaultLabel();
                if (ScreenUsed != EnumScreen.SmallPhone)
                    thisLabel.FontSize = 20; // can always be adjusted as needed
                else
                    thisLabel.FontSize = 12;
                thisLabel.HorizontalOptions = LayoutOptions.Center;
                thisLabel.FontAttributes = FontAttributes.Bold;
                thisLabel.BindingContext = thisPoker;
                thisLabel.SetBinding(Label.TextProperty, new Binding(nameof(DisplayCard.Text)));
                GridHelper.AddControlToGrid(_thisGrid, thisLabel, 1, x);
                x += 1;
            }
        }
        private DeckOfCardsXF<PokerCardInfo> GetNewCard(DisplayCard thisPoker)
        {
            DeckOfCardsXF<PokerCardInfo> thisCard = new DeckOfCardsXF<PokerCardInfo>();
            thisCard.SendSize(ts.TagUsed, thisPoker.CurrentCard);
            var thisBind = GetCommandBinding(nameof(PokerViewModel.HoldUnHoldCardCommand));
            thisCard.SetBinding(DeckOfCardsXF<PokerCardInfo>.CommandProperty, thisBind);
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
            thisControl!.BindingContext = newCard.CurrentCard; //hopefully this is enough (?)
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