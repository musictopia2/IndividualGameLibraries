using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using RummyDiceCP.Data;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Interfaces;
using BasicGameFrameworkLibrary.CommandClasses;
//i think this is the most common things i like to do
namespace RummyDiceCP.Logic
{
    [SingletonGame]
    [AutoReset] //tried taking out autoreset but not making any difference.
    public class RummyBoardCP : ObservableObject
    {
        //readonly RummyDiceMainGameClass _mainGame;
        readonly IAsyncDelayer _delay;
        private readonly BasicData _basicData;
        private readonly CommandContainer _command;
        private readonly TestOptions _test;

        //public BasicGameCommand<RummyDiceInfo> DiceCommand { get; set; }
        readonly IGenerateDice<int> _gens;

        public async Task SelectDiceAsync(RummyDiceInfo dice)
        {
            if (_command.IsExecuting)
            {
                return;
            }
            _command.IsExecuting = true;
            var list = SaveRoot!.Invoke().DiceList;
            int index = list.IndexOf(dice);
            if (index == -1)
            {
                throw new BasicBlankException("had problems hooking up.  Rethink");
            }
            if (_basicData.MultiPlayer == true)
                await _network!.SendAllAsync("diceclicked", index); //i think
            await SelectOneMainAsync!.Invoke(index);
        }
        private readonly INetworkMessages? _network;
        public RummyBoardCP(TestOptions test, 
            IGenerateDice<int> gens,
            IAsyncDelayer delay,
            BasicData basicData,
            CommandContainer command
            )
        {
            _test = test;
            _gens = gens; //hopefully putting here is acceptable
            _delay = delay;
            _basicData = basicData;
            _command = command;
            _network = _basicData.GetNetwork();
        }
        #region "Delegates"
        public Func<RummyDiceSaveInfo>? SaveRoot { get; set; }
        public Func<int, Task>? SelectOneMainAsync { get; set; }
        #endregion
        public void EndTurn()
        {
            _doStart = true;
            SaveRoot!.Invoke().DiceList.Clear();
        }
        private bool _doStart = true;
        public void SelectDice(int whichOne)
        {
            SaveRoot!.Invoke().DiceList[whichOne].IsSelected = !SaveRoot!.Invoke().DiceList[whichOne].IsSelected;
        }
        private void UnselectAll()
        {
            SaveRoot!.Invoke().DiceList.UnselectAllObjects();
        }
        public CustomBasicList<CustomBasicList<RummyDiceInfo>> RollDice()
        {
            int newNum;
            if (SaveRoot!.Invoke().RollNumber == 1)
            {
                newNum = 10;
                SaveRoot!.Invoke().DiceList.Clear();
            }
            else
                newNum = SaveRoot!.Invoke().DiceList.Count;
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
                SaveRoot!.Invoke().DiceList.ReplaceRange(thisList);
                if (_test.NoAnimations == false)
                    await _delay.DelayMilli(delay);
            });
            SaveRoot!.Invoke().DiceList.Sort();
        }
        public void AddBack(CustomBasicList<RummyDiceInfo> thisList)
        {
            SaveRoot!.Invoke().DiceList.AddRange(thisList);
            if (thisList.Count == 0)
                return;
            UnselectAll();
            SaveRoot!.Invoke().DiceList.Sort();
        }
        public ICustomBasicList<RummyDiceInfo> GetSelectedList()
        {
            ICustomBasicList<RummyDiceInfo> output = SaveRoot!.Invoke().DiceList.RemoveAllAndObtain(Items => Items.IsSelected == true);
            SaveRoot!.Invoke().DiceList.Sort();
            return output;
        }
        public bool HasSelectedDice()
        {
            return SaveRoot!.Invoke().DiceList.Any(Items => Items.IsSelected == true);
        }
    }
}
