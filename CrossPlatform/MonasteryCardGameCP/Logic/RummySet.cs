using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using MonasteryCardGameCP.Data;
using System.Linq;

namespace MonasteryCardGameCP.Logic
{
    public class RummySet : SetInfo<EnumSuitList, EnumColorList, MonasteryCardInfo, SavedSet>
    {
        private EnumWhatSets _setType;
        private readonly MonasteryCardGameGameContainer _gameContainer;

        public RummySet(MonasteryCardGameGameContainer gameContainer) : base(gameContainer.Command)
        {
            CanExpandRuns = true;
            _gameContainer = gameContainer;
        }
        public override void LoadSet(SavedSet payLoad)
        {
            _setType = payLoad.WhatType;
            HandList.ReplaceRange(payLoad.CardList);
        }
        public override SavedSet SavedSet()
        {
            SavedSet output = new SavedSet();
            output.WhatType = _setType;
            output.CardList = HandList.ToRegularDeckDict();
            return output;
        }
        protected override bool IsRun()
        {
            return _setType == EnumWhatSets.DoubleRun || _setType == EnumWhatSets.RegularRuns || _setType == EnumWhatSets.RunColors || _setType == EnumWhatSets.SuitRuns;
        }
        protected override bool CanClickMainBoard()
        {
            return true; //maybe it can now even with desktop because of timings.  learned from rummy 500.
        }
        public void CreateSet(IDeckDict<MonasteryCardInfo> thisCol, EnumWhatSets whatSet)
        {
            _setType = whatSet;
            DeckRegularDict<MonasteryCardInfo> tempList = new DeckRegularDict<MonasteryCardInfo>();
            thisCol.ForEach(thisCard =>
            {
                var newCard = new MonasteryCardInfo();
                newCard.Populate(thisCard.Temp); //hopefully this works.
                newCard.Deck = thisCard.Deck;
                tempList.Add(newCard);
            });
            HandList.ReplaceRange(tempList);
        }
        public bool IsDoubleRun => _setType == EnumWhatSets.DoubleRun;
        public DeckRegularDict<MonasteryCardInfo> NeedList(int position)
        {
            var thisCard = HandList.First();
            if (thisCard.Value == EnumCardValueList.LowAce && _setType == EnumWhatSets.DoubleRun)
                thisCard = HandList[1];
            var entireList = _gameContainer.Rummys!.EntireList;
            DeckRegularDict<MonasteryCardInfo> output = new DeckRegularDict<MonasteryCardInfo>();
            if (_setType == EnumWhatSets.RegularSuits)
            {
                output.AddRange(entireList.Where(items => items.Suit == thisCard.Suit));
                return output;
            }
            if (_setType == EnumWhatSets.EvenOdd)
            {
                var temps = (int)thisCard.Value;
                bool isOdd = temps.IsNumberOdd();
                output.AddRange(entireList.Where(items =>
                {
                    if (items.Value == EnumCardValueList.HighAce || items.Value == EnumCardValueList.LowAce)
                        return true;
                    var xx = (int)items.Value;
                    return xx.IsNumberOdd() == isOdd;
                }));
                return output; //hopefully this simple.
            }
            var numberNeeded = (int)thisCard.Value;
            var colorNeeded = thisCard.Color;
            if (_setType == EnumWhatSets.KindColor)
            {
                output.AddRange(entireList.Where(items =>
                {
                    if (items.Color != colorNeeded)
                        return false;
                    var temps = (int)items.Value;
                    if (temps == numberNeeded)
                        return true;
                    return items.Value == EnumCardValueList.HighAce || items.Value == EnumCardValueList.LowAce;
                }));
                return output;
            }
            if (_setType == EnumWhatSets.RegularKinds) //colors don't matter this time.
            {
                output.AddRange(entireList.Where(items =>
                {
                    var temps = (int)items.Value;
                    if (temps == numberNeeded)
                        return true;
                    return items.Value == EnumCardValueList.HighAce || items.Value == EnumCardValueList.LowAce;
                }));
                return output;
            }
            void SendRunOutput(int needs)
            {
                output.AddRange(entireList.Where(items =>
                {
                    if (_setType == EnumWhatSets.SuitRuns && items.Suit != thisCard.Suit)
                        return false;
                    if (_setType == EnumWhatSets.RunColors && items.Color != thisCard.Color)
                        return false;
                    var temps = (int)items.Value;
                    if (temps == needs)
                        return true;
                    return items.Value == EnumCardValueList.HighAce || items.Value == EnumCardValueList.LowAce;
                })); //don't return yet.
            }
            if (numberNeeded > 1 && numberNeeded < 14 && position == 1)
            {
                numberNeeded--;
                SendRunOutput(numberNeeded);
            }
            thisCard = HandList.Last();
            numberNeeded = (int)thisCard.Value;
            if (numberNeeded < 13 && position == 2)
            {
                numberNeeded++;
                SendRunOutput(numberNeeded);
            }
            return output;
        }
        private MonasteryCardInfo GetCard(MonasteryCardInfo thisCard, int position)
        {
            var tempCard = HandList.First();
            if (tempCard.Value == EnumCardValueList.LowAce && _setType == EnumWhatSets.DoubleRun)
                tempCard = HandList[1];
            if (_setType == EnumWhatSets.RegularSuits)
                return thisCard;
            if (thisCard.Value != EnumCardValueList.HighAce && thisCard.Value != EnumCardValueList.LowAce)
                return thisCard;
            if (position == 1)
            {
                if (thisCard.Value == EnumCardValueList.Two)
                {
                    if (_setType == EnumWhatSets.RegularRuns || _setType == EnumWhatSets.SuitRuns || _setType == EnumWhatSets.RunColors)
                        return thisCard;
                }
            }
            var entireList = _gameContainer.Rummys!.EntireList;
            if (_setType == EnumWhatSets.RegularKinds | _setType == EnumWhatSets.KindColor & tempCard.Color == EnumColorList.Red)
                return entireList.First(items => items.Value == tempCard.Value && items.Suit == EnumSuitList.Diamonds);
            if (_setType == EnumWhatSets.KindColor)
                return entireList.First(items => items.Value == tempCard.Value && items.Suit == EnumSuitList.Clubs);
            if (_setType == EnumWhatSets.EvenOdd)
            {
                int nums = (int)tempCard.Value;
                if (nums.IsNumberOdd())
                    return thisCard;
                return entireList.First(items => items.Value == tempCard.Value && items.Suit == EnumSuitList.Diamonds);
            }
            if (position == 2)
                tempCard = HandList.Last();
            if (_setType == EnumWhatSets.DoubleRun)
            {
                int nums = HandList.Count; //could be iffy.
                if (nums.IsNumberOdd())
                {
                    if (tempCard.Value == EnumCardValueList.LowAce || tempCard.Value == EnumCardValueList.HighAce)
                        return thisCard;
                    return entireList.First(items => items.Value == tempCard.Value && items.Suit == EnumSuitList.Diamonds);
                }
            }
            int numberNeeded;
            if (position == 2)
                numberNeeded = (int)tempCard.Value + 1;
            else
                numberNeeded = (int)tempCard.Value - 1;
            if (numberNeeded == 1)
                return thisCard;
            EnumSuitList suitNeeded;
            if (_setType == EnumWhatSets.RegularRuns)
                suitNeeded = EnumSuitList.Diamonds;
            else if (_setType == EnumWhatSets.SuitRuns)
                suitNeeded = tempCard.Suit;
            else if (tempCard.Color == EnumColorList.Red)
                suitNeeded = EnumSuitList.Diamonds;
            else
                suitNeeded = EnumSuitList.Clubs;
            return entireList.First(items => (int)items.Value == numberNeeded && items.Suit == suitNeeded);
        }
        public int PositionToPlay(MonasteryCardInfo thisCard, int thisPos)
        {
            var thisList = NeedList(thisPos);
            if (thisList.Count > 0)
            {
                if (thisList.ObjectExist(thisCard.Deck))
                    return thisPos;
            }
            if (_setType == EnumWhatSets.KindColor || _setType == EnumWhatSets.RegularKinds || _setType == EnumWhatSets.RegularSuits || _setType == EnumWhatSets.EvenOdd)
                return 0;
            if (thisPos == 1)
                thisPos = 2;
            else
                thisPos = 1;
            thisList = NeedList(thisPos);
            if (thisList.Count > 0)
            {
                if (thisList.ObjectExist(thisCard.Deck))
                    return thisPos;
            }
            return 0;
        }
        public void AddCard(MonasteryCardInfo thisCard, int position)
        {
            var newCard = GetCard(thisCard, position);
            var finalCard = new MonasteryCardInfo();
            finalCard.Populate(newCard.Deck);
            finalCard.Deck = thisCard.Deck; //i think
            if (position == 1)
                HandList.InsertBeginning(finalCard);
            else
                HandList.Add(finalCard);
        }
    }
}
