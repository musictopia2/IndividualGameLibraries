using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace AccordianSolitaireCP
{
    public class GameBoard : HandViewModel<AccordianSolitaireCardInfo>
    {

        public int Score
        {
            get => _mainGame.SaveRoot.Score;
            set => _mainGame.SaveRoot.Score = value;
        }
        private int DeckSelected
        {
            get => _mainGame.SaveRoot.DeckSelected;
            set => _mainGame.SaveRoot.DeckSelected = value;
        }
        private int NewestOne
        {
            get => _mainGame.SaveRoot.NewestOne;
            set => _mainGame.SaveRoot.NewestOne = value;
        }
        public void SaveGame() => _mainGame.SaveRoot.HandList = HandList.ToRegularDeckDict();
        public void ReloadSavedGame() => HandList.ReplaceRange(_mainGame.SaveRoot.HandList);
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
            _thisRedo.RedoList();
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
        public void NewGame(IDeckDict<AccordianSolitaireCardInfo> thisCol)
        {
            HandList.Clear();
            PopulateObjects(thisCol);
            if (HandList.Count != 52)
                throw new BasicBlankException("The hand must have 52 cards");
            HandList.First().IsUnknown = false;
            DeckSelected = 0;
            Score = 1;
            NewestOne = 0;
        }

        private readonly AccordianSolitaireGameClass _mainGame;
        private readonly IRedo _thisRedo;
        public GameBoard(IBasicGameVM ThisMod) : base(ThisMod)
        {
            _mainGame = ThisMod.MainContainer!.Resolve<AccordianSolitaireGameClass>();
            _thisRedo = ThisMod.MainContainer.Resolve<IRedo>();
            AutoSelect = EnumAutoType.None;
            Text = "Reserve Pile";
        }
    }
}