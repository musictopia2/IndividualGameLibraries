using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using System.Windows;
using System.Windows.Data;
using TileRummyCP;
namespace TileRummyWPF
{
    public class TileWPF : BaseDeckGraphicsWPF<TileInfo, TileCP>
    {
        public static readonly DependencyProperty IsJokerProperty = DependencyProperty.Register("IsJoker", typeof(bool), typeof(TileWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsJokerPropertyChanged)));
        public bool IsJoker
        {
            get
            {
                return (bool)GetValue(IsJokerProperty);
            }
            set
            {
                SetValue(IsJokerProperty, value);
            }
        }
        private static void IsJokerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (TileWPF)sender;
            thisItem.MainObject!.IsJoker = (bool)e.NewValue;
        }
        public static readonly DependencyProperty WhatDrawProperty = DependencyProperty.Register("WhatDraw", typeof(EnumDrawType), typeof(TileWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(WhatDrawPropertyChanged)));
        public EnumDrawType WhatDraw
        {
            get
            {
                return (EnumDrawType)GetValue(WhatDrawProperty);
            }
            set
            {
                SetValue(WhatDrawProperty, value);
            }
        }
        private static void WhatDrawPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (TileWPF)sender;
            thisItem.MainObject!.WhatDraw = (EnumDrawType)e.NewValue;
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(EnumColorType), typeof(TileWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));
        public EnumColorType Color
        {
            get
            {
                return (EnumColorType)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (TileWPF)sender;
            thisItem.MainObject!.Color = (EnumColorType)e.NewValue;
        }
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register("Number", typeof(int), typeof(TileWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(NumberPropertyChanged)));
        public int Number
        {
            get
            {
                return (int)GetValue(NumberProperty);
            }
            set
            {
                SetValue(NumberProperty, value);
            }
        }
        private static void NumberPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (TileWPF)sender;
            thisItem.MainObject!.Number = (int)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ColorProperty, new Binding(nameof(TileInfo.Color)));
            SetBinding(NumberProperty, new Binding(nameof(TileInfo.Number)));
            SetBinding(WhatDrawProperty, new Binding(nameof(TileInfo.WhatDraw)));
            SetBinding(IsJokerProperty, new Binding(nameof(TileInfo.IsJoker)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.2f;
    }
}