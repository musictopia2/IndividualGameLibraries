using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeBoardGameCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
namespace LifeBoardGameWPF.Views
{
    public class ShowCardView : UserControl, IUIView
    {
        public ShowCardView(LifeBoardGameVMData model)
        {
            LifePileWPF card = new LifePileWPF();
            card.Init(model.SinglePile, "");
            Content = card;
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