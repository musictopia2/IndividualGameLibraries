using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PaydayCP.Cards;
using PaydayCP.Data;
using PaydayCP.Graphics;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PaydayWPF.Views
{
    public class MailPileView : UserControl, IUIView
    {
        public MailPileView(PaydayVMData data)
        {
            BasePileWPF<MailCard, CardGraphicsCP, MailCardWPF> pile = new BasePileWPF<MailCard, CardGraphicsCP, MailCardWPF>();
            pile.Init(data.MailPile, "");
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