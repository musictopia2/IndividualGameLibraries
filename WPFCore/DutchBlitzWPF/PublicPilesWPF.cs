using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using DutchBlitzCP;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Media;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace DutchBlitzWPF
{
    public class PublicPilesWPF : UserControl
    {
        private CustomBasicCollection<BasicPileInfo<DutchBlitzCardInformation>>? _pileList;

        private WrapPanel? _thisStack;

        private PublicViewModel? _thisMod;

        private IndividualPileWPF? FindControl(BasicPileInfo<DutchBlitzCardInformation> thisPile)
        {
            foreach (object? thisCon in _thisStack!.Children)
            {
                var news = (IndividualPileWPF)thisCon!;
                if (news.DataContext.Equals(thisPile) == true)
                    return news;
            }
            return null;
        }
        public void Init(PublicViewModel mod)
        {
            Background = Brushes.Transparent;
            _thisMod = mod;
            _thisStack = new WrapPanel();
            var tempCard = new DutchBlitzCardInformation();
            var thisP = Resolve<IProportionImage>();
            _thisStack.ItemHeight = tempCard.DefaultSize.Height * thisP.Proportion;
            _thisStack.ItemWidth = tempCard.DefaultSize.Width * thisP.Proportion;
            _thisStack.Orientation = Orientation.Horizontal; // start out horizontally
            MouseUp += PublicPilesWPF_MouseUp;
            _pileList = _thisMod.PileList; // i think its that simple.
            _pileList.CollectionChanged += PileList_CollectionChanged;
            foreach (var thisPile in _pileList)
            {
                IndividualPileWPF thisCon = new IndividualPileWPF();
                thisCon.IsHitTestVisible = true;
                thisCon.ThisPile = thisPile;
                thisCon.MainMod = _thisMod;
                thisCon.Init(); // i think i needed this as well
                _thisStack.Children.Add(thisCon);
            }
            Content = _thisStack;
        }
        private void PublicPilesWPF_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
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