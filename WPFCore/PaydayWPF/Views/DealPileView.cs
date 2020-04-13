using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PaydayCP.Cards;
using PaydayCP.Data;
using PaydayCP.Graphics;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PaydayWPF.Views
{
    public class DealPileView : UserControl, IUIView
    {
        public DealPileView(PaydayVMData data)
        {
            BasePileWPF<DealCard, CardGraphicsCP, DealCardWPF> pile = new BasePileWPF<DealCard, CardGraphicsCP, DealCardWPF>();
            pile.Init(data.DealPile, "");
            Content = pile;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}