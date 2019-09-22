using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicControlsAndWindowsCore.Helpers;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using RummyDiceCP;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace RummyDiceWPF
{
    public class RummyDiceHandWPF : BaseFrameWPF
    {
        private CustomBasicCollection<RummyDiceInfo>? _handList;
        private StackPanel? _thisStack;
        private Grid? thisGrid;
        RummyDiceHandVM? _thisMod;
        public void LoadList(RummyDiceHandVM thisMod)
        {
            _thisMod = thisMod;
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
            thisBut.Margin = new Thickness(5, 5, 5, 5);
            GridHelper.AddControlToGrid(firstGrid, _thisStack, 0, 0);
            GridHelper.AddControlToGrid(firstGrid, thisBut, 1, 0);
            thisGrid = new Grid();
            SetBinding(VisibilityProperty, GetVisibleBinding(nameof(RummyDiceHandVM.Visible)));
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(firstGrid);
            var thisRect = ThisFrame.GetControlArea();
            _thisStack.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            Width = 1520; //different on xamarin forms.
            Height = 250;
            PopulateControls(); //just in case there is something to start with.
            Content = thisGrid;
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
                thisGraphics.SendDiceInfo(firstDice);
                var thisBind = GetCommandBinding(nameof(RummyDiceHandVM.DiceCommand));
                thisGraphics.SetBinding(RummyDiceGraphicsWPF.CommandProperty, thisBind);
                thisGraphics.CommandParameter = firstDice;
                thisGraphics.Margin = new Thickness(5, 5, 5, 5); //hopefully this works as well (?)
                _thisStack.Children.Add(thisGraphics);
                x += 1;
            }
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
    }
}