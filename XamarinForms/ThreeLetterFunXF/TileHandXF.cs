using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.GraphicsCP;
using ThreeLetterFunCP.Logic;
using Xamarin.Forms;
namespace ThreeLetterFunXF
{
    public class TileHandXF : ContentView
    {
        private CustomBasicCollection<TileInformation>? _tileList;
        private TileBoardObservable? _tempMod;
        private StackLayout? _thisStack;
        public void Init(ThreeLetterFunVMData model)
        {
            _tempMod = model.TileBoard1;
            _tileList = _tempMod!.HandList;
            _tileList.CollectionChanged += TileList_CollectionChanged;
            _thisStack = new StackLayout();
            _thisStack.Orientation = StackOrientation.Horizontal;
            Content = _thisStack;
            PopulateControls();
        }
        private void TileList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateControls();
        }
        private void PopulateControls()
        {
            _thisStack!.Children.Clear();
            if (_tileList!.Count == 0)
                return;// if bug, obvious
            if (_tileList.Count != 2)
                throw new BasicBlankException("Must have only 2 tiles");
            foreach (var ThisTile in _tileList)
            {
                var ThisGraphics = GetGraphics(ThisTile);
                _thisStack.Children.Add(ThisGraphics);
            }
        }
        private TileCardXF GetGraphics(TileInformation thisTile)
        {
            TileCardXF thisGraphics = new TileCardXF();
            var thisBind = GetCommandBinding(nameof(TileBoardObservable.ObjectSingleClickCommand));
            thisGraphics.SetBinding(GraphicsCommand.CommandProperty, thisBind);
            thisGraphics.CommandParameter = thisTile;
            thisGraphics.SendSize(TileCP.TagUsed, thisTile);
            return thisGraphics; //well see what else we need.
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _tempMod;
            return thisBind;
        }
    }
}