using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using MonopolyCardGameCP;
using System.Windows;
using System.Windows.Data;
namespace MonopolyCardGameWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP> //beginning
    {
        public static readonly DependencyProperty CardValueProperty = DependencyProperty.Register("CardValue", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(CardValuePropertyChanged)));
        public int CardValue
        {
            get
            {
                return (int)GetValue(CardValueProperty);
            }
            set
            {
                SetValue(CardValueProperty, value);
            }
        }
        private static void CardValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.CardValue = (int)e.NewValue;
        } //end
        protected override void DoCardBindings()
        {
            SetBinding(CardValueProperty, new Binding(nameof(MonopolyCardGameCardInformation.CardValue)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    public class MonopolyHandWPF : BaseHandWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsWPF>, IMonopolyScroll
    {
        void IMonopolyScroll.ScrollToBottom()
        {
            ThisScroll!.ScrollToBottom();
        }
    }
}
