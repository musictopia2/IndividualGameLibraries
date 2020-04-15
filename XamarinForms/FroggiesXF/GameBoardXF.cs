using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.Helpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using FroggiesCP.Data;
using FroggiesCP.Logic;
using FroggiesXF.Views;
using SkiaSharp.Views.Forms;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace FroggiesXF
{
    public class GameBoardXF : ContentView, IHandleAsync<SubscribeGameBoardEventModel>
    {
        private readonly Grid _thisGrid;
        private readonly SKCanvasView _thisDraw;
        private readonly FroggiesMainGameClass _game;
        private readonly FroggiesMainView _view;
        private readonly CustomBasicList<LilyPadXF> _lilyList = new CustomBasicList<LilyPadXF>();
        private readonly IEventAggregator _aggregator;
        public GameBoardXF(IEventAggregator aggregator, FroggiesMainGameClass game, FroggiesMainView view)
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
            _thisGrid.Children.Add(_thisDraw);
            Grid.SetRowSpan(_thisDraw, 10);
            Grid.SetColumnSpan(_thisDraw, 10);
            //Content = _thisGrid;
            aggregator.Subscribe(this);
            _aggregator = aggregator;
            _game = game;
            _view = view;
            Content = _thisGrid;
        }


        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            _game.DrawBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        async Task IHandleAsync<SubscribeGameBoardEventModel>.HandleAsync(SubscribeGameBoardEventModel message)
        {
            if (message.DrawCategory == EnumDrawCategory.NewLilyList)
            {
                if (_lilyList.Count > 0)
                {
                    _lilyList.ForEach(thisLily => _thisGrid.Children.Remove(thisLily));
                }
                var tempList = _game.GetCompleteLilyList();
                GamePackageViewModelBinder.ManuelElements.Clear();
                tempList.ForEach(thisTemp =>
                {
                    LilyPadXF thisLily = new LilyPadXF(thisTemp);
                    GamePackageViewModelBinder.ManuelElements.Add(thisLily); //we still have to add to manuel list
                    AddControlToGrid(_thisGrid, thisLily, thisTemp.Row, thisTemp.Column);
                    _lilyList.Add(thisLily);
                });
                await _view.RefreshBindingsAsync(_aggregator);
                return;
            }
            _lilyList.ForEach(thisLily => thisLily.Redraw());
        }
    }
}