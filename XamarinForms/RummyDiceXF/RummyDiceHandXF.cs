using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using RummyDiceCP;
using SkiaSharp;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace RummyDiceXF
{
    public class RummyDiceHandXF : BaseFrameXF
    {
        private CustomBasicCollection<RummyDiceInfo>? _handList;
        private StackLayout? _thisStack;
        private Grid? thisGrid;
        RummyDiceHandVM? _thisMod;
        public void LoadList(RummyDiceHandVM thisMod)
        {
            _thisMod = thisMod;
            BindingContext = thisMod;
            HorizontalOptions = LayoutOptions.FillAndExpand; //hopefully this works too.
            _handList = thisMod.HandList;
            _handList.CollectionChanged += HandList_CollectionChanged;
            Text = $"Temp Set {thisMod.Index}";
            SetBinding(IsEnabledProperty, new Binding(nameof(RummyDiceHandVM.IsEnabled)));
            RummyDiceInfo firstDice = new RummyDiceInfo();
            if (firstDice.HeightWidth == 0)
                throw new BasicBlankException("You must specify the height/width of the dice");
            IProportionImage thisI = new CustomProportionXF();
            SKSize firstSize = new SKSize(firstDice.HeightWidth, firstDice.HeightWidth);
            var sizeUsed = firstSize.GetSizeUsed(thisI.Proportion);
            Margin = new Thickness(3, 3, 3, 3);
            _thisStack = new StackLayout();
            _thisStack.Orientation = StackOrientation.Horizontal;
            _thisStack.Spacing = 0;
            Button thisBut = GetSmallerButton("Place Tiles", nameof(RummyDiceHandVM.BoardCommand));
            StackLayout finalStack = new StackLayout();
            finalStack.Orientation = StackOrientation.Horizontal;
            finalStack.Children.Add(thisBut);
            finalStack.Spacing = 2;
            finalStack.Children.Add(_thisStack);
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            _thisStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            thisBut.FontSize *= .7f;
            thisGrid = new Grid();
            SetBinding(IsVisibleProperty, new Binding(nameof(RummyDiceHandVM.Visible)));
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(finalStack);
            var thisRect = ThisFrame.GetControlArea();
            thisBut.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            _thisStack.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            PopulateControls(); //just in case there is something to start with.
            Content = thisGrid;
        }
        public void UpdateList(RummyDiceHandVM thisMod)
        {
            BindingContext = null;
            BindingContext = thisMod;
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
                RummyDiceGraphicsXF thisGraphics = new RummyDiceGraphicsXF();
                thisGraphics.SendDiceInfo(firstDice);
                var thisBind = GetCommandBinding(nameof(RummyDiceHandVM.DiceCommand));
                thisGraphics.SetBinding(RummyDiceGraphicsXF.CommandProperty, thisBind);
                thisGraphics.CommandParameter = firstDice;
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