using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using FroggiesCP;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace FroggiesWPF
{
    public class GameBoardWPF : UserControl, IHandle<SubscribeGameBoardEventModel>
    {
        private readonly Grid _thisGrid;
        private readonly SKElement _thisDraw;
        private readonly FroggiesViewModel _thisMod;
        private readonly CustomBasicList<LilyPadWPF> _lilyList = new CustomBasicList<LilyPadWPF>();

        public GameBoardWPF(FroggiesViewModel thisMod)
        {
            _thisGrid = new Grid();
            10.Times(x =>
            {
                AddPixelColumn(_thisGrid, 64);
                AddPixelRow(_thisGrid, 64);
            });
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            _thisMod = thisMod;
            _thisGrid.Children.Add(_thisDraw);
            Grid.SetRowSpan(_thisDraw, 10);
            Grid.SetColumnSpan(_thisDraw, 10);
            EventAggregator thisE = thisMod.MainContainer!.Resolve<EventAggregator>();
            thisE.Subscribe(this);
            Content = _thisGrid;
        }

        private void ThisDraw_PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            _thisMod.GameBoard1!.DrawBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        public void Handle(SubscribeGameBoardEventModel Message)
        {
            if (Message.DrawCategory == EnumDrawCategory.NewLilyList)
            {
                if (_lilyList.Count > 0)
                {
                    _lilyList.ForEach(thisLily => _thisGrid.Children.Remove(thisLily));
                }
                var tempList = _thisMod.GameBoard1!.GetCompleteLilyList();
                tempList.ForEach(thisTemp =>
                {
                    LilyPadWPF thisLily = new LilyPadWPF();
                    thisLily.ThisMod = _thisMod;
                    thisLily.ThisLily = thisTemp;
                    AddControlToGrid(_thisGrid, thisLily, thisTemp.Row, thisTemp.Column);
                    _lilyList.Add(thisLily);
                });
                return;
            }
            _lilyList.ForEach(thisLily => thisLily.Redraw());
        }
    }
}