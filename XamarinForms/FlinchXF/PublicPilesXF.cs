using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System.Collections.Specialized;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using FlinchCP.Cards;
using FlinchCP.Piles;

namespace FlinchXF
{
    public class PublicPilesXF : ContentView, IHandleAsync<AnimateCardInPileEventModel<FlinchCardInformation>>
    {
        private CustomBasicCollection<BasicPileInfo<FlinchCardInformation>>? _pileList;

        private StackLayout? _thisStack; //decided to use stack layout.

        private PublicPilesViewModel? _thisMod;

        private IndividualPileXF? FindControl(BasicPileInfo<FlinchCardInformation> thisPile)
        {
            foreach (var thisCon in _thisStack!.Children)
            {
                var news = (IndividualPileXF)thisCon!;
                if (news.BindingContext.Equals(thisPile) == true)
                    return news;
            }
            return null;
        }
        private CustomCanvasXF? _thisCanvas;
        private AnimateImageClass? _animates;
        private Grid? _parentGrid;
        public void StartAnimationListener(string animationTag) // has to be manual
        {
            if (_thisCanvas != null)
                throw new BasicBlankException("You already started an animation listener");
            _thisCanvas = new CustomCanvasXF();
            _thisCanvas.HorizontalOptions = LayoutOptions.Start;
            _thisCanvas.VerticalOptions = LayoutOptions.Start;
            _parentGrid!.HorizontalOptions = LayoutOptions.Start;
            _parentGrid.VerticalOptions = LayoutOptions.Start;
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
            thisWidth = _thisCanvas!.WidthRequest; //could be iffy (?)
            thisHeight = _thisCanvas.HeightRequest;
            thisWidth += _sizeUsed.Width;
            thisHeight += _sizeUsed.Height;
            return new SKPoint((float)thisWidth, (float)thisHeight);
        }
        private SKPoint GetCardLocation(IndividualPileXF thisControl) // harder
        {
            var thisIndex = _thisStack!.Children.IndexOf(thisControl);
            if (thisIndex == -1)
                throw new BasicBlankException("It should have found the card location for the individual control for animations");
            return new SKPoint(_sizeUsed.Width * thisIndex, 0);
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
            CardGraphicsXF thisControl = new CardGraphicsXF();
            thisControl.SendSize("", data.ThisCard!);
            thisControl.Drew = false;
            thisControl.IsSelected = false;
            thisControl.DoInvalidate();
            await _animates!.DoAnimateAsync(thisControl); // i think
            thisControl.IsVisible = false;
            _thisCanvas!.Children.Clear(); //try this too.
        }
        public async Task HandleAsync(AnimateCardInPileEventModel<FlinchCardInformation> message)
        {
            await PrepareToAnimateAsync(message);
        }
        private SKSize _sizeUsed;
        public void Init(PublicPilesViewModel mod)
        {
            _thisMod = mod;
            _parentGrid = new Grid();
            _thisStack = new StackLayout();
            var tempCard = new FlinchCardInformation();
            var thisP = Resolve<IProportionImage>();
            _sizeUsed = tempCard.DefaultSize.GetSizeUsed(thisP.Proportion);
            _thisStack.Orientation = StackOrientation.Horizontal; // start out horizontally
            _pileList = _thisMod.PileList; // i think its that simple.
            _pileList.CollectionChanged += PileList_CollectionChanged;
            HeightRequest = _sizeUsed.Height;
            foreach (var thisPile in _pileList)
            {
                IndividualPileXF thisCon = new IndividualPileXF();
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
                IndividualPileXF thisCon = new IndividualPileXF();
                thisCon.ThisPile = thisPile;
                thisCon.MainMod = _thisMod;
                thisCon.Init(); // i think i needed this as well
                _thisStack.Children.Add(thisCon);
            }
        }
        private void PileList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _thisStack!.Children.Clear(); // hopefully this simple (nothing else).  well see
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
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
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var thisItem in e.NewItems)
                {
                    var thisPile = (BasicPileInfo<FlinchCardInformation>)thisItem!;
                    IndividualPileXF thisCon = new IndividualPileXF();
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