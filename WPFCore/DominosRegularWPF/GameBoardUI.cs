using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dominos;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Dominos;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using DominosRegularCP;
using SkiaSharp;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace DominosRegularWPF
{
    public class GameBoardUI : BaseFrameWPF
    {

        private DominosWPF<SimpleDominoInfo>? _firstGraphics;
        private DominosWPF<SimpleDominoInfo>? _secondGraphics;
        private DominosWPF<SimpleDominoInfo>? _centerGraphics;
        private Grid? _thisGrid;
        private DeckObservableDict<SimpleDominoInfo>? _dominoList;
        private DominosRegularViewModel? _thisMod;
        private DominosWPF<SimpleDominoInfo> CreateGraphics(SimpleDominoInfo thisD)
        {
            DominosWPF<SimpleDominoInfo> output = new DominosWPF<SimpleDominoInfo>();
            output.SendSize(ts.TagUsed, thisD);
            Binding binding = new Binding(nameof(GameBoardCP.DominoCommand));
            binding.Source = _thisMod!.GameBoard1;
            output.SetBinding(DominosWPF<SimpleDominoInfo>.CommandProperty, binding);
            output.CommandParameter = thisD;
            return output;
        }
        public void LoadList()
        {
            _thisMod = Resolve<DominosRegularViewModel>();
            IProportionImage thisP = _thisMod.MainContainer!.Resolve<IProportionImage>(ts.TagUsed);
            Text = "Display";
            _thisGrid = new Grid();
            Grid finalGrid = new Grid();
            _dominoList = _thisMod.GameBoard1!.DominoList;
            _dominoList.CollectionChanged += DominoCollectionChange;
            if (_dominoList.Count != 3)
                throw new BasicBlankException("Only 3 dominos are supported");
            if (_dominoList.First().DefaultSize.Width == 0)
                throw new BasicBlankException("The width can never be 0 for dominos");
            SKSize thisSize = _dominoList.First().DefaultSize.GetSizeUsed(thisP.Proportion);
            int pixels = (int)thisSize.Width / 2;
            for (int x = 0; x < 4; x++)
            {
                AddPixelColumn(_thisGrid, pixels);
            }
            RepopulateList();
            var thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(_thisGrid, thisRect);
            finalGrid.Children.Add(ThisDraw);
            finalGrid.Children.Add(_thisGrid);
            Content = finalGrid;
        }
        private DominosWPF<SimpleDominoInfo> FindControl(int index)
        {
            switch (index)
            {
                case 0:
                    return _firstGraphics!;
                case 1:
                    return _centerGraphics!;
                case 2:
                    return _secondGraphics!;
                default:
                    throw new BasicBlankException("Only 3 dominos are supported");
            }
        }
        private void RepopulateList()
        {
            _thisGrid!.Children.Clear();
            _firstGraphics = CreateGraphics(_dominoList.First());
            _secondGraphics = CreateGraphics(_dominoList.Last());
            _centerGraphics = CreateGraphics(_dominoList![1]);
            _dominoList[1].IsEnabled = false; //you can't ever click on the center one.
            _centerGraphics.IsEnabled = false;
            _centerGraphics.Focusable = false;
            _centerGraphics.IsHitTestVisible = false;
            AddControlToGrid(_thisGrid, _centerGraphics, 0, 1);
            AddControlToGrid(_thisGrid, _firstGraphics, 0, 0);
            AddControlToGrid(_thisGrid, _secondGraphics, 0, 2);
            Grid.SetColumnSpan(_firstGraphics, 2);
            Grid.SetColumnSpan(_secondGraphics, 2);
            Grid.SetColumnSpan(_centerGraphics, 2);
        }
        private void DominoCollectionChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                RepopulateList();
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Count != 1)
                    throw new BasicBlankException("Not sure when there are more than one to replace");
                int index = e.OldStartingIndex;
                var thisControl = FindControl(index);
                thisControl.DataContext = e.NewItems[0];
                if (index != 1)
                    thisControl.CommandParameter = e.NewItems[0]!;
                thisControl.RepaintManually(); //i think.
            }
        }
    }
}