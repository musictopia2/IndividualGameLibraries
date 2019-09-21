using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using FroggiesCP;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using AndyCristinaGamePackageCP.DataClasses;
namespace FroggiesXF
{
    public class GameBoardXF : ContentView, IHandle<SubscribeGameBoardEventModel>
    {
        private readonly Grid _thisGrid;
        private readonly SKCanvasView _thisDraw;
        private readonly FroggiesViewModel _thisMod;
        private readonly CustomBasicList<LilyPadXF> _lilyList = new CustomBasicList<LilyPadXF>();
        public GameBoardXF(FroggiesViewModel thisMod)
        {
            _thisGrid = new Grid();
            int pixelSize;
            if (ScreenUsed != EnumScreen.SmallPhone)
                pixelSize = 64;
            else
                pixelSize = 18; //try to make is smaller for phones.
            10.Times(x =>
            {
                AddPixelColumn(_thisGrid, pixelSize);
                AddPixelRow(_thisGrid, pixelSize);
            });
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += PaintSurface;
            _thisMod = thisMod;
            _thisGrid.Children.Add(_thisDraw);
            Grid.SetRowSpan(_thisDraw, 10);
            Grid.SetColumnSpan(_thisDraw, 10);
            EventAggregator thisE = thisMod.MainContainer!.Resolve<EventAggregator>();
            thisE.Subscribe(this);
            Content = _thisGrid;
        }

        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
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
                    LilyPadXF thisLily = new LilyPadXF();
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