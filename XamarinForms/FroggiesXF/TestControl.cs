using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FroggiesXF
{
    public class TestControl : ContentView
    {
        private readonly Button _button;
        //private readonly SKCanvasView _thisDraw;
        public TestControl()
        {
            
            //_thisDraw = new SKCanvasView();
            ////_thisDraw.WidthRequest = 200;
            ////_thisDraw.HeightRequest = 200;
            //_thisDraw.PaintSurface += Paint;
            //_thisDraw.EnableTouchEvents = true;
            //_thisDraw.Touch += Touch;
            //StackLayout stack = new StackLayout();
            //stack.Children.Add(_thisDraw);
            Button button = new Button();
            button.IsEnabled = true;
            button.Text = "Test";
            _button = button;
            _button.Clicked += Button_Clicked;
            Content = button;
            //stack.Children.Add(button);
            //Content = stack;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            
        }

        private void Touch(object sender, SKTouchEventArgs e)
        {
            
        }

        private void Paint(object sender, SKPaintSurfaceEventArgs e)
        {
            SKRect rect = SKRect.Create(0, 0, 160, 160);
            SKPaint paint = new SKPaint();
            paint.Color = SKColors.Red;
            paint.Style = SKPaintStyle.Fill;
            e.Surface.Canvas.DrawRect(rect, paint);
        }
    }
}
