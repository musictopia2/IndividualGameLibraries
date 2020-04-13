using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using System;
using System.Linq;

namespace FluxxCP.Logic
{
    public static class ComputerAI
    {
        public static int FirstRandomPlayed(FluxxGameContainer gameContainer)
        {
            var tempPlayer = gameContainer.PlayerList!.GetWhoPlayer();
            return tempPlayer.MainHandList.GetRandomItem().Deck;
        }
        public static DeckRegularDict<FluxxCardInformation> CardsForTaxation(FluxxGameContainer gameContainer)
        {
            int extras = gameContainer.IncreaseAmount();
            int howMany = extras + 1;
            return gameContainer.SingleInfo!.MainHandList.GetRandomList(true, howMany).ToRegularDeckDict(); //i think you do need to remove previous.
        }
        public static FluxxCardInformation CardToPlay(FluxxGameContainer gameContainer)
        {
            return gameContainer.SingleInfo!.MainHandList.GetRandomItem();
        }
        public static DeckRegularDict<FluxxCardInformation> DiscardKeepers(FluxxGameContainer gameContainer, int howMany)
        {
            if (howMany <= 0)
                throw new BasicBlankException("Must discard at least one keeper");
            var firsts = gameContainer.SingleInfo!.KeeperList.GetRandomList(false, howMany);
            return new DeckRegularDict<FluxxCardInformation>(firsts);
        }
        public static DeckRegularDict<FluxxCardInformation> CardsToDiscardFromHand(FluxxGameContainer gameContainer, int howMany)
        {
            if (howMany <= 0)
                throw new BasicBlankException("Must discard at least one card from hand");

            return gameContainer.SingleInfo!.MainHandList.GetRandomList(false, howMany).ToRegularDeckDict();
        }
        public static KeeperPlayer KeeperToStealTrash(FluxxGameContainer gameContainer, bool isTrashed)
        {
            var tempList = gameContainer.PlayerList.Where(items => items.KeeperList.Count > 0).ToCustomBasicList();
            if (isTrashed == false)
                tempList.RemoveSpecificItem(gameContainer.SingleInfo!);
            if (tempList.Count == 0)
                throw new BasicBlankException("There are no keepers to trash or steal");
            var thisPlayer = tempList.GetRandomItem();
            return new KeeperPlayer { Card = (int)thisPlayer.KeeperList.GetRandomItem().Deck, Player = thisPlayer.Id };
        }
        public static Tuple<KeeperPlayer, KeeperPlayer> ExchangeKeepers(FluxxGameContainer gameContainer)
        {
            KeeperPlayer keeperFrom;
            keeperFrom = new KeeperPlayer() { Player = gameContainer.WhoTurn, Card = (int)gameContainer.SingleInfo!.KeeperList.GetRandomItem().Deck };
            var tempList = gameContainer.PlayerList!.AllPlayersExceptForCurrent();
            var tempPlayer = tempList.GetRandomItem();
            KeeperPlayer KeeperTo = new KeeperPlayer() { Player = tempPlayer.Id, Card = (int)tempPlayer.KeeperList.GetRandomItem().Deck };
            return new Tuple<KeeperPlayer, KeeperPlayer>(keeperFrom, KeeperTo);
        }
        public static int TempCardUse(FluxxGameContainer gameContainer)
        {
            return gameContainer.TempActionHandList.GetRandomItem();
        }
        public static int GetPlayerSelectedIndex(ActionContainer action)
        {
            var tempList = action.GetTempPlayerList();
            if (tempList.Count == 0)
                throw new BasicBlankException("Cannot get the player selected because there are no players");
            return tempList.GetRandomItem();
        }
        public static int CardToDoAgain(ActionContainer action)
        {
            var tempList = action.GetTempCardList();
            if (tempList.Count == 0)
                throw new BasicBlankException("Cannot get the cards to do again because there are no cards");
            return tempList.GetRandomItem();
        }
        public static CustomBasicList<int> SimplifyRules(FluxxGameContainer gameContainer, ActionContainer actionContainer)
        {
            var maxs = actionContainer.RulesToDiscard;
            var howMany = gameContainer.Random.GetRandomNumber(maxs, 0); // can even choose 0 to discard
            if (howMany == 0)
                return new CustomBasicList<int>();
            var tempList = actionContainer.GetTempRuleList();
            return tempList.GetRandomList(false, howMany).ToCustomBasicList();
        }
        public static int RuleToTrash(ActionContainer action)
        {
            var tempList = action.GetTempRuleList();
            if (tempList.Count == 0)
                throw new BasicBlankException("There are no rules to trash");
            return tempList.GetRandomItem();
        }
        public static int UseTake(FluxxGameContainer gameContainer, ActionContainer actionContainer, int selectedIndex)
        {
            var index = actionContainer.GetPlayerIndex(selectedIndex);
            var tempPlayer = gameContainer.PlayerList![index];
            return tempPlayer.MainHandList.GetRandomItem().Deck;
        }
        public static EnumDirection GetDirection()
        {
            CustomBasicList<EnumDirection> tempList = new CustomBasicList<EnumDirection> { EnumDirection.Left, EnumDirection.Right };
            return tempList.GetRandomItem();
        }
        public static int GoalToRemove(FluxxGameContainer gameContainer)
        {
            if (gameContainer.SaveRoot!.GoalList.Count == 1)
                throw new BasicBlankException("No need to remove a goal because always allow at least one goal");
            if (gameContainer.SaveRoot.GoalList.Count == 2)
                return (int)gameContainer.SaveRoot.GoalList.GetRandomItem().Deck;
            var tempList = (gameContainer.SaveRoot.GoalList.Take(gameContainer.SaveRoot.GoalList.Count - 1)).ToCustomBasicList();
            return (int)tempList.GetRandomItem().Deck;
        }
    }
}