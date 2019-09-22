using BattleshipCP;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using v = BasicGameFramework.MiscProcesses;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BattleshipWPF
{
    public class SpaceWPF : UserControl
    {
        private readonly SKElement _thisDraw;
        private readonly v.Vector _space;
        private readonly GameBoardCP _gameBoard1;
        private readonly BattleshipViewModel _thisMod;
        public SpaceWPF(FieldInfoCP field)
        {
            DataContext = field; // needs to be this way so if somethign changes,can act accordingly
            _thisDraw = new SKElement();
            _space = field.Vector;
            _gameBoard1 = Resolve<GameBoardCP>();
            _thisMod = Resolve<BattleshipViewModel>();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            MouseUp += SpaceWPF_MouseUp;
            Width = _gameBoard1.SpaceSize;
            Height = _gameBoard1.SpaceSize; // used the idea from tic tac toe.
            Content = _thisDraw;
        }
        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register("FillColor", typeof(string), typeof(SpaceWPF), new FrameworkPropertyMetadata(cs.Transparent, new PropertyChangedCallback(FillColorPropertyChanged)));
        public string FillColor
        {
            get
            {
                return (string)GetValue(FillColorProperty);
            }
            set
            {
                SetValue(FillColorProperty, value);
            }
        }
        private static void FillColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (SpaceWPF)sender;
            thisItem._thisDraw.InvalidateVisual();
        }
        public static readonly DependencyProperty WhatHitProperty = DependencyProperty.Register("WhatHit", typeof(EnumWhatHit), typeof(SpaceWPF), new FrameworkPropertyMetadata(EnumWhatHit.None, new PropertyChangedCallback(WhatHitPropertyChanged)));
        public EnumWhatHit WhatHit
        {
            get
            {
                return (EnumWhatHit)GetValue(WhatHitProperty);
            }
            set
            {
                SetValue(WhatHitProperty, value);
            }
        }
        private static void WhatHitPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (SpaceWPF)sender;
            thisItem._thisDraw.InvalidateVisual();
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            var thisSpace = (FieldInfoCP)DataContext;
            _gameBoard1.DrawSpace(e.Surface.Canvas, thisSpace, e.Info.Width, e.Info.Height);
        }
        private void SpaceWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_thisMod.GameBoardCommand!.CanExecute(_space) == true)
                _thisMod.GameBoardCommand.Execute(_space);
        }
    }
}