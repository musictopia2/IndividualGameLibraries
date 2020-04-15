using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using FlinchCP.Cards;
using FlinchCP.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FlinchXF.Views
{
    //this could be a control eventually
    public class PlayerPilesView : CustomControlBase
    {
        readonly BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF> _discardGraphics;
        readonly BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF> _stockGraphics;
        private readonly IEventAggregator _aggregator;

        public PlayerPilesView(FlinchVMData model, FlinchGameContainer gameContainer, IEventAggregator aggregator)
        {
            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            var player = gameContainer.PlayerList!.GetWhoPlayer();

            _discardGraphics = new BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>();
            _discardGraphics.Init(model.DiscardPiles!, "");
            _discardGraphics.StartAnimationListener("discard" + player.NickName);
            stack.Children.Add(_discardGraphics);
            _stockGraphics = new BasicMultiplePilesXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>();
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
