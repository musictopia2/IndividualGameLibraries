using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplePilesViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
namespace HeapSolitaireCP
{
    public class MainPiles : BasicMultiplePilesCP<HeapSolitaireCardInfo>
    {

        public int Score
        {
            set => _mainGame.SaveRoot.Score = value;
            get => _mainGame.SaveRoot.Score;
        }
        private readonly HeapSolitaireGameClass _mainGame;
        public MainPiles(IBasicGameVM ThisMod) : base(ThisMod)
        {
            _mainGame = ThisMod.MainContainer!.Resolve<HeapSolitaireGameClass>();
            Rows = 1;
            HasText = true;
            HasFrame = true;
            Columns = 13;
            Style = EnumStyleList.HasList;
            LoadBoard();
            PileList!.ForEach(thisPile => thisPile.Text = "Start");
        }
        public void RefreshInfo()
        {
            PileList!.ForEach(thisPile =>
            {
                int lefts = 8 - thisPile.ObjectList.Count;
                thisPile.Text = $"{lefts} Left";
                thisPile.IsEnabled = lefts > 0;
            });
        }
        public override void AddCardToPile(int Pile, HeapSolitaireCardInfo ThisCard)
        {
            base.AddCardToPile(Pile, ThisCard);
            Score++;
            RefreshInfo();
        }
        public void ClearBoard(DeckRegularDict<HeapSolitaireCardInfo> thisCol)
        {
            if (thisCol.Count != 13)
                throw new BasicBlankException("There needs to be 13 cards");
            base.ClearBoard();
            int x = 0;
            PileList!.ForEach(thisPile =>
            {
                var thisCard = thisCol[x];
                thisPile.ObjectList.Add(thisCard);
                x++;
            });
            Score = 13;
            RefreshInfo();
        }

    }
}
