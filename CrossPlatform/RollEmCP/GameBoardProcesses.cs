using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RollEmCP
{
    [SingletonGame]
    public class GameBoardProcesses
    {
        private readonly RollEmMainGameClass _mainGame;
        public GameBoardProcesses(RollEmMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }

        private string GetSavedItem(NumberInfo thisNumber)
        {
            if (thisNumber.IsCrossed == true)
                return "crossed";
            if (thisNumber.Recently == true)
                return "recent";
            return "open";
        }
        public void SaveGame()
        {
            _mainGame.SaveRoot!.SpaceList = _mainGame.NumberList!.Values.Select(items => GetSavedItem(items)).ToCustomBasicList();
        }
        public void ClearBoard()
        {
            foreach (var item in _mainGame.NumberList!.Values)
            {
                item.IsCrossed = false;
                item.Recently = false;
            }
            _mainGame.ThisCup!.HowManyDice = 2;
            _mainGame.ThisCup.HideDice();
            _mainGame.ThisCup.CanShowDice = false;
            _mainGame.RepaintBoard(); //try this too.
        }
        public void LoadSavedGame()
        {
            ClearBoard();
            int x = 0;
            _mainGame.SaveRoot!.SpaceList.ForEach(items =>
            {
                x++;
                if (items == "crossed")
                    CrossOffSaved(x);
                else if (items == "recent")
                    CrossOffTemp(x);
                else if (items != "open")
                    throw new BasicBlankException("Wrong Text.  Rethink");
            });
            _mainGame.RepaintBoard();
        }
        private void CrossOffSaved(int x)
        {
            _mainGame.NumberList![x].IsCrossed = true;
        }
        private void CrossOffTemp(int x)
        {
            _mainGame.NumberList![x].Recently = true;
        }
        private void CrossOffPreviousNumbers()
        {
            var temps = _mainGame.NumberList!.Values.Where(items => items.Recently == true).ToCustomBasicList();
            temps.ForEach(items =>
            {
                items.Recently = false;
                items.IsCrossed = true;
            });
        }
        private void ClearPreviousNumbers()
        {
            var temps = _mainGame.NumberList!.Values.Where(items => items.Recently == true).ToCustomBasicList();
            temps.ForEach(items =>
            {
                items.Recently = false;
            });
        }
        private int HowManyRecently => _mainGame.NumberList!.Values.Count(items => items.Recently == true);
        private int RecentTotal => _mainGame.NumberList!.Values.Where(items => items.Recently == true).Sum(Items => Items.Number);
        public int CalculateScore => _mainGame.NumberList!.Values.Where(items => items.IsCrossed == false && items.Number <= 9).Sum(Items => Items.Number);
        private bool NeedOneDice => !_mainGame.NumberList!.Values.Any(items => items.Number >= 7 && items.IsCrossed == false && items.Number <= 9);
        public async Task MakeMoveAsync(int Number)
        {
            CrossOffTemp(Number);
            _mainGame.RepaintBoard();
            if (_mainGame.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self && _mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.5);
        }
        public void FinishMove()
        {
            CrossOffPreviousNumbers();
            if (NeedOneDice == true)
                _mainGame.ThisMod!.ThisCup!.HowManyDice = 1;
            _mainGame.RepaintBoard();
        }
        public bool IsMoveFinished()
        {
            int Nums = RecentTotal;
            if (Nums == GetDiceTotal)
                return true;
            return false;
        }
        public int GetDiceTotal => _mainGame.ThisCup!.TotalDiceValue;
        public bool CanMakeMove(int number)
        {
            if (_mainGame.ThisTest!.AllowAnyMove == true)
                return true; //for testing.
            int diceTotal = GetDiceTotal;
            if (HowManyRecently == 1)
            {
                return RecentTotal + number == diceTotal;
            }
            return number <= diceTotal;
        }
        public bool HadRecent
        {
            get
            {
                int Manys = HowManyRecently;
                if (Manys > 1)
                    throw new BasicBlankException("Can only have one recent one");
                return Manys == 1;
            }
        }
        public void ClearRecent(bool doRefresh)
        {
            ClearPreviousNumbers();
            if (doRefresh == true)
                _mainGame.RepaintBoard();
        }
        public CustomBasicList<int> GetNumberList()
        {
            if (_mainGame.NumberList!.Values.Any(items => items.Recently))
                throw new BasicBlankException("Cannot be recently checked when the computer is checking for numbers left");
            CustomBasicList<NumberInfo> tempList = _mainGame.NumberList.Values.Where(items => items.IsCrossed == false).ToCustomBasicList();
            return tempList.Select(items => items.Number).ToCustomBasicList();
        }
    }
}