using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dominos;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace DiceDominosCP
{
    [SingletonGame]
    public class GameBoardCP : GameBoardViewModel<SimpleDominoInfo>
    {
        private readonly DiceDominosViewModel _thisMod;
        private readonly IAsyncDelayer _thisDelay;
        private readonly DiceDominosMainGameClass _mainGame;
        private readonly TestOptions _thisTest;
        public GameBoardCP(DiceDominosViewModel thisMod) : base(thisMod)
        {
            _thisMod = thisMod;
            _thisDelay = thisMod.MainContainer!.Resolve<IAsyncDelayer>();
            Columns = 7;
            Rows = 4;
            _mainGame = thisMod.MainContainer.Resolve<DiceDominosMainGameClass>();
            _thisTest = thisMod.MainContainer.Resolve<TestOptions>();
            Text = "Dominos";
            HasFrame = true;
        }
        private bool HasSix()
        {
            return _thisMod.ThisCup!.DiceList.Any(items => items.Value == 6);
        }
        public int DiceValue(int index)
        {
            if (index == 1)
                return _thisMod.ThisCup!.DiceList.First().Value;
            else if (index == 2)
                return _thisMod.ThisCup!.DiceList.Last().Value;
            else
                throw new BasicBlankException($"Must be 1 or 2, not {index} to find the dice value");
        }
        public void MakeMove(int deck)
        {
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            thisDomino.Visible = false;
        }
        public bool IsValidMove(int deck)
        {
            if (_mainGame.SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer && _thisTest.AllowAnyMove == true)
                return true; //because we are allowing any move for testing.
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            if (HasSix() == false)
            {
                // this means no 6
                if (thisDomino.FirstNum == DiceValue(1) && thisDomino.SecondNum == DiceValue(2))
                    return true;
                if (thisDomino.FirstNum == DiceValue(2) && thisDomino.SecondNum == DiceValue(1))
                    return true;
                return false;
            }
            if (thisDomino.FirstNum == DiceValue(1) && thisDomino.SecondNum == DiceValue(2))
                return true;
            if (thisDomino.FirstNum == DiceValue(2) && thisDomino.SecondNum == DiceValue(1))
                return true;
            if (DiceValue(1) == 6 && DiceValue(2) == 6 && (thisDomino.FirstNum == 0 || thisDomino.FirstNum == 6) && (thisDomino.SecondNum == 0 || thisDomino.SecondNum == 6))
                return true;
            if (DiceValue(1) == 6 && thisDomino.FirstNum == 0 && DiceValue(2) == thisDomino.SecondNum)
                return true;
            if (DiceValue(1) == 6 && thisDomino.SecondNum == 0 && DiceValue(2) == thisDomino.FirstNum)
                return true;
            if (DiceValue(2) == 6 && thisDomino.SecondNum == 0 && DiceValue(1) == thisDomino.FirstNum)
                return true;
            if (DiceValue(2) == 6 && thisDomino.FirstNum == 0 && DiceValue(1) == thisDomino.SecondNum)
                return true;
            return false;
        }
        protected override async Task ClickProcessAsync(SimpleDominoInfo payLoad)
        {
            await _thisMod.DominoClickedAsync(payLoad);
        }
        public void ClearPieces()
        {
            ObjectList.ReplaceRange(_thisMod.DominosList);
        }
        public DeckRegularDict<SimpleDominoInfo> GetVisibleList()
        {
            return ObjectList.Where(Items => Items.Visible).ToRegularDeckDict();
        }
        public async Task ShowDominoAsync(int deck)
        {
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            thisDomino.IsUnknown = false;
            await _thisDelay.DelaySeconds(2);
            thisDomino.IsUnknown = true;
        }
    }
}