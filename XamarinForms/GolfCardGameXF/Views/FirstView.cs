using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GolfCardGameCP.Data;
using GolfCardGameCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace GolfCardGameXF.Views
{
    public class FirstView : CustomControlBase, IHandleAsync<LoadEventModel>
    {
        private readonly GolfCardGameVMData _model;
        private readonly IEventAggregator _aggregator;
        private readonly CardBoardXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _beginWPF;
        public FirstView(GolfCardGameVMData model, IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            SimpleLabelGridXF label = new SimpleLabelGridXF();
            label.AddRow("Instructions", nameof(FirstViewModel.Instructions));
            _aggregator.Subscribe(this);
            _beginWPF = new CardBoardXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _model = model;
            StackLayout stack = new StackLayout();
            StackLayout other = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            other.Children.Add(_beginWPF);
            Button button = GetGamingButton("Choose First Cards", nameof(FirstViewModel.ChooseFirstCardsAsync));
            other.Children.Add(button);
            stack.Children.Add(other);
            stack.Children.Add(label.GetContent);
            Content = stack;
        }

        async Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _beginWPF.LoadList(_model.Beginnings1, ts.TagUsed);
            await this.RefreshBindingsAsync(_aggregator); //because of the buttons.
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }
    }
}