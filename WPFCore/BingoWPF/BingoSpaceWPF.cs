using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using BingoCP.Data;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Input;
using SkiaSharp.Views.Desktop;
using BingoCP.Logic;
using BingoCP.ViewModels;
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows.Data;
//i think this is the most common things i like to do
namespace BingoWPF
{
    public class BingoSpaceWPF : GraphicsCommand
    {
        public SpaceInfoCP? ThisSpace;
        private SKElement? _thisDraw;
        private BingoMainGameClass? _mainGame;
        public void Init()
        {
            if (ThisSpace == null)
                throw new BasicBlankException("Must send in the space");
            _mainGame = Resolve<BingoMainGameClass>();
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            ThisSpace.PropertyChanged += ThisSpace_PropertyChanged;
            Width = 100;
            CommandParameter = ThisSpace;
            Height = 100;
            Name = nameof(BingoMainViewModel.SelectSpace);
            GamePackageViewModelBinder.ManuelElements.Add(this);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(_thisDraw);
            TextBlock thisLabel = new TextBlock();
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            thisLabel.VerticalAlignment = VerticalAlignment.Center; // needs to stretch for background.
            thisLabel.TextAlignment = TextAlignment.Center; // will be centered for drawing text (hopefully this work)
            thisLabel.FontWeight = FontWeights.Bold;
            if (ThisSpace.IsEnabled == false)
            {
                thisLabel.Foreground = Brushes.White;
                thisLabel.FontSize = 40;
            }
            else
            {
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
