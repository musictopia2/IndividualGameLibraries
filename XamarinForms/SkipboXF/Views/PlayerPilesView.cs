using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using SkipboCP.Cards;
using SkipboCP.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SkipboXF.Views
{
    public class PlayerPilesView : CustomControlBase
    {
        readonly BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF> _discardGraphics;
        readonly BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF> _stockGraphics;
        private readonly IEventAggregator _aggregator;

        public PlayerPilesView(SkipboVMData model, SkipboGameContainer gameContainer, IEventAggregator aggregator)
        {
            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            var player = gameContainer.PlayerList!.GetWhoPlayer();

            _discardGraphics = new BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
            _discardGraphics.Init(model.DiscardPiles!, "");
            _discardGraphics.StartAnimationListener("discard" + player.NickName);
            stack.Children.Add(_discardGraphics);
            _stockGraphics = new BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
            _stockGraphics.Init(model.StockPile!.StockFrame, ""); // i think
            _stockGraphics.StartAnimationListener("stock" + player.NickName);
            stack.Children.Add(_stockGraphics);
            _aggregator = aggregator;
            Content = stack;
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(_discardGraphics, isAll: true);
            _aggregator.Unsubscribe(_stockGraphics, isAll: true);
            _discardGraphics.Unregister();
            _stockGraphics.Unregister();
            return base.TryCloseAsync();
        }
    }
}
