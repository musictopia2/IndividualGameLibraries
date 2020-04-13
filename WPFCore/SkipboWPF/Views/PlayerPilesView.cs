using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkipboCP.Cards;
using SkipboCP.Data;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SkipboWPF.Views
{
    public class PlayerPilesView : UserControl, IUIView
    {
        readonly BasicMultiplePilesWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF> _discardGraphics;
        readonly BasicMultiplePilesWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF> _stockGraphics;
        private readonly IEventAggregator _aggregator;

        public PlayerPilesView(SkipboVMData model, SkipboGameContainer gameContainer, IEventAggregator aggregator)
        {
            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            var player = gameContainer.PlayerList!.GetWhoPlayer();

            _discardGraphics = new BasicMultiplePilesWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF>();
            _discardGraphics.Init(model.DiscardPiles!, "");
            _discardGraphics.StartAnimationListener("discard" + player.NickName);
            stack.Children.Add(_discardGraphics);
            _stockGraphics = new BasicMultiplePilesWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF>();
            _stockGraphics.Init(model.StockPile!.StockFrame, ""); // i think
            _stockGraphics.StartAnimationListener("stock" + player.NickName);
            stack.Children.Add(_stockGraphics);
            _aggregator = aggregator;
            Content = stack;
        }


        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(_discardGraphics, isAll: true);
            _aggregator.Unsubscribe(_stockGraphics, isAll: true);
            return Task.CompletedTask;
        }
    }
}
