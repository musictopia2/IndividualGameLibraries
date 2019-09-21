using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.NetworkingClasses.Interfaces;
using BasicGameFramework.SimpleMiscClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace BowlingDiceGameCP
{
    [SingletonGame]
    public class BowlingDiceSet : IRollMultipleDice<bool>
    {
        public IGamePackageResolver? MainContainer { get; set; }
        private IAsyncDelayer? _delay;
        public async Task<CustomBasicList<CustomBasicList<bool>>> GetDiceList(string payLoad)
        {
            return await js.DeserializeObjectAsync<CustomBasicList<CustomBasicList<bool>>>(payLoad);
        }
        public async Task LoadGameAsync(string payLoad)
        {
            if (DiceList.Count != 10)
                throw new BasicBlankException("You have to already have 10 dice");
            CustomBasicList<bool> ThisList = await js.DeserializeObjectAsync<CustomBasicList<bool>>(payLoad);
            if (ThisList.Count != 10)
                throw new BasicBlankException("You had to saved 10 items");
            int x = 0;
            ThisList.ForEach(items =>
            {
                DiceList[x].DidHit = items;
                DiceList[x].Value = items; //i think this was needed too.
                x++;
            });
        }
        public async Task<string> SaveGameAsync()
        {
            CustomBasicList<bool> thisList = DiceList.Select(Items => Items.DidHit).ToCustomBasicList();
            return await js.SerializeObjectAsync(thisList);
        }
        public CustomBasicList<CustomBasicList<bool>> RollDice(int howManySections = 7)
        {
            if (DiceList.Count == 0)
                throw new BasicBlankException("There are no dice to even roll.  Try FirstLoad");
            int counts = DiceList.Count(Items => Items.Value == false);
            CustomBasicList<CustomBasicList<bool>> output = new CustomBasicList<CustomBasicList<bool>>();
            AsyncDelayer.SetDelayer(this, ref _delay!);
            IDiceContainer<bool> thisG = MainContainer!.Resolve<IDiceContainer<bool>>();
            thisG.MainContainer = MainContainer;
            CustomBasicList<bool> possList = thisG.GetPossibleList;
            howManySections.Times(() =>
            {
                CustomBasicList<bool> firsts = new CustomBasicList<bool>();
                counts.Times(() =>
                {
                    firsts.Add(possList.GetRandomItem());
                });
                output.Add(firsts);
            });
            return output;
        }
        private readonly INetworkMessages _thisNet;
        public readonly CustomBasicList<SingleDiceInfo> DiceList = new CustomBasicList<SingleDiceInfo>();
        public BowlingDiceSet(INetworkMessages thisNet)
        {
            _thisNet = thisNet; //may use dependency injection for this (?)
        }
        public async Task SendMessageAsync(CustomBasicList<CustomBasicList<bool>> thisList)
        {
            await SendMessageAsync("rolled", thisList);
        }
        public async Task SendMessageAsync(string category, CustomBasicList<CustomBasicList<bool>> thisList)
        {
            await _thisNet.SendAllAsync(category, thisList); //i think
        }
        public async Task ShowRollingAsync(CustomBasicList<CustomBasicList<bool>> thisCol)
        {
            bool isLast = false;
            AsyncDelayer.SetDelayer(this, ref _delay!); //has to be here to learn lesson from other dice games.
            await thisCol.ForEachAsync(async firstList =>
            {
                if (thisCol.Last() == firstList)
                    isLast = true;
                int x;
                x = 0;
                firstList.ForEach(items =>
                {
                    SingleDiceInfo thisDice = FindNextPin(ref x);
                    thisDice.Populate(items);
                    thisDice.Index = DiceList.IndexOf(thisDice) + 1;
                    if (isLast == true) //animations has to show value
                        thisDice.DidHit = items;

                });
                await _delay.DelayMilli(50); //decided to have here less performance problem when rolling bowling dice.
            });
            if (isLast == false)
                throw new BasicBlankException("Was never last for showing rolling.  Rethink");
        }
        public void ClearDice()
        {
            if (DiceList.Count != 10)
                throw new BasicBlankException("You had to have 10 dice.  Otherwise, can't clear");
            DiceList.ForEach(Items =>
            {
                Items.DidHit = false;
                Items.Value = false;
            });
        }
        public void FirstLoad()
        {
            10.Times(() =>
            {
                DiceList.Add(new SingleDiceInfo());
            });
        }
        public int HowManyHit()
        {
            return DiceList.Count(items => items.DidHit == true);
        }
        private SingleDiceInfo FindNextPin(ref int previous)
        {
            if (previous > 10)
                throw new BasicBlankException($"Cannot find the next pin because its already upto 10.  The number chosen is {previous}");
            if (previous < 0)
                throw new BasicBlankException($"Must be at least 0 to find the next pin");
            int starts = previous + 1;
            for (int y = starts; y <= 10; y++)
            {
                var thisDice = DiceList[y - 1];
                if (thisDice.DidHit == false)
                {
                    previous = y;
                    return thisDice;
                }
            }
            throw new BasicBlankException("There was no other pins that has not been hit.  Therefore, there must be a problem");
        }
    }
}