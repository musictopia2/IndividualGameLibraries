using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FlinchCP.Cards;
using FlinchCP.Data;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FlinchWPF.Views
{
    public class PlayerPilesView : UserControl, IUIView
    {
        readonly BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF> _discardGraphics;
        readonly BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF> _stockGraphics;
        private readonly IEventAggregator _aggregator;

        public PlayerPilesView(FlinchVMData model, FlinchGameContainer gameContainer, IEventAggregator aggregator)
        {
            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            var player = gameContainer.PlayerList!.GetWhoPlayer();

            _discardGraphics = new BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>();
            _discardGraphics.Init(model.DiscardPiles!, "");
            _discardGraphics.StartAnimationListener("discard" + player.NickName);
            stack.Children.Add(_discardGraphics);
            _stockGraphics = new BasicMultiplePilesWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>();
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
