using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using MillebournesCP;
using System.Windows;
using System.Windows.Data;
namespace MillebournesWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<MillebournesCardInformation, MillebournesGraphicsCP>
    {
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(EnumCompleteCategories), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CategoryPropertyChanged)));
        public EnumCompleteCategories Category
        {
            get
            {
                return (EnumCompleteCategories)GetValue(CategoryProperty);
            }
            set
            {
                SetValue(CategoryProperty, value);
            }
        }
        private static void CategoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Category = (EnumCompleteCategories)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(CategoryProperty, new Binding(nameof(MillebournesCardInformation.CompleteCategory)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}