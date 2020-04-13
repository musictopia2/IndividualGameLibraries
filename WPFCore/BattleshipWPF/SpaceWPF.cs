using BasicGamingUIWPFLibrary.GameGraphics.Base;
using BasicGamingUIWPFLibrary.Helpers;
using BattleshipCP.Data;
using BattleshipCP.Logic;
using BattleshipCP.ViewModels;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
using v = BasicGameFrameworkLibrary.MiscProcesses;
namespace BattleshipWPF
{
    public class SpaceWPF : GraphicsCommand
    {
        private readonly SKElement _thisDraw;
        private readonly v.Vector _space;
        private readonly GameBoardCP _gameBoard1;
        public SpaceWPF(FieldInfoCP field)
        {
            DataContext = field; // needs to be this way so if somethign changes,can act accordingly
            _thisDraw = new SKElement();
            _space = field.Vector;
            CommandParameter = _space;
            _gameBoard1 = Resolve<GameBoardCP>();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            Name = nameof(BattleshipMainViewModel.MakeMoveAsync);
            GamePackageViewModelBinder.ManuelElements.Add(this); //just in case.  because i have seen patterns where ones i get later never get picked up.
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

    }
}
