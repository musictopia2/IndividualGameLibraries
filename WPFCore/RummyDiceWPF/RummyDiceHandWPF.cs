using BasicControlsAndWindowsCore.Helpers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using RummyDiceCP.Data;
using RummyDiceCP.Logic;
using RummyDiceCP.ViewModels;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace RummyDiceWPF
{
    public class RummyDiceHandWPF : BaseFrameWPF
    {
        private CustomBasicCollection<RummyDiceInfo>? _handList;
        private StackPanel? _thisStack;
        private Grid? _thisGrid;
        RummyDiceHandVM? _thisMod;
        private RummyDiceMainGameClass? _mainGame;
        public void LoadList(RummyDiceHandVM thisMod, RummyDiceMainGameClass mainGame)
        {
            _thisMod = thisMod;
            _mainGame = mainGame;
            DataContext = thisMod;
            _handList = thisMod.HandList;
            _handList.CollectionChanged += HandList_CollectionChanged;
            Text = $"Temp Set {thisMod.Index}";
            SetBinding(IsEnabledProperty, nameof(RummyDiceHandVM.IsEnabled));
            RummyDiceInfo firstDice = new RummyDiceInfo();
            if (firstDice.HeightWidth == 0)
                throw new BasicBlankException("You must specify the height/width of the dice");
            IProportionImage thisI = new CustomProportionWPF();
            SKSize firstSize = new SKSize(firstDice.HeightWidth, firstDice.HeightWidth);
            var sizeUsed = firstSize.GetSizeUsed(thisI.Proportion);
            Grid firstGrid = new Grid();
            GridHelper.AddPixelRow(firstGrid, (int)sizeUsed.Height + 50);
            GridHelper.AddLeftOverRow(firstGrid, 1);
            Margin = new Thickness(3, 3, 3, 3);
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Horizontal;
            Button thisBut = GetGamingButton("Place Tiles", nameof(RummyDiceHandVM.BoardCommand));
            //i think since its a different view model, has to do manually.
            thisBut.Name = "";
            thisBut.Command = thisMod.BoardCommand;
            thisBut.Margin = new Thickness(5, 5, 5, 5);
            GridHelper.AddControlToGrid(firstGrid, _thisStack, 0, 0);
            GridHelper.AddControlToGrid(firstGrid, thisBut, 1, 0);
            _thisGrid = new Grid();
            //hopefully does not need visible.  if we need visible, rethink.


            //SetBinding(VisibilityProperty, GetVisibleBinding(nameof(RummyDiceHandVM.Visible)));
            _thisGrid.Children.Add(ThisDraw);
            _thisGrid.Children.Add(firstGrid);
            var thisRect = ThisFrame.GetControlArea();
            _thisStack.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            Width = 1520; //different on xamarin forms.
            Height = 250;
            PopulateControls(); //just in case there is something to start with.
            Content = _thisGrid;
        }
        public void UpdateList(RummyDiceHandVM thisMod)
        {
            DataContext = null;
            DataContext = thisMod;
            _thisMod = thisMod;
            _handList!.CollectionChanged -= HandList_CollectionChanged;
            _handList = thisMod.HandList; //rehook up again.
            PopulateControls();
        }
        public int Index { get; set; }
        private void HandList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateControls(); //try to just populate controls everytime.
        }
        private void PopulateControls()
        {
            _thisStack!.Children.Clear();
            int x = 0;
            foreach (var firstDice in _handList!)
            {
                RummyDiceGraphicsWPF thisGraphics = new RummyDiceGraphicsWPF();
                thisGraphics.SendDiceInfo(firstDice, _mainGame!.MainBoard1);
                thisGraphics.Command = _thisMod!.DiceCommand;
                thisGraphics.Margin = new Thickness(5, 5, 5, 5); //hopefully this works as well (?)
                _thisStack.Children.Add(thisGraphics);
                x += 1;
            }
        }
        
    }
}
