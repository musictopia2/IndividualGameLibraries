using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using BattleshipCP.Data;
using BattleshipCP.Logic;
using BattleshipCP.ViewModels;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
using v = BasicGameFrameworkLibrary.MiscProcesses;
namespace BattleshipXF
{

    //public class OldSpace : ContentView
    //{
    //    private readonly SKCanvasView _thisDraw;
    //    private readonly v.Vector _space;
    //    private readonly GameBoardCP _gameBoard1;
    //    //private readonly BattleshipViewModel _thisMod;
    //    public OldSpace(FieldInfoCP field)
    //    {
    //        BindingContext = field; // needs to be this way so if somethign changes,can act accordingly
    //        _thisDraw = new SKCanvasView();
    //        _space = field.Vector;
    //        _gameBoard1 = Resolve<GameBoardCP>();
    //        //_thisMod = Resolve<BattleshipViewModel>();
    //        _thisDraw.PaintSurface += ThisDraw_PaintSurface;
    //        _thisDraw.EnableTouchEvents = true;
    //        _thisDraw.Touch += ThisDrawTouch;
    //        WidthRequest = _gameBoard1.SpaceSize;
    //        HeightRequest = _gameBoard1.SpaceSize; // used the idea from tic tac toe.
    //        Content = _thisDraw;
    //    }
    //    private void ThisDrawTouch(object sender, SKTouchEventArgs e)
    //    {
    //        //if (_thisMod.GameBoardCommand!.CanExecute(_space) == true)
    //        //    _thisMod.GameBoardCommand.Execute(_space);
    //    }
    //    public static readonly BindableProperty FillColorProperty = BindableProperty.Create(propertyName: "FillColor", returnType: typeof(string), declaringType: typeof(SpaceXF), defaultValue: cs.Transparent, defaultBindingMode: BindingMode.TwoWay, propertyChanged: FillColorPropertyChanged);
    //    public string FillColor
    //    {
    //        get
    //        {
    //            return (string)GetValue(FillColorProperty);
    //        }
    //        set
    //        {
    //            SetValue(FillColorProperty, value);
    //        }
    //    }
    //    private static void FillColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    //    {
    //        var thisItem = (OldSpace)bindable;
    //        thisItem._thisDraw.InvalidateSurface();
    //    }
    //    public static readonly BindableProperty WhatHitProperty = BindableProperty.Create(propertyName: "WhatHit", returnType: typeof(EnumWhatHit), declaringType: typeof(SpaceXF), defaultValue: EnumWhatHit.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: WhatHitPropertyChanged);
    //    public EnumWhatHit WhatHit
    //    {
    //        get
    //        {
    //            return (EnumWhatHit)GetValue(WhatHitProperty);
    //        }
    //        set
    //        {
    //            SetValue(WhatHitProperty, value);
    //        }
    //    }
    //    private static void WhatHitPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    //    {
    //        var thisItem = (OldSpace)bindable;
    //        thisItem._thisDraw.InvalidateSurface();
    //    }
    //    private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    //    {
    //        var thisSpace = (FieldInfoCP)BindingContext;
    //        _gameBoard1.DrawSpace(e.Surface.Canvas, thisSpace, e.Info.Width, e.Info.Height);
    //    }
    //}

    public class SpaceXF : GraphicsCommand
    {
        private readonly v.Vector _space;
        private readonly GameBoardCP _gameBoard1;
        //private bool _disableDrawing = true;
        public SpaceXF(FieldInfoCP field)
        {
            //return;
            BindingContext = field; // needs to be this way so if somethign changes,can act accordingly
            _space = field.Vector;
            CommandParameter = _space;
            _gameBoard1 = Resolve<GameBoardCP>();
            ThisDraw.PaintSurface += ThisDraw_PaintSurface;
            this.SetName(nameof(BattleshipMainViewModel.MakeMoveAsync));
            GamePackageViewModelBinder.ManuelElements.Add(this);
            WidthRequest = _gameBoard1.SpaceSize;
            HeightRequest = _gameBoard1.SpaceSize; // used the idea from tic tac toe.
        }
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(propertyName: "FillColor", returnType: typeof(string), declaringType: typeof(SpaceXF), defaultValue: cs.Transparent, defaultBindingMode: BindingMode.TwoWay, propertyChanged: FillColorPropertyChanged);
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
        private static void FillColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (SpaceXF)bindable;
            thisItem.ThisDraw.InvalidateSurface();
        }
        public static readonly BindableProperty WhatHitProperty = BindableProperty.Create(propertyName: "WhatHit", returnType: typeof(EnumWhatHit), declaringType: typeof(SpaceXF), defaultValue: EnumWhatHit.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: WhatHitPropertyChanged);
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
        private static void WhatHitPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (SpaceXF)bindable;
            thisItem.ThisDraw.InvalidateSurface();
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            //return;
            //if (_disableDrawing)
            //{
            //    _disableDrawing = false;
            //    return;
            //}
            var thisSpace = (FieldInfoCP)BindingContext;
            _gameBoard1.DrawSpace(e.Surface.Canvas, thisSpace, e.Info.Width, e.Info.Height);
        }
    }
}