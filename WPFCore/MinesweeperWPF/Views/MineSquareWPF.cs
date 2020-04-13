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
using System.Windows.Controls;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using MinesweeperCP.Data;
//using SkiaSharp.Views.WPF;
//using SkiaSharp.Views.Desktop;
using MinesweeperCP.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
//i think this is the most common things i like to do
namespace MinesweeperWPF.Views
{
    public class MineSquareWPF : GraphicsCommand
    {
        //took a risk and inherited from graphicscommand.
        //hopefully this pays off.

        private readonly SKElement _thisDraw;
        private readonly MineSquareCP _thisSquare;
        public MineSquareWPF(MineSquareCP thisSquare)
        {
            CommandParameter = thisSquare;
            Name = nameof(MinesweeperMainViewModel.MakeMoveAsync);
            _thisSquare = thisSquare;
            _thisDraw = new SKElement();
            Content = _thisDraw;
        }

        public void StartUp()
        {
            _thisSquare.NeedsToRedrawEvent += Repaint;
            _thisDraw.PaintSurface += PaintSurface;

        }


        private void PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear();
            _thisSquare.DrawSquare(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        public void Repaint()
        {
            _thisDraw.InvalidateVisual();
        }

    }
}