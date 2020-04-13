using AccordianSolitaireCP.Data;
using AccordianSolitaireCP.EventModels;
using AccordianSolitaireCP.ViewModels;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace AccordianSolitaireWPF.Views
{
    public class AccordianSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>,
        IHandle<RedoEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseHandWPF<AccordianSolitaireCardInfo, ts, DeckOfCardsWPF<AccordianSolitaireCardInfo>>? _board;
        public AccordianSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            StackPanel stack = new StackPanel();
            

            SimpleLabelGrid thisLabel = new SimpleLabelGrid();
            thisLabel.AddRow("Score", nameof(AccordianSolitaireMainViewModel.Score));
            _board = new BaseHandWPF<AccordianSolitaireCardInfo, ts, DeckOfCardsWPF<AccordianSolitaireCardInfo>>();
            _board.Divider = 1.5f;
            stack.Children.Add(_board);
            stack.Children.Add(thisLabel.GetContent);
            Content = stack; //if not doing this, rethink.
        }

        void IHandle<RedoEventModel>.Handle(RedoEventModel message)
        {
            _board!.RedoCards();
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            AccordianSolitaireMainViewModel model = (AccordianSolitaireMainViewModel)DataContext;
            _board!.LoadList(model.GameBoard1!, ts.TagUsed);

            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
