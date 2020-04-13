using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using UnoCP.Cards;
using UnoCP.Data;

namespace UnoCP.Logic
{
    [SingletonGame]
    public class UnoComputerAI
    {
        private readonly UnoGameContainer _gameContainer;
        private readonly UnoVMData _model;

        public UnoComputerAI(UnoGameContainer gameContainer, UnoVMData model)
        {
            _gameContainer = gameContainer;
            _model = model;
        }

        public int ComputerMove()
        {
            //return 0; //for now
            DeckRegularDict<UnoCardInformation> moveList;
            moveList = ValidMoveList();
            if (moveList.Count == 0)
                return 0;
            if (moveList.Count == 1)
                return moveList.First().Deck;
            int maxs;
            maxs = MaxPoints(moveList);
            if (maxs > 0)
                return maxs;
            // next, consider the draw 2
            int specs;
            specs = SpecialCard(moveList, "Draw 2");
            if (specs > 0)
                return specs;
            specs = SpecialCard(moveList, "Skip");
            if (specs > 0)
                return specs;
            specs = SpecialCard(moveList, "Reverse");
            if (specs > 0)
                return specs;
            int colors;
            colors = ColorMatches(moveList);
            if (colors > 0)
                return colors;
            int nums;
            nums = NumberMatches(moveList);
            if (nums > 0)
                return nums;
            // now see if there is a wild draw 4.  if so, then play it
            specs = SpecialCard(moveList, "Wild Draw 4");
            if (specs > 0)
                return specs;
            // now see if there is a regular wild
            specs = SpecialCard(moveList, "Wild");
            if (specs > 0)
                return specs;
            throw new Exception("There was an item from the movelist.  However, was not able to play based on color, wild, number");
        }

        private DeckRegularDict<UnoCardInformation> ValidMoveList()
        {
            if (_gameContainer.CanPlay == null)
            {
                throw new BasicBlankException("Nobody is handling the canplay.  Rethink");
            }
            return (from x in _gameContainer.SingleInfo!.MainHandList!
                    where _gameContainer!.CanPlay(x.Deck) == true
                    select x).ToRegularDeckDict();
        }
        private int ColorMatches(DeckRegularDict<UnoCardInformation> moveList)
        {
            var tempList = moveList.ToRegularDeckDict(); //to make a copy.
            tempList.KeepConditionalItems(Items => Items.Color == _gameContainer.SaveRoot.CurrentColor);
            if (tempList.Count == 0)
                return 0;
            return tempList.OrderByDescending(Items => Items.Points).Take(1).Single().Deck;
        }
        private int NumberMatches(DeckRegularDict<UnoCardInformation> moveList)
        {
            UnoCardInformation currents;
            currents = _model.Pile1.CurrentCard;
            if (currents.Number == -4)
                return 0;

            var TempList = moveList.ToRegularDeckDict();
            TempList.KeepConditionalItems(Items => Items.Number == currents.Number);

            if (TempList.Count == 0)
                return 0;

            return TempList.GetRandomItem().Deck;
        }
        private int SpecialCard(DeckRegularDict<UnoCardInformation> moveList, string whatLabel)
        {
            var tempList = moveList.ToRegularDeckDict();
            if (whatLabel == "Draw 2")
                tempList.KeepConditionalItems(x => x.WhichType == EnumCardTypeList.Draw2);
            else if (whatLabel == "Skip")
                tempList.KeepConditionalItems(x => x.WhichType == EnumCardTypeList.Skip);
            else if (whatLabel == "Reverse")
                tempList.KeepConditionalItems(x => x.WhichType == EnumCardTypeList.Reverse);
            else if (whatLabel == "Wild Draw 4")
                tempList.KeepConditionalItems(x => x.Draw == 4);
            else if (whatLabel == "Wild")
                tempList.KeepConditionalItems(x => x.Draw == 0 && x.WhichType == EnumCardTypeList.Wild);
            else
                throw new BasicBlankException($"Cannot find any special card to attempt for {whatLabel}");
            if (tempList.Count == 0)
                return 0;
            return tempList.GetRandomItem().Deck;
        }
        private int MaxPoints(DeckRegularDict<UnoCardInformation> moveList)
        {
            if (SomeoneLow() == false)
                return 0;


            return moveList.OrderByDescending(Items => Items.Points).Take(1).Single().Deck;
        }

        private bool SomeoneLow()
        {

            foreach (var player in _gameContainer.PlayerList!)
            {
                if (player.MainHandList.Count > 1 && player.NickName != _gameContainer.SingleInfo!.NickName)
                    return true;
            }
            return false;
        }

        public EnumColorTypes ColorChosen()
        {
            var firstLinq = from Cards in _gameContainer.SingleInfo!.MainHandList
                            where Cards.Color != EnumColorTypes.ZOther
                            select Cards;

            var bestColor = firstLinq.GroupBy(Cards => Cards.Color);

            return bestColor.First().Key;

        }
    }
}
