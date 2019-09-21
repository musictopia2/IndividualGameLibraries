using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
namespace FluxxCP
{
    public static class ComputerAI
    {
        public static int FirstRandomPlayed(FluxxMainGameClass mainGame)
        {
            var tempPlayer = mainGame.PlayerList!.GetWhoPlayer();
            return tempPlayer.MainHandList.GetRandomItem().Deck;
        }
        public static DeckRegularDict<FluxxCardInformation> CardsForTaxation(FluxxMainGameClass mainGame, GlobalClass thisGlobal)
        {
            int extras = thisGlobal.IncreaseAmount();
            int howMany = extras + 1;
            return mainGame.SingleInfo!.MainHandList.GetRandomList(true, howMany).ToRegularDeckDict(); //i think you do need to remove previous.
        }
        public static FluxxCardInformation CardToPlay(FluxxMainGameClass mainGame)
        {
            return mainGame.SingleInfo!.MainHandList.GetRandomItem();
        }
        public static DeckRegularDict<FluxxCardInformation> DiscardKeepers(FluxxMainGameClass mainGame, int howMany)
        {
            if (howMany <= 0)
                throw new BasicBlankException("Must discard at least one keeper");
            var firsts = mainGame.SingleInfo!.KeeperList.GetRandomList(false, howMany);
            return new DeckRegularDict<FluxxCardInformation>(firsts);
        }
        public static DeckRegularDict<FluxxCardInformation> CardsToDiscardFromHand(FluxxMainGameClass mainGame, int howMany)
        {
            if (howMany <= 0)
                throw new BasicBlankException("Must discard at least one card from hand");
            return mainGame.SingleInfo!.MainHandList.GetRandomList(false, howMany).ToRegularDeckDict();
        }
        public static KeeperPlayer KeeperToStealTrash(FluxxMainGameClass mainGame, bool isTrashed)
        {
            var tempList = mainGame.PlayerList.Where(items => items.KeeperList.Count > 0).ToCustomBasicList();
            if (isTrashed == false)
                tempList.RemoveSpecificItem(mainGame.SingleInfo!);
            if (tempList.Count == 0)
                throw new BasicBlankException("There are no keepers to trash or steal");
            var thisPlayer = tempList.GetRandomItem();
            return new KeeperPlayer { Card = (int)thisPlayer.KeeperList.GetRandomItem().Deck, Player = thisPlayer.Id };
        }
        public static Tuple<KeeperPlayer, KeeperPlayer> ExchangeKeepers(FluxxMainGameClass mainGame)
        {
            KeeperPlayer keeperFrom;
            keeperFrom = new KeeperPlayer() { Player = mainGame.WhoTurn, Card = (int)mainGame.SingleInfo!.KeeperList.GetRandomItem().Deck };
            var tempList = mainGame.PlayerList!.AllPlayersExceptForCurrent();
            var tempPlayer = tempList.GetRandomItem();
            KeeperPlayer KeeperTo = new KeeperPlayer() { Player = tempPlayer.Id, Card = (int)tempPlayer.KeeperList.GetRandomItem().Deck };
            return new Tuple<KeeperPlayer, KeeperPlayer>(keeperFrom, KeeperTo);
        }
        public static int TempCardUse(GlobalClass thisGlobal)
        {
            return thisGlobal.TempActionHandList.GetRandomItem();
        }
        public static int GetPlayerSelectedIndex(FluxxMainGameClass mainGame)
        {
            var tempList = mainGame.ThisMod!.Action1!.GetTempPlayerList();
            if (tempList.Count == 0)
                throw new BasicBlankException("Cannot get the player selected because there are no players");
            return tempList.GetRandomItem();
        }
        public static int CardToDoAgain(FluxxMainGameClass mainGame)
        {
            var tempList = mainGame.ThisMod!.Action1!.GetTempCardList();
            if (tempList.Count == 0)
                throw new BasicBlankException("Cannot get the cards to do again because there are no cards");
            return tempList.GetRandomItem();
        }
        public static CustomBasicList<int> SimplifyRules(FluxxMainGameClass mainGame)
        {
            RandomGenerator rs = mainGame.MainContainer.Resolve<RandomGenerator>();
            var maxs = mainGame.ThisMod!.Action1!.MaxRulesToDiscard();
            var howMany = rs.GetRandomNumber(maxs, 0); // can even choose 0 to discard
            if (howMany == 0)
                return new CustomBasicList<int>();
            var tempList = mainGame.ThisMod.Action1.GetTempRuleList();
            return tempList.GetRandomList(false, howMany).ToCustomBasicList();
        }
        public static int RuleToTrash(FluxxMainGameClass mainGame)
        {
            var tempList = mainGame.ThisMod!.Action1!.GetTempRuleList();
            if (tempList.Count == 0)
                throw new BasicBlankException("There are no rules to trash");
            return tempList.GetRandomItem();
        }
        public static int UseTake(FluxxMainGameClass mainGame, int selectedIndex)
        {
            var index = mainGame.ThisMod!.Action1!.GetPlayerIndex(selectedIndex);
            var tempPlayer = mainGame.PlayerList![index];
            return tempPlayer.MainHandList.GetRandomItem().Deck;
        }
        public static EnumDirection GetDirection()
        {
            CustomBasicList<EnumDirection> tempList = new CustomBasicList<EnumDirection> { EnumDirection.Left, EnumDirection.Right };
            return tempList.GetRandomItem();
        }
        public static int GoalToRemove(FluxxMainGameClass mainGame)
        {
            if (mainGame.SaveRoot!.GoalList.Count == 1)
                throw new BasicBlankException("No need to remove a goal because always allow at least one goal");
            if (mainGame.SaveRoot.GoalList.Count == 2)
                return (int)mainGame.SaveRoot.GoalList.GetRandomItem().Deck;
            var tempList = (mainGame.SaveRoot.GoalList.Take(mainGame.SaveRoot.GoalList.Count - 1)).ToCustomBasicList();
            return (int)tempList.GetRandomItem().Deck;
        }
    }
}