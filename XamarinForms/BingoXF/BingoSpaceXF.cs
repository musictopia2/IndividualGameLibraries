using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BingoCP;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp.Views.Forms;
using System.ComponentModel;
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BingoXF
{
    public class BingoSpaceXF : ContentView
    {
        public SpaceInfoCP? ThisSpace;
        private SKCanvasView? _thisDraw;
        private BingoViewModel? _thisMod;
        private BingoMainGameClass? _mainGame;
        public void Init()
        {
            if (ThisSpace == null)
                throw new BasicBlankException("Must send in the space");
            _thisMod = Resolve<BingoViewModel>();
            _mainGame = Resolve<BingoMainGameClass>();
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            _thisDraw.EnableTouchEvents = true;
            _thisDraw.Touch += Touch;
            ThisSpace.PropertyChanged += ThisSpace_PropertyChanged;
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                WidthRequest = 60;
                HeightRequest = 60;
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                WidthRequest = 90;
                HeightRequest = 90;
            }
            else
            {
                WidthRequest = 120;
                HeightRequest = 120;
            }
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(_thisDraw);
            Label thisLabel = new Label();
            thisLabel.HorizontalOptions = LayoutOptions.Center;
            thisLabel.VerticalOptions = LayoutOptions.Center; // needs to stretch for background.
            thisLabel.FontAttributes = FontAttributes.Bold;
            if (ThisSpace.IsEnabled == false)
            {
                // ThisLabel.Background = Brushes.Black
                thisLabel.TextColor = Color.White;
                if (ScreenUsed == EnumScreen.SmallPhone)
                    thisLabel.FontSize = 40;
                else if (ScreenUsed == EnumScreen.SmallTablet)
                    thisLabel.FontSize = 50;
                else
                    thisLabel.FontSize = 60;
            }
            else
            {
                // ThisLabel.Background = Brushes.White
                thisLabel.TextColor = Color.Black; // otherwise, can't do the other part.
                if (ThisSpace.Text == "Free")
                {
                    if (ScreenUsed == EnumScreen.SmallPhone)
                        thisLabel.FontSize = 25;
                    else if (ScreenUsed == EnumScreen.SmallTablet)
                        thisLabel.FontSize = 34;
                    else
                        thisLabel.FontSize = 40;
                }
                else
                {
                    if (ScreenUsed == EnumScreen.SmallPhone)
                        thisLabel.FontSize = 45;
                    else if (ScreenUsed == EnumScreen.SmallTablet)
                        thisLabel.FontSize = 50;
                    else
                        thisLabel.FontSize = 55;
                }
            }
            thisLabel.BindingContext = ThisSpace; // i think
            thisLabel.SetBinding(Label.TextProperty, new Binding(nameof(SpaceInfoCP.Text)));
            thisLabel.InputTransparent = true; //try this too.
            thisGrid.Children.Add(thisLabel);
            Content = thisGrid;
        }

        private void Touch(object sender, SKTouchEventArgs e)
        {
            if (_thisMod!.SelectSpaceCommand!.CanExecute(ThisSpace!))
                _thisMod.SelectSpaceCommand.Execute(ThisSpace!);
        }
        private void ThisSpace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SpaceInfoCP.AlreadyMarked))
                _thisDraw!.InvalidateSurface();
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            _mainGame!.SaveRoot!.BingoBoard.DrawBingoPiece(e.Surface.Canvas, e.Info.Width, e.Info.Height, ThisSpace!);
        }
    }
}