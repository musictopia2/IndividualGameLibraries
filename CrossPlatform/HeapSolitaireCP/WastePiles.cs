using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplePilesViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
namespace HeapSolitaireCP
{
    public class WastePiles : BasicMultiplePilesCP<HeapSolitaireCardInfo>
    {
        private int PreviousSelected
        {
            set => _mainGame.SaveRoot.PreviousSelected = value;
            get => _mainGame.SaveRoot.PreviousSelected;
        }

        public void ClearBoard(DeckRegularDict<HeapSolitaireCardInfo> thisCol)
        {
            if (thisCol.Count != 91)
                throw new BasicBlankException("The collection must have 91 cards");
            base.ClearBoard();
            int y = 0;
            PileList!.ForEach(thisPile =>
            {
                for (int x = 0; x < 4; x++)
                {
                    y++;
                    if (y > thisCol.Count)
                    {
                        FinishClearing();
                        return;
                    }
                    thisPile.TempList.Add(thisCol[y - 1]);
                }
            });
        }
        public override void SelectPile(int Index)
        {
            if (Index == PreviousSelected)
                return;
            if (PreviousSelected > -1)
                SelectUnselectSinglePile(PreviousSelected);
            PreviousSelected = Index;
            SelectUnselectSinglePile(Index);
        }
        public override void RemoveCardFromPile(int Index)
        {
            base.RemoveCardFromPile(Index);
            PreviousSelected = -1;
            PileList!.ForEach(thisPile =>
            {
                thisPile.IsSelected = false;
                thisPile.ObjectList.ForEach(thisCard => thisCard.IsSelected = false);
            });
        }
        public HeapSolitaireCardInfo GetCard()
        {
            var thisCard = GetLastCard(PreviousSelected);
            if (thisCard.Deck == 0)
                throw new BasicBlankException($"There are no cards to get for {PreviousSelected}");
            return thisCard;
        }
        public bool DidSelectCard => PreviousSelected > -1;
        private readonly HeapSolitaireGameClass _mainGame;
        public WastePiles(IBasicGameVM ThisMod) : base(ThisMod)
        {
            Style = EnumStyleList.HasList;
            HasFrame = false;
            HasText = false;
            Columns = 5;
            Rows = 5;
            LoadBoard();
            RemoveLastDiscardPiles(2);
            _mainGame = ThisMod.MainContainer!.Resolve<HeapSolitaireGameClass>();
        }
        protected override void PossiblePileChangeWhenClearingBoard(BasicPileInfo<HeapSolitaireCardInfo> ThisPile)
        {
            ThisPile.IsSelected = false;
            ThisPile.IsEnabled = true;
        }
        private void FinishClearing()
        {
            PileList!.ForEach(thisPile => thisPile.ObjectList.ReplaceRange(thisPile.TempList));
        }
        protected override bool CanClearCardsAutomatically()
        {
            return false;
        }
        protected override void AfterRemoveCardFromPile(BasicPileInfo<HeapSolitaireCardInfo> ThisPile)
        {
            if (ThisPile.ObjectList.Count == 0)
                ThisPile.IsEnabled = false;
        }
        protected override bool CanAutoUnselect()
        {
            return false;
        }
    }
}