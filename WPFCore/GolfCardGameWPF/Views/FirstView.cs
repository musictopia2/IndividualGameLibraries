using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GolfCardGameCP.Data;
using GolfCardGameCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace GolfCardGameWPF.Views
{
    public class FirstView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly GolfCardGameVMData _model;
        private readonly IEventAggregator _aggregator;
        private readonly CardBoardWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _beginXF;
        public FirstView(GolfCardGameVMData model, IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            SimpleLabelGrid label = new SimpleLabelGrid();
            label.AddRow("Instructions", nameof(FirstViewModel.Instructions));
            _aggregator.Subscribe(this);
            _beginXF = new CardBoardWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _model = model;
            StackPanel stack = new StackPanel();
            StackPanel other = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            other.Children.Add(_beginXF);
            Button button = GetGamingButton("Choose First Cards", nameof(FirstViewModel.ChooseFirstCardsAsync));
            other.Children.Add(button);
            stack.Children.Add(other);
            stack.Children.Add(label.GetContent);
            Content = stack;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _beginXF.LoadList(_model.Beginnings1, ts.TagUsed);
            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
