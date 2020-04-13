using BasicGamingUIWPFLibrary.GameGraphics.Base;
using System.Windows;
using System.Windows.Data;
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.GraphicsCP;
namespace ThreeLetterFunWPF
{
    public class TileCardWPF : BaseDeckGraphicsWPF<TileInformation, TileCP>
    {
        public static readonly DependencyProperty IsMovedProperty = DependencyProperty.Register("IsMoved", typeof(bool), typeof(TileCardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsMovedPropertyChanged)));
        public bool IsMoved
        {
            get
            {
                return (bool)GetValue(IsMovedProperty);
            }
            set
            {
                SetValue(IsMovedProperty, value);
            }
        }
        private static void IsMovedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (TileCardWPF)sender;
            thisItem.MainObject!.IsMoved = (bool)e.NewValue;
        }
        public static readonly DependencyProperty LetterProperty = DependencyProperty.Register("Letter", typeof(char), typeof(TileCardWPF), new FrameworkPropertyMetadata(new char(), new PropertyChangedCallback(LetterPropertyChanged)));
        public char Letter
        {
            get
            {
                return (char)GetValue(LetterProperty);
            }
            set
            {
                SetValue(LetterProperty, value);
            }
        }
        private static void LetterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (TileCardWPF)sender;
            thisItem.MainObject!.Letter = (char)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(IsMovedProperty, new Binding(nameof(TileInformation.IsMoved)));
            SetBinding(LetterProperty, new Binding(nameof(TileInformation.Letter)));
        }
        protected override void PopulateInitObject() //this is needed too.
        {
            MainObject!.Init();
        }
    }
}
