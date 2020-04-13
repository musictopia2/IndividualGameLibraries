using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using FroggiesCP.Data;
using FroggiesCP.Logic;
using FroggiesWPF.Views;
using SkiaSharp.Views.WPF;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace FroggiesWPF
{
    public class GameBoardWPF : UserControl, IHandleAsync<SubscribeGameBoardEventModel>
    {
        private readonly Grid _thisGrid;
        private readonly SKElement _thisDraw;
        private readonly CustomBasicList<LilyPadWPF> _lilyList = new CustomBasicList<LilyPadWPF>();
        private readonly IEventAggregator _aggregator;
        private readonly FroggiesMainGameClass _game;
        private readonly FroggiesMainView _view;

        public GameBoardWPF(IEventAggregator aggregator, FroggiesMainGameClass game, FroggiesMainView view)
        {
            _thisGrid = new Grid();
            10.Times(x =>
            {
                AddPixelColumn(_thisGrid, 64);
                AddPixelRow(_thisGrid, 64);
            });
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            _thisGrid.Children.Add(_thisDraw);
            Grid.SetRowSpan(_thisDraw, 10);
            Grid.SetColumnSpan(_thisDraw, 10);
            aggregator.Subscribe(this);
            Content = _thisGrid;
            _aggregator = aggregator;
            _game = game;
            _view = view;
        }

        private void ThisDraw_PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
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
                    LilyPadWPF thisLily = new LilyPadWPF(thisTemp);
                    GamePackageViewModelBinder.ManuelElements.Add(thisLily); //try this way.
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