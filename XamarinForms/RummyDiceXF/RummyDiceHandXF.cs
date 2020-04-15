using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using RummyDiceCP.Data;
using RummyDiceCP.ViewModels;
using RummyDiceCP.Logic;

namespace RummyDiceXF
{
    public class RummyDiceHandXF : BaseFrameXF
    {
        private CustomBasicCollection<RummyDiceInfo>? _handList;
        private StackLayout? _thisStack;
        private Grid? _thisGrid;
        RummyDiceHandVM? _thisMod;
        private RummyDiceMainGameClass? _mainGame;
        public void LoadList(RummyDiceHandVM thisMod, RummyDiceMainGameClass mainGame)
        {
            _thisMod = thisMod;
            _mainGame = mainGame;
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
            Button thisBut = GetSmallerButton("Place Tiles", "");
            thisBut.Command = thisMod.BoardCommand; //i think this way this time.
            StackLayout finalStack = new StackLayout();
            finalStack.Orientation = StackOrientation.Horizontal;
            finalStack.Children.Add(thisBut);
            finalStack.Spacing = 2;
            finalStack.Children.Add(_thisStack);
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            _thisStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            thisBut.FontSize *= .7f;
            _thisGrid = new Grid();
            _thisGrid.Children.Add(ThisDraw);
            _thisGrid.Children.Add(finalStack);
            var thisRect = ThisFrame.GetControlArea();
            thisBut.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            _thisStack.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            PopulateControls(); //just in case there is something to start with.
            Content = _thisGrid;
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
                thisGraphics.SendDiceInfo(firstDice, _mainGame!.MainBoard1);
                thisGraphics.Command = _thisMod!.DiceCommand;
                _thisStack.Children.Add(thisGraphics);
                x += 1;
            }
        }
        
    }
}