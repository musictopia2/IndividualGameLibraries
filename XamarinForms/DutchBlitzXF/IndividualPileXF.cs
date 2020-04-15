using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.Exceptions;
using DutchBlitzCP.Cards;
using DutchBlitzCP.Piles;
using SkiaSharp;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
namespace DutchBlitzXF
{
    public class IndividualPileXF : ContentView
    {
        public BasicPileInfo<DutchBlitzCardInformation>? ThisPile;
        public PublicViewModel? MainMod;
        private DeckObservableDict<DutchBlitzCardInformation>? _thisList;
        private CardGraphicsXF? _thisGraphics;
        internal SKPoint CardLocation { get; set; } // this is needed for the animations part.
        private void CalculateEnables()
        {
            if (MainMod!.IsEnabled == false)
                IsEnabled = false;
            else if (ThisPile!.IsEnabled == false)
                IsEnabled = false;
            else
                IsEnabled = true;
        }
        public void Init()
        {
            if (ThisPile == null)
                throw new BasicBlankException("You must have sent in a pile before calling the init");
            if (MainMod == null)
                throw new BasicBlankException("You must have sent in the main multiple piles view model so its able to populate the actual card");
            BindingContext = ThisPile;
            _thisList = ThisPile.ObjectList;
            _thisGraphics = new CardGraphicsXF();
            DutchBlitzCardInformation tempCard;
            if (ThisPile.ObjectList.Count == 0)
                throw new BasicBlankException("I think needs at least one card for this(?)");
            tempCard = ThisPile.ObjectList.Last();
            _thisGraphics.SendSize("", tempCard);
            _thisGraphics.IsVisible = true; // try this as well
            Binding ThisBind = new Binding(nameof(PublicViewModel.PileCommand));
            ThisBind.Source = MainMod; // has to be that one  still has to be this one.
            _thisGraphics.SetBinding(CardGraphicsXF.CommandProperty, ThisBind);
            _thisGraphics.CommandParameter = ThisPile; // this time, the parameter is the pile, not the card
            CardLocation = new SKPoint(0, 0);
            _thisGraphics.Init(); // this should be okay.  will do the right thing afterall
            CalculateEnables();
            MainMod.PropertyChanged += MainMod_PropertyChanged;
            _thisList.CollectionChanged += ThisList_CollectionChanged;
            Content = _thisGraphics; // i think its this simple for this one.
        }
        private void ThisList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ThisPile!.ObjectList.Count == 0)
                throw new Exception("Should not have 0 cards for this.  Should have removed instead");
            var thisCard = ThisPile.ObjectList.Last();
            _thisGraphics!.BindingContext = null;
            _thisGraphics.SendSize("", thisCard);
        }
        private void MainMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PublicViewModel.IsEnabled))
                CalculateEnables();
        }
    }
}