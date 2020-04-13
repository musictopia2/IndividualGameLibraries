using BasicGamingUIWPFLibrary.GameGraphics.Base;
using ClueBoardGameCP.Cards;
using ClueBoardGameCP.Data;
using ClueBoardGameCP.Graphics;
using System.Windows;
using System.Windows.Data;

namespace ClueBoardGameWPF
{
    public class CardWPF : BaseDeckGraphicsWPF<CardInfo, CardCP>
    {
        public static readonly DependencyProperty CardValueProperty = DependencyProperty.Register("CardValue", typeof(EnumCardValues), typeof(CardWPF), new FrameworkPropertyMetadata(EnumCardValues.None, new PropertyChangedCallback(CardValuePropertyChanged)));
        public EnumCardValues CardValue
        {
            get
            {
                return (EnumCardValues)GetValue(CardValueProperty);
            }
            set
            {
                SetValue(CardValueProperty, value);
            }
        }
        private static void CardValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardWPF)sender;
            thisItem.MainObject!.CardValue = (EnumCardValues)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardValueProperty, new Binding(nameof(CardInfo.CardValue)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}