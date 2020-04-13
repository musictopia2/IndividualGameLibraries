using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using RollEmCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RollEmCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardProcesses
    {
        private readonly RollEmGameContainer _gameContainer;
        private readonly RollEmVMData _model;

        public GameBoardProcesses(RollEmGameContainer gameContainer, RollEmVMData model)
        {
            _gameContainer = gameContainer;
            _model = model;
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
            _gameContainer.SaveRoot!.SpaceList = _gameContainer.NumberList!.Values.Select(items => GetSavedItem(items)).ToCustomBasicList();
        }
        public void ClearBoard()
        {
            foreach (var item in _gameContainer.NumberList!.Values)
            {
                item.IsCrossed = false;
                item.Recently = false;
            }
            _model.Cup!.HowManyDice = 2;
            _model.Cup.HideDice();
            _model.Cup.CanShowDice = false;
            _gameContainer.Aggregator.RepaintBoard(); //try this too.
        }
        public void LoadSavedGame()
        {
            //ClearBoard();
            int x = 0;
            _gameContainer.SaveRoot!.SpaceList.ForEach(items =>
            {
                x++;
                if (items == "crossed")
                    CrossOffSaved(x);
                else if (items == "recent")
                    CrossOffTemp(x);
                else if (items != "open")
                    throw new BasicBlankException("Wrong Text.  Rethink");
            });
            _gameContainer.Aggregator.RepaintBoard();
        }
        private void CrossOffSaved(int x)
        {
            _gameContainer.NumberList![x].IsCrossed = true;
        }
        private void CrossOffTemp(int x)
        {
            _gameContainer.NumberList![x].Recently = true;
        }
        private void CrossOffPreviousNumbers()
        {
            var temps = _gameContainer.NumberList!.Values.Where(items => items.Recently == true).ToCustomBasicList();
            temps.ForEach(items =>
            {
                items.Recently = false;
                items.IsCrossed = true;
            });
        }
        private void ClearPreviousNumbers()
        {
            var temps = _gameContainer.NumberList!.Values.Where(items => items.Recently == true).ToCustomBasicList();
            temps.ForEach(items =>
            {
                items.Recently = false;
            });
        }
        private int HowManyRecently => _gameContainer.NumberList!.Values.Count(items => items.Recently == true);
        private int RecentTotal => _gameContainer.NumberList!.Values.Where(items => items.Recently == true).Sum(Items => Items.Number);
        public int CalculateScore => _gameContainer.NumberList!.Values.Where(items => items.IsCrossed == false && items.Number <= 9).Sum(Items => Items.Number);
        private bool NeedOneDice => !_gameContainer.NumberList!.Values.Any(items => items.Number >= 7 && items.IsCrossed == false && items.Number <= 9);
        public async Task MakeMoveAsync(int Number)
        {
            CrossOffTemp(Number);
            _gameContainer.Aggregator.RepaintBoard();
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self && _gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.5);
        }
        public void FinishMove()
        {
            CrossOffPreviousNumbers();
            if (NeedOneDice == true)
                _model.Cup!.HowManyDice = 1;
            _gameContainer.Aggregator.RepaintBoard();
        }
        public bool IsMoveFinished()
        {
            int Nums = RecentTotal;
            if (Nums == GetDiceTotal)
                return true;
            return false;
        }
        public int GetDiceTotal => _model.Cup!.TotalDiceValue;
        public bool CanMakeMove(int number)
        {
            if (_gameContainer.Test!.AllowAnyMove == true)
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
                _gameContainer.Aggregator.RepaintBoard();
        }
        public CustomBasicList<int> GetNumberList()
        {
            if (_gameContainer.NumberList!.Values.Any(items => items.Recently))
                throw new BasicBlankException("Cannot be recently checked when the computer is checking for numbers left");
            CustomBasicList<NumberInfo> tempList = _gameContainer.NumberList.Values.Where(items => items.IsCrossed == false).ToCustomBasicList();
            return tempList.Select(items => items.Number).ToCustomBasicList();
        }
    }
}
