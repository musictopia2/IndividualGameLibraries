using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace FourSuitRummyCP
{
    public class MainSets : MainSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard, SetInfo, SavedSet>
    {
        readonly FourSuitRummyPlayerItem _thisPlayer;
        readonly IBasicGameVM _thisMod;
        public MainSets(IBasicGameVM thisMod, FourSuitRummyPlayerItem thisPlayer) : base(thisMod)
        {
            _thisPlayer = thisPlayer;
            if (_thisPlayer.PlayerCategory == BasicGameFramework.MultiplayerClasses.BasicPlayerClasses.EnumPlayerCategory.Self)
                Text = "Your Sets";
            else
                Text = "Opponent Sets";
            HasFrame = true;
            _thisMod = thisMod;
        }
        public void AddNewSet(IDeckDict<RegularRummyCard> thisCol)
        {
            if (thisCol.Count != 3)
                throw new BasicBlankException("You must have 3 cards to add to a set");
            thisCol.UnhighlightObjects();
            SetInfo thisSet = new SetInfo(_thisMod);
            thisSet.HandList.AddRange(thisCol);
            SetList.Add(thisSet);
        }
        public bool CanAddSet(IDeckDict<RegularRummyCard> thisCol)
        {
            if (SetList.Count == 0)
                return true; //you can always do a first one.
            RegularRummyCard thisCard = thisCol.First();
            foreach (var thisSet in SetList)
            {
                var oldCard = thisSet.HandList.First();
                if (thisCard.Suit == oldCard.Suit)
                    return false;
            }
            return true;
        }
        private int HowManyMatches
        {
            get
            {
                if (SetList.Count == 0)
                    throw new BasicBlankException("There was no sets");
                if (SetList.Count == 1)
                    return 0;
                CustomBasicList<int> numList = new CustomBasicList<int>();
                SetList.ForEach(thisSet =>
                {
                    numList.Add((int)thisSet.HandList.First().Value);
                });
                int uniques = numList.GroupBy(items => items).Count(); //hopefully this works.
                int counts = numList.Count;
                if (uniques == counts)
                    return 0;
                if (uniques == 1 && counts == 2)
                    return 1;
                if (uniques == 1 && counts == 3)
                    return 3;
                if (uniques == 1 && counts == 4)
                    return 6;
                if (uniques == 2 && counts == 3)
                    return 1;
                if (uniques == 3 && counts == 4)
                    return 1;
                if (uniques == 2 && counts == 4)
                {
                    var newList = numList.GroupOrderAscending(items => items);
                    if (newList.First().Count() == 2)
                        return 2;
                    return 3;
                }
                throw new BasicBlankException($"Need to figure out how many matches for unique list of {uniques}  for numbers {counts}");
            }
        }
        public int CalculateScore
        {
            get
            {
                if (SetList.Count == 0)
                    return 0;
                int matches = HowManyMatches;
                int firstNum = SetList.Count * 20;
                int matchAmount = matches * 50;
                return firstNum + matchAmount;
            }
        }
    }
}