using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.GraphicsCP;
using ThreeLetterFunCP.Logic;
namespace ThreeLetterFunWPF
{
    public class TileHandWPF : UserControl
    {
        private CustomBasicCollection<TileInformation>? _tileList;
        private TileBoardObservable? _tempMod;
        private StackPanel? _thisStack;
        public void Init(ThreeLetterFunVMData model)
        {
            //ThreeLetterFunViewModel thisMod = Resolve<ThreeLetterFunViewModel>();
            _tempMod = model.TileBoard1;
            _tileList = _tempMod!.HandList;
            _tileList.CollectionChanged += TileList_CollectionChanged;
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Horizontal;
            Content = _thisStack;
            PopulateControls();
        }
        //public void Update()
        //{
        //    ThreeLetterFunViewModel thisMod = Resolve<ThreeLetterFunViewModel>();
        //    _tempMod = thisMod.TileBoard1;
        //    _tileList = _tempMod!.HandList; //hook up again.
        //    _tileList.CollectionChanged -= TileList_CollectionChanged;
        //    _tileList.CollectionChanged += TileList_CollectionChanged;
        //    PopulateControls(); //this too.
        //}
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
                throw new Exception("Must have only 2 tiles");
            foreach (var ThisTile in _tileList)
            {
                var ThisGraphics = GetGraphics(ThisTile);
                _thisStack.Children.Add(ThisGraphics);
            }
        }
        private TileCardWPF GetGraphics(TileInformation thisTile)
        {
            TileCardWPF thisGraphics = new TileCardWPF();
            var thisBind = GetCommandBinding(nameof(TileBoardObservable.ObjectSingleClickCommand));
            thisGraphics.SetBinding(TileCardWPF.CommandProperty, thisBind);
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
