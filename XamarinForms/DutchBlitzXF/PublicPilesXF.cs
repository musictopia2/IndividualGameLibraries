using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using DutchBlitzCP;
using System.Collections.Specialized;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
namespace DutchBlitzXF
{
    public class PublicPilesXF : ContentView
    {
        private CustomBasicCollection<BasicPileInfo<DutchBlitzCardInformation>>? _pileList;

        private StackLayout? _thisStack;

        private PublicViewModel? _thisMod;
        private SKCanvasView? _drawControl;

        private IndividualPileXF? FindControl(BasicPileInfo<DutchBlitzCardInformation> thisPile)
        {
            foreach (object? thisCon in _thisStack!.Children)
            {
                var news = (IndividualPileXF)thisCon!;
                if (news.BindingContext.Equals(thisPile) == true)
                    return news;
            }
            return null;
        }
        public void Init(PublicViewModel mod)
        {
            _thisMod = mod;
            _thisStack = new StackLayout();
            var tempCard = new DutchBlitzCardInformation();
            var thisP = Resolve<IProportionImage>();
            HeightRequest = tempCard.DefaultSize.Height * thisP.Proportion;
            _thisStack.Orientation = StackOrientation.Horizontal; // start out horizontally
            _thisStack.HorizontalOptions = LayoutOptions.Start;
            _thisStack.VerticalOptions = LayoutOptions.Start;
            Grid grid = new Grid();
            _drawControl = new SKCanvasView();
            _drawControl.EnableTouchEvents = true;
            _drawControl.Touch += TouchEvent;
            _pileList = _thisMod.PileList; // i think its that simple.
            _pileList.CollectionChanged += PileList_CollectionChanged;
            foreach (var thisPile in _pileList)
            {
                IndividualPileXF thisCon = new IndividualPileXF();
                thisCon.ThisPile = thisPile;
                thisCon.MainMod = _thisMod;
                thisCon.Init(); // i think i needed this as well
                _thisStack.Children.Add(thisCon);
            }
            grid.Children.Add(_drawControl);
            grid.Children.Add(_thisStack);
            Content = grid;
        }

        private void TouchEvent(object sender, SKTouchEventArgs e)
        {
            if (_thisMod == null)
                return;
            if (_thisMod.NewCommand.CanExecute(null!) == false)
                return;
            _thisMod.NewCommand.Execute(null!);
        }
        public void UpdateLists(PublicViewModel mod)
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
            if ((int)e.Action == (int)NotifyCollectionChangedAction.Reset)
            {
                _thisStack!.Children.Clear(); // hopefully this simple (nothing else).  well see
                return;
            }
            if ((int)e.Action == (int)NotifyCollectionChangedAction.Remove)
            {
                foreach (var thisItem in e.OldItems)
                {
                    var thisPile = (BasicPileInfo<DutchBlitzCardInformation>)thisItem!;
                    var thisCon = FindControl(thisPile);
                    _thisStack!.Children.Remove(thisCon);
                }
                return;
            }
            if (e.Action == (int)NotifyCollectionChangedAction.Add)
            {
                foreach (var ThisItem in e.NewItems)
                {
                    var thisPile = (BasicPileInfo<DutchBlitzCardInformation>)ThisItem!;
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
