using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PaydayCP.Cards;
using PaydayCP.Data;
using PaydayCP.Graphics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
namespace PaydayWPF.Views
{
    public class MailListView : UserControl, IUIView
    {
        public MailListView(PaydayVMData model)
        {
            BaseHandWPF<MailCard, CardGraphicsCP, MailCardWPF> hand = new BaseHandWPF<MailCard, CardGraphicsCP, MailCardWPF>();
            hand.HandType = HandObservable<MailCard>.EnumHandList.Vertical;
            hand.Height = 500;
            hand.HorizontalAlignment = HorizontalAlignment.Left;
            hand.LoadList(model.CurrentMailList, "");
            Content = hand;
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