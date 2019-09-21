using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.Dominos;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace DominosRegularCP
{
    public class GameBoardCP : SimpleControlViewModel
    {

        public SimpleDominoInfo CenterDomino
        {
            get
            {
                return _mainGame.SaveRoot!.CenterDomino!;
            }
            set
            {
                _mainGame.SaveRoot!.CenterDomino = value;
            }
        }
        public SimpleDominoInfo FirstDomino
        {
            get
            {
                return _mainGame.SaveRoot!.FirstDomino!;
            }
            set
            {
                _mainGame.SaveRoot!.FirstDomino = value;
            }
        }
        public SimpleDominoInfo SecondDomino
        {
            get
            {
                return _mainGame.SaveRoot!.SecondDomino!;
            }
            set
            {
                _mainGame.SaveRoot!.SecondDomino = value;
            }
        }
        public void LoadSavedGame()
        {
            DominoList.ReplaceRange(new CustomBasicList<SimpleDominoInfo> { FirstDomino, CenterDomino, SecondDomino });
        }
        public void ClearBoard()
        {
            FirstDomino = new SimpleDominoInfo();
            FirstDomino.Deck = -1;
            SecondDomino = new SimpleDominoInfo();
            SecondDomino.Deck = -2;
            CenterDomino = new SimpleDominoInfo();
            CenterDomino.Deck = -3;
            DominoList.ReplaceRange(new CustomBasicList<SimpleDominoInfo> { FirstDomino, CenterDomino, SecondDomino });
        }
        public void PopulateCenter(SimpleDominoInfo thisDomino)
        {
            if (DominoList.Count != 3)
                throw new BasicBlankException("Must have 3 dominos before populating center");
            DominoList.ReplaceItem(CenterDomino, thisDomino);
            CenterDomino = thisDomino; //just in case.
        }
        public DeckObservableDict<SimpleDominoInfo> DominoList = new DeckObservableDict<SimpleDominoInfo>();

        public ControlCommand<SimpleDominoInfo> DominoCommand { get; set; }
        private readonly DominosRegularMainGameClass _mainGame;
        public GameBoardCP(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<DominosRegularMainGameClass>();
            DominoCommand = new ControlCommand<SimpleDominoInfo>(this, async thisDomino =>
            {
                if (thisDomino.Deck == CenterDomino.Deck)
                    throw new BasicBlankException("Should never click on center domino.");
                if (thisDomino.Deck == FirstDomino.Deck)
                {
                    await _mainGame.DominoPileClicked(1);
                    return;
                }
                if (thisDomino.Deck == SecondDomino.Deck)
                {
                    await _mainGame.DominoPileClicked(2);
                    return;
                }
                throw new BasicBlankException("Problem");
            }, thisMod, thisMod.CommandContainer!);
            Visible = true;
        }
        protected override void EnableChange()
        {
            DominoCommand.ReportCanExecuteChange();
        }
        protected override void PrivateEnableAlways() { }
        protected override void VisibleChange() { }
        public bool IsValidMove(int whichOne, SimpleDominoInfo thisDomino)
        {
            if (FirstDomino.Deck <= 0 && whichOne == 1 || SecondDomino.Deck <= 0 && whichOne == 2)
            {
                if (CenterDomino.FirstNum == CenterDomino.SecondNum)
                {
                    if ((thisDomino.FirstNum == CenterDomino.FirstNum) | (thisDomino.SecondNum == CenterDomino.FirstNum))
                        return true;
                }
                if (thisDomino.FirstNum == CenterDomino.FirstNum && whichOne == 1)
                    return true;
                if (thisDomino.SecondNum == CenterDomino.FirstNum && whichOne == 1)
                    return true;
                if (thisDomino.FirstNum == CenterDomino.SecondNum && whichOne == 2)
                    return true;
                if (thisDomino.SecondNum == CenterDomino.SecondNum && whichOne == 2)
                    return true;
                return false;
            }
            if ((whichOne == 1) & (FirstDomino.CurrentFirst == thisDomino.FirstNum))
                return true;
            if ((whichOne == 1) & (FirstDomino.CurrentFirst == thisDomino.SecondNum))
                return true;
            if (whichOne == 1)
                return false;
            if (whichOne != 2)
                throw new BasicBlankException("Must be 1 or 2; not " + whichOne);
            if (SecondDomino.CurrentSecond == thisDomino.FirstNum)
                return true;
            if (SecondDomino.CurrentSecond == thisDomino.SecondNum)
                return true;
            return false;
        }
        public void MakeMove(int whichOne, SimpleDominoInfo thisDomino)
        {
            if (whichOne != 1 && whichOne != 2)
                throw new BasicBlankException("Must be 1 or 2 for makemove");
            thisDomino.IsEnabled = true;
            thisDomino.IsSelected = false;
            thisDomino.Drew = false;
            thisDomino.CurrentFirst = thisDomino.FirstNum;
            thisDomino.CurrentSecond = thisDomino.SecondNum; // make sure its not rotated
            if (SecondDomino.Deck <= 0 && whichOne == 2)
                SecondDomino.CurrentSecond = CenterDomino.FirstNum;
            if (FirstDomino.Deck <= 0 && whichOne == 1)
                FirstDomino.CurrentFirst = CenterDomino.FirstNum;
            if (whichOne == 1)
            {
                if ((FirstDomino.CurrentFirst == thisDomino.FirstNum) & (thisDomino.FirstNum != thisDomino.SecondNum))
                {
                    thisDomino.CurrentFirst = thisDomino.SecondNum;
                    thisDomino.CurrentSecond = thisDomino.FirstNum;
                }
                DominoList.ReplaceItem(FirstDomino, thisDomino);

                FirstDomino = thisDomino;
            }
            else
            {
                if ((SecondDomino.CurrentSecond == thisDomino.SecondNum) & (thisDomino.FirstNum != thisDomino.SecondNum))
                {
                    thisDomino.CurrentFirst = thisDomino.SecondNum;
                    thisDomino.CurrentSecond = thisDomino.FirstNum;
                }
                DominoList.ReplaceItem(SecondDomino, thisDomino);
                SecondDomino = thisDomino;
            }
        }
    }
}