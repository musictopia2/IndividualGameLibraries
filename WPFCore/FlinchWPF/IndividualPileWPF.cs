using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.Exceptions;
using FlinchCP;
using SkiaSharp;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace FlinchWPF
{
    public class IndividualPileWPF : UserControl
    {
        public BasicPileInfo<FlinchCardInformation>? ThisPile;
        public PublicPilesViewModel? MainMod;
        private DeckObservableDict<FlinchCardInformation>? _thisList;
        private CardGraphicsWPF? _thisGraphics;
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
            DataContext = ThisPile;
            _thisList = ThisPile.ObjectList;
            _thisGraphics = new CardGraphicsWPF();
            FlinchCardInformation tempCard;
            if (ThisPile.ObjectList.Count == 0)
                throw new BasicBlankException("I think needs at least one card for this(?)");
            tempCard = ThisPile.ObjectList.Last();
            _thisGraphics.SendSize("", tempCard);
            _thisGraphics.Visibility = Visibility.Visible; // try this as well
            Binding thisBind = new Binding(nameof(PublicPilesViewModel.PileCommand));
            thisBind.Source = MainMod; // has to be that one  still has to be this one.
            _thisGraphics.SetBinding(CardGraphicsWPF.CommandProperty, thisBind);
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
            _thisGraphics!.SendSize("", thisCard);
        }
        private void MainMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PublicPilesViewModel.IsEnabled))
                CalculateEnables();
        }
    }
}