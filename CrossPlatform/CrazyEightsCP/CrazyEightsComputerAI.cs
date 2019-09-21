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
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
namespace CrazyEightsCP
{
    public class CrazyEightsComputerAI
    {
        public int CardToPlay(CrazyEightsSaveInfo SaveRoot)
        {
            CrazyEightsPlayerItem SingleInfo = SaveRoot.PlayerList.GetWhoPlayer();
            DeckRegularDict<RegularSimpleCard> ThisList = SingleInfo.MainHandList.Where(Items => Items.Value != EnumCardValueList.Eight && Items.Suit == SaveRoot.CurrentSuit).ToRegularDeckDict();
            if (ThisList.Count != 0)
                return ThisList.GetRandomItem().Deck;
            ThisList = SingleInfo.MainHandList.Where(Items => Items.Value != EnumCardValueList.Eight && Items.Value == SaveRoot.CurrentNumber).ToRegularDeckDict();
            if (ThisList.Count == 1)
                return ThisList.Single().Deck;
            if (ThisList.Count != 0)
                return FindBestCard(ThisList);
            ThisList = SingleInfo.MainHandList.Where(Items => Items.Value == EnumCardValueList.Eight).ToRegularDeckDict();
            if (ThisList.Count != 0)
                return ThisList.GetRandomItem().Deck;
            return 0;//0 means needs to draw.
        }
        private int FindBestCard(DeckRegularDict<RegularSimpleCard> ThisList)
        {
            var BestSuit = ThisList.GroupBy(Items => Items.Suit).OrderByDescending(Temps => Temps.Count());
            var SuitToUse = BestSuit.First().Key;
            ThisList.KeepConditionalItems(Items => Items.Suit == SuitToUse);
            return ThisList.GetRandomItem().Deck;
        }
        public EnumSuitList SuitToChoose(CrazyEightsPlayerItem SingleInfo)
        {
            var BestSuit = SingleInfo.MainHandList.GroupBy(Items => Items.Suit).OrderByDescending(Temps => Temps.Count());
            return BestSuit.First().Key;

            //return (from Cards in SingleInfo.MainHandList
            //        group Cards by Cards.Suit into Count
            //        let Results = Count.Key
            //        select Cards).First().Suit;
        }
    }
}