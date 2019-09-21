using BasicGameFramework.Attributes;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RummyDiceCP
{
    [SingletonGame]
    public class RummyBoardCP : ObservableObject
    {
        readonly RummyDiceMainGameClass _mainGame;
        readonly IAsyncDelayer _delay;
        public BasicGameCommand<RummyDiceInfo> DiceCommand { get; set; }
        readonly IGenerateDice<int> _gens;
        public RummyBoardCP(RummyDiceMainGameClass mainGame, RummyDiceViewModel thisMod, IGenerateDice<int> gens,
            IAsyncDelayer Delay)
        {
            _mainGame = mainGame;
            _gens = gens; //hopefully putting here is acceptable
            _delay = Delay;
            DiceCommand = new BasicGameCommand<RummyDiceInfo>(thisMod, async thisDice =>
            {
                int index = mainGame.SaveRoot!.DiceList.IndexOf(thisDice);
                if (index == -1)
                    throw new BasicBlankException("had problems hooking up.  Rethink");
                if (mainGame.ThisData!.MultiPlayer == true)
                    await mainGame.ThisNet!.SendAllAsync("diceclicked", index); //i think
                await mainGame.SelectOneMainAsync(index);
            }, Items => true, thisMod, thisMod.CommandContainer!);
        }
        public void EndTurn()
        {
            _doStart = true;
            _mainGame.SaveRoot!.DiceList.Clear();
        }
        private bool _doStart = true;
        public void SelectDice(int whichOne)
        {
            _mainGame.SaveRoot!.DiceList[whichOne].IsSelected = !_mainGame.SaveRoot.DiceList[whichOne].IsSelected;
        }
        private void UnselectAll()
        {
            _mainGame.SaveRoot!.DiceList.UnselectAllObjects();
        }
        public CustomBasicList<CustomBasicList<RummyDiceInfo>> RollDice()
        {
            int newNum;
            if (_mainGame.SaveRoot!.RollNumber == 1)
            {
                newNum = 10;
                _mainGame.SaveRoot.DiceList.Clear();
            }
            else
                newNum = _mainGame.SaveRoot.DiceList.Count;
            CustomBasicList<CustomBasicList<RummyDiceInfo>> output = new CustomBasicList<CustomBasicList<RummyDiceInfo>>();
            CustomBasicList<RummyDiceInfo> tempCol;
            CustomBasicList<int> possibleList = _gens.GetPossibleList;
            RummyDiceInfo thisDice;
            7.Times(x =>
            {
                tempCol = new CustomBasicList<RummyDiceInfo>();
                newNum.Times(y =>
                {
                    thisDice = new RummyDiceInfo();
                    thisDice.Populate(possibleList.GetRandomItem());
                    tempCol.Add(thisDice);
                });
                output.Add(tempCol);
            });
            return output;
        }
        //50 then 100.
        public async Task ShowRollingAsync(CustomBasicList<CustomBasicList<RummyDiceInfo>> diceCollection)
        {
            int delay;
            if (_doStart)
                delay = 100;
            else
                delay = 50;
            await diceCollection.ForEachAsync(async thisList =>
            {
                _mainGame.SaveRoot!.DiceList.ReplaceRange(thisList);
                if (_mainGame.ThisTest!.NoAnimations == false)
                    await _delay.DelayMilli(delay);
            });
            _mainGame.SaveRoot!.DiceList.Sort();
        }
        public void AddBack(CustomBasicList<RummyDiceInfo> thisList)
        {
            _mainGame.SaveRoot!.DiceList.AddRange(thisList);
            if (thisList.Count == 0)
                return;
            UnselectAll();
            _mainGame.SaveRoot.DiceList.Sort();
        }
        public ICustomBasicList<RummyDiceInfo> GetSelectedList()
        {
            ICustomBasicList<RummyDiceInfo> output = _mainGame.SaveRoot!.DiceList.RemoveAllAndObtain(Items => Items.IsSelected == true);
            _mainGame.SaveRoot.DiceList.Sort();
            return output;
        }
        public bool HasSelectedDice()
        {
            return _mainGame.SaveRoot!.DiceList.Any(Items => Items.IsSelected == true);
        }
    }
}