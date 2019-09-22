using BingoCP;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BingoWPF
{
    public class BingoSpaceWPF : UserControl
    {
        public SpaceInfoCP? ThisSpace;
        private SKElement? _thisDraw;
        private BingoViewModel? _thisMod;
        private BingoMainGameClass? _mainGame;
        public void Init()
        {
            if (ThisSpace == null)
                throw new BasicBlankException("Must send in the space");
            _thisMod = Resolve<BingoViewModel>();
            _mainGame = Resolve<BingoMainGameClass>();
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            ThisSpace.PropertyChanged += ThisSpace_PropertyChanged;
            MouseUp += BingoSpaceWPF_MouseUp;
            Width = 100;
            Height = 100;
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(_thisDraw);
            TextBlock thisLabel = new TextBlock();
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            thisLabel.VerticalAlignment = VerticalAlignment.Center; // needs to stretch for background.
            thisLabel.TextAlignment = TextAlignment.Center; // will be centered for drawing text (hopefully this work)
            thisLabel.FontWeight = FontWeights.Bold;
            if (ThisSpace.IsEnabled == false)
            {
                // ThisLabel.Background = Brushes.Black
                thisLabel.Foreground = Brushes.White;
                thisLabel.FontSize = 40;
            }
            else
            {
                // ThisLabel.Background = Brushes.White
                thisLabel.Foreground = Brushes.Black; // otherwise, can't do the other part.
                if (ThisSpace.Text == "Free")
                    thisLabel.FontSize = 34;
                else
                    thisLabel.FontSize = 45;
            }
            thisLabel.DataContext = ThisSpace; // i think
            thisLabel.SetBinding(TextBlock.TextProperty, new Binding(nameof(SpaceInfoCP.Text)));
            thisGrid.Children.Add(thisLabel);
            Content = thisGrid;
        }

        private void BingoSpaceWPF_MouseUp(object? sender, MouseButtonEventArgs e)
        {
            if (_thisMod!.SelectSpaceCommand!.CanExecute(ThisSpace!))
                _thisMod.SelectSpaceCommand.Execute(ThisSpace!);
        }

        private void ThisSpace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SpaceInfoCP.AlreadyMarked))
                _thisDraw!.InvalidateVisual();
        }

        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            _mainGame!.SaveRoot!.BingoBoard.DrawBingoPiece(e.Surface.Canvas, e.Info.Width, e.Info.Height, ThisSpace!);
        }
    }
}