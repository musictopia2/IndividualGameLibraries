using AccordianSolitaireCP.EventModels;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccordianSolitaireCP.Data
{
    public class GameBoard : HandObservable<AccordianSolitaireCardInfo>
    {

        public int Score
        {
            get => _saveRoot!.Score;
            set => _saveRoot!.Score = value;
        }
        private int DeckSelected
        {
            get => _saveRoot!.DeckSelected;
            set => _saveRoot!.DeckSelected = value;
        }
        private int NewestOne
        {
            get => _saveRoot!.NewestOne;
            set => _saveRoot!.NewestOne = value;
        }
        public void SaveGame() => _saveRoot!.HandList = HandList.ToRegularDeckDict();
        public void ReloadSavedGame(AccordianSolitaireSaveInfo saveroot)
        {
            _saveRoot = saveroot;
            HandList.ReplaceRange(_saveRoot.HandList);
        }
        public void SelectUnselectCard(AccordianSolitaireCardInfo thisCard)
        {
            if (NewestOne > 0)
            {
                var tempCard = HandList.GetSpecificItem(NewestOne);
                tempCard.Drew = false;
                NewestOne = 0;
            }
            if (thisCard.IsSelected)
            {
                thisCard.IsSelected = false;
                DeckSelected = 0;
            }
            else
            {
                thisCard.IsSelected = true;
                DeckSelected = thisCard.Deck;
            }
        }
        public override void UnselectAllObjects()
        {
            base.UnselectAllObjects();
            _saveRoot!.DeckSelected = 0; //one possible bug.  not sure if we need to save state even when unselecting objects (?)
        }
        public void MakeMove(AccordianSolitaireCardInfo thisCard)
        {
            Score++;
            var newCard = HandList.GetSpecificItem(DeckSelected);
            newCard.Drew = true;
            newCard.IsSelected = false;
            HandList.ReplaceCardPlusRemove(thisCard.Deck, DeckSelected);
            NewestOne = DeckSelected;
            DeckSelected = 0;
            if (HandList.ObjectExist(NewestOne) == false)
                throw new BasicBlankException("Replacing failed");
            _aggregator.Publish(new RedoEventModel());
        }
        private bool ValidSoFar(AccordianSolitaireCardInfo firstCard, AccordianSolitaireCardInfo secondCard)
        {
            var firstNumber = HandList.FindIndexByDeck(firstCard.Deck);
            var secondNumber = HandList.FindIndexByDeck(secondCard.Deck);
            if (secondNumber > firstNumber)
                return false;
            if (firstNumber - 1 == secondNumber)
                return true;
            return firstNumber - 3 == secondNumber;
        }
        public bool IsValidMove(AccordianSolitaireCardInfo thisCard)
        {
            //return true; //until i get to the root of the problem.
            if (DeckSelected == thisCard.Deck)
                throw new BasicBlankException("The same one selected was used.  Therefore, it should have unselected the card instead");
            var firstCard = HandList.GetSpecificItem(DeckSelected);
            if (ValidSoFar(firstCard, thisCard) == false)
                return false;
            if (firstCard.Suit == thisCard.Suit)
                return true;
            return firstCard.Value == thisCard.Value;
        }

        public bool IsCardSelected(AccordianSolitaireCardInfo thisCard)
        {
            if (DeckSelected == thisCard.Deck)
                return false;
            return DeckSelected > 0;
        }
        public void NewGame(IDeckDict<AccordianSolitaireCardInfo> thisCol, AccordianSolitaireSaveInfo saveInfo)
        {
            _saveRoot = saveInfo;
            HandList.Clear();
            PopulateObjects(thisCol);
            if (HandList.Count != 52)
                throw new BasicBlankException("The hand must have 52 cards");
            HandList.First().IsUnknown = false;
            DeckSelected = 0;
            Score = 1;
            NewestOne = 0;
        }
        private AccordianSolitaireSaveInfo? _saveRoot;
        private readonly IEventAggregator _aggregator;

        //private readonly AccordianSolitaireGameClass _mainGame;
        //private readonly IRedo _thisRedo;
        public GameBoard(CommandContainer command, IEventAggregator aggregator) : base(command)
        {
            AutoSelect = EnumAutoType.None;
            Text = "Reserve Pile";
            _aggregator = aggregator;
        }
    }
}
