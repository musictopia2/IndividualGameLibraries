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
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.ColorCards;

namespace UnoCP
{
    public class UnoComputerAI
    {

        internal UnoMainGameClass ThisGame;

        public int ComputerMove()
        {
            //return 0; //for now
            DeckRegularDict<UnoCardInformation> MoveList;
            MoveList = ValidMoveList();
            if (MoveList.Count == 0)
                return 0;
            if (MoveList.Count == 1)
                return MoveList.First().Deck;
            int maxs;
            maxs = MaxPoints(MoveList);
            if (maxs > 0)
                return maxs;
            // next, consider the draw 2
            int specs;
            specs = SpecialCard(MoveList, "Draw 2");
            if (specs > 0)
                return specs;
            specs = SpecialCard(MoveList, "Skip");
            if (specs > 0)
                return specs;
            specs = SpecialCard(MoveList, "Reverse");
            if (specs > 0)
                return specs;
            int colors;
            colors = ColorMatches(MoveList);
            if (colors > 0)
                return colors;
            int nums;
            nums = NumberMatches(MoveList);
            if (nums > 0)
                return nums;
            // now see if there is a wild draw 4.  if so, then play it
            specs = SpecialCard(MoveList, "Wild Draw 4");
            if (specs > 0)
                return specs;
            // now see if there is a regular wild
            specs = SpecialCard(MoveList, "Wild");
            if (specs > 0)
                return specs;
            throw new Exception("There was an item from the movelist.  However, was not able to play based on color, wild, number");
        }

        private DeckRegularDict<UnoCardInformation> ValidMoveList()
        {
            return (from Items in ThisGame.SingleInfo.MainHandList
                    where ThisGame.CanPlay(Items.Deck) == true
                    select Items).ToRegularDeckDict();
        }
        private int ColorMatches(DeckRegularDict<UnoCardInformation> MoveList)
        {
            var TempList = MoveList.ToRegularDeckDict(); //to make a copy.
            TempList.KeepConditionalItems(Items => Items.Color == ThisGame.SaveRoot.CurrentColor);
            if (TempList.Count == 0)
                return 0;
            return TempList.OrderByDescending(Items => Items.Points).Take(1).Single().Deck;
        }
        private int NumberMatches(DeckRegularDict<UnoCardInformation> MoveList)
        {
            UnoCardInformation Currents;
            Currents = ThisGame.CurrentObject;
            if (Currents.Number == -4)
                return 0;

            var TempList = MoveList.ToRegularDeckDict();
            TempList.KeepConditionalItems(Items => Items.Number == Currents.Number);

            if (TempList.Count == 0)
                return 0;

            return TempList.GetRandomItem().Deck;
        }
        private int SpecialCard(DeckRegularDict<UnoCardInformation> MoveList, string WhatLabel)
        {
            var TempList = MoveList.ToRegularDeckDict();
            if (WhatLabel == "Draw 2")
                TempList.KeepConditionalItems(Items => Items.WhichType == EnumCardTypeList.Draw2);
            else if (WhatLabel == "Skip")
                TempList.KeepConditionalItems(Items => Items.WhichType == EnumCardTypeList.Skip);
            else if (WhatLabel == "Reverse")
                TempList.KeepConditionalItems(Items => Items.WhichType == EnumCardTypeList.Reverse);
            else if (WhatLabel == "Wild Draw 4")
                TempList.KeepConditionalItems(Items => Items.Draw == 4);
            else if (WhatLabel == "Wild")
                TempList.KeepConditionalItems(Items => Items.Draw == 0 && Items.WhichType == EnumCardTypeList.Wild);
            else
                throw new BasicBlankException($"Cannot find any special card to attempt for {WhatLabel}");
            if (TempList.Count == 0)
                return 0;
            return TempList.GetRandomItem().Deck;
        }
        private int MaxPoints(DeckRegularDict<UnoCardInformation> MoveList)
        {
            if (SomeoneLow() == false)
                return 0;


            return MoveList.OrderByDescending(Items => Items.Points).Take(1).Single().Deck;
        }

        private bool SomeoneLow()
        {
            
            foreach (var ThisPlayer in ThisGame.PlayerList)
            {
                if (ThisPlayer.MainHandList.Count > 1 && ThisPlayer.NickName != ThisGame.SingleInfo.NickName)
                    return true;
            }
            return false;
        }

        public EnumColorTypes ColorChosen()
        {
            var FirstLinq = from Cards in ThisGame.SingleInfo.MainHandList
                            where Cards.Color != EnumColorTypes.ZOther
                            select Cards;

            var BestColor = FirstLinq.GroupBy(Cards => Cards.Color);

            return BestColor.First().Key;

        }
    }
}
