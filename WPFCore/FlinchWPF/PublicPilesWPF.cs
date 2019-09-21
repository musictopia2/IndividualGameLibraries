using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.GameGraphicsCP.Animations;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using FlinchCP;
using SkiaSharp;
using System.Collections.Specialized;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FlinchWPF
{
    public class PublicPilesWPF : UserControl, IHandleAsync<AnimateCardInPileEventModel<FlinchCardInformation>>
    {
        private CustomBasicCollection<BasicPileInfo<FlinchCardInformation>>? _pileList;

        private WrapPanel? _thisStack;

        private PublicPilesViewModel? _thisMod;

        private IndividualPileWPF? FindControl(BasicPileInfo<FlinchCardInformation> thisPile)
        {
            foreach (var thisCon in _thisStack!.Children)
            {
                var news = (IndividualPileWPF)thisCon!;
                if (news.DataContext.Equals(thisPile) == true)
                    return news;
            }
            return null;
        }
        private CustomCanvas? _thisCanvas;
        private AnimateImageClass? _animates;
        private Grid? _parentGrid;
        public void StartAnimationListener(string animationTag) // has to be manual
        {
            if (_thisCanvas != null)
                throw new BasicBlankException("You already started an animation listener");
            _thisCanvas = new CustomCanvas();
            _parentGrid!.Children.Add(_thisCanvas);
            _animates = new AnimateImageClass();
            _animates.LongestTravelTime = 200;
            _animates.GameBoard = _thisCanvas;
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this, animationTag);
        }
        private SKPoint GetStartLocation()
        {
            return new SKPoint(0, 0); // i think that 0, 0 is fine.
        }
        private SKPoint GetBottomLocation()
        {
            double thisWidth;
            double thisHeight;
            thisWidth = _thisCanvas!.ActualWidth;
            thisHeight = _thisCanvas.ActualHeight;
            thisWidth += _thisStack!.ItemWidth;
            thisHeight += _thisStack.ItemHeight;
            return new SKPoint((float)thisWidth, (float)thisHeight);
        }
        private SKPoint GetCardLocation(IndividualPileWPF thisControl) // harder
        {
            var thisIndex = _thisStack!.Children.IndexOf(thisControl);
            if (thisIndex == -1)
                throw new BasicBlankException("It should have found the card location for the individual control for animations");
            int maxColumns;
            maxColumns = _thisCanvas!.ActualWidth.RoundToLowerNumber();
            var (row, column) = GetRowColumn(thisIndex, maxColumns);
            var thisHeight = _thisStack.ItemHeight * row;
            var thisWidth = _thisStack.ItemWidth * column;
            return new SKPoint((float)thisWidth, (float)thisHeight);
        }
        private (int Row, int Column) GetRowColumn(int index, int maxColumns)
        {
            int x;
            int y = default;
            int z = default;
            do
            {
                var loopTo = maxColumns;
                for (x = 1; x <= loopTo; x++)
                {
                    if (z == index)
                        return (y + 1, x);
                    z += 1;
                }

                y += 1;
            }
            while (true); // because 0 based
        }
        private async Task PrepareToAnimateAsync(AnimateCardInPileEventModel<FlinchCardInformation> data) // don't need index ever for this.
        {
            var thisPile = FindControl(data.ThisPile!);
            var topLocation = GetStartLocation();
            var bottomLocaton = GetBottomLocation();
            var cardLocation = GetCardLocation(thisPile!);
            switch (data.Direction)
            {
                case EnumAnimcationDirection.StartCardToDown:
                    {
                        _animates!.LocationFrom = cardLocation;
                        _animates.LocationTo = bottomLocaton;
                        break;
                    }

                case EnumAnimcationDirection.StartCardToUp:
                    {
                        _animates!.LocationFrom = cardLocation;
                        _animates.LocationTo = topLocation;
                        break;
                    }

                case EnumAnimcationDirection.StartDownToCard:
                    {
                        _animates!.LocationFrom = bottomLocaton;
                        _animates.LocationTo = cardLocation;
                        break;
                    }

                case EnumAnimcationDirection.StartUpToCard:
                    {
                        _animates!.LocationFrom = topLocation;
                        _animates.LocationTo = cardLocation;
                        break;
                    }
            }
            CardGraphicsWPF thisControl = new CardGraphicsWPF();
            thisControl.SendSize("", data.ThisCard!);
            thisControl.Drew = false;
            thisControl.IsSelected = false;
            thisControl.DoInvalidate();
            await _animates!.DoAnimateAsync(thisControl); // i think
            thisControl.Visibility = Visibility.Collapsed;
            _thisCanvas!.Children.Clear(); //try this too.
        }
        public async Task HandleAsync(AnimateCardInPileEventModel<FlinchCardInformation> message)
        {
            await PrepareToAnimateAsync(message);
        }
        public void Init(PublicPilesViewModel mod)
        {
            _thisMod = mod;
            _parentGrid = new Grid();
            _thisStack = new WrapPanel();
            var tempCard = new FlinchCardInformation();
            var thisP = Resolve<IProportionImage>();
            _thisStack.ItemHeight = tempCard.DefaultSize.Height * thisP.Proportion;
            _thisStack.ItemWidth = tempCard.DefaultSize.Width * thisP.Proportion;
            _thisStack.Orientation = Orientation.Horizontal; // start out horizontally
            _pileList = _thisMod.PileList; // i think its that simple.
            _pileList.CollectionChanged += PileList_CollectionChanged;
            foreach (var thisPile in _pileList)
            {
                IndividualPileWPF thisCon = new IndividualPileWPF();
                thisCon.ThisPile = thisPile;
                thisCon.MainMod = _thisMod;
                thisCon.Init(); // i think i needed this as well
                _thisStack.Children.Add(thisCon);
            }
            _parentGrid.Children.Add(_thisStack);
            Content = _parentGrid;
        }
        public void UpdateLists(PublicPilesViewModel mod)
        {
            _thisMod = mod;
            _thisStack!.Children.Clear(); //best to just redo this time.
            _pileList!.CollectionChanged -= PileList_CollectionChanged;
            _pileList = _thisMod.PileList;
            _pileList.CollectionChanged += PileList_CollectionChanged;
            foreach (var thisPile in _pileList)
            {
                IndividualPileWPF thisCon = new IndividualPileWPF();
                thisCon.ThisPile = thisPile;
                thisCon.MainMod = _thisMod;
                thisCon.Init(); // i think i needed this as well
                _thisStack.Children.Add(thisCon);
            }
        }
        private void PileList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if ((int)e.Action == (int)NotifyCollectionChangedAction.Reset)
            {
                _thisStack!.Children.Clear(); // hopefully this simple (nothing else).  well see
                return;
            }
            if ((int)e.Action == (int)NotifyCollectionChangedAction.Remove)
            {
                // this means one got removed.  for this, will only remove one alone.
                foreach (var thisItem in e.OldItems)
                {
                    var thisPile = (BasicPileInfo<FlinchCardInformation>)thisItem!;
                    var thisCon = FindControl(thisPile);
                    _thisStack!.Children.Remove(thisCon);
                }
                return;
            }
            if ((int)e.Action == (int)NotifyCollectionChangedAction.Add)
            {
                foreach (var thisItem in e.NewItems)
                {
                    var thisPile = (BasicPileInfo<FlinchCardInformation>)thisItem!;
                    IndividualPileWPF thisCon = new IndividualPileWPF();
                    thisCon.ThisPile = thisPile;
                    thisCon.MainMod = _thisMod;
                    thisCon.Init();
                    _thisStack!.Children.Add(thisCon);
                }
                return;
            }
        }
    }
}