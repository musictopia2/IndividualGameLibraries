using BasicGamingUIWPFLibrary.GameGraphics.Base;
using YahtzeeHandsDownCP.Cards;
using System.Windows;
using System.Windows.Data;
namespace YahtzeeHandsDownWPF
{
    public class ComboCardWPF : BaseDeckGraphicsWPF<ComboCardInfo, ComboCP>
    {
        public static readonly DependencyProperty CurrentComboProperty = DependencyProperty.Register("CurrentCombo", typeof(ComboCardInfo), typeof(ComboCardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CurrentComboPropertyChanged)));
        public ComboCardInfo CurrentCombo
        {
            get
            {
                return (ComboCardInfo)GetValue(CurrentComboProperty);
            }
            set
            {
                SetValue(CurrentComboProperty, value);
            }
        }
        private static void CurrentComboPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (ComboCardWPF)sender;
            thisItem.MainObject!.ThisCombo = (ComboCardInfo)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ComboCardWPF.CurrentComboProperty, new Binding(nameof(ComboCardInfo.CurrentCombo)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
