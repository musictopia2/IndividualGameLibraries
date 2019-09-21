using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace PaydayCP
{
    public static class PaydayComputerAI
    {
        private static CustomBasicList<PaydayPlayerItem> GetPossiblePlayerList(PaydayMainGameClass mainGame)
        {
            CustomBasicList<PaydayPlayerItem> output = new CustomBasicList<PaydayPlayerItem>();
            CustomBasicList<string> tempList = mainGame.ThisMod!.PopUpList!.TextList.Select(items => items.DisplayText).ToCustomBasicList();
            tempList.ForEach(Name =>
            {
                output.Add(mainGame.PlayerList.Single(Items => Items.NickName == Name));
            });
            return output;
        }
        public static string PlayerChosen(PaydayMainGameClass mainGame)
        {
            if (mainGame.ThisMod!.PopUpList!.Count() == 1)
                return mainGame.ThisMod.PopUpList.GetText(1); //i think.
            CustomBasicList<PaydayPlayerItem> tempList = GetPossiblePlayerList(mainGame);
            MailCard thisMail = mainGame.ThisMod.MailPile!.GetCardInfo();
            return thisMail.MailType switch
            {
                EnumMailType.MadMoney => tempList.OrderByDescending(Items => Items.NetIncome()).Take(1).Single().NickName,
                EnumMailType.PayNeighbor => tempList.OrderBy(Items => Items.NetIncome()).Take(1).Single().NickName,
                _ => throw new BasicBlankException($"Must be madmoney or payneighbor, not {thisMail.CardCategory.ToString()}"),
            };
        }
        public static bool LandDeal(PaydayMainGameClass mainGame)
        {
            return !mainGame.SingleInfo!.Hand.Any(items => items.CardCategory == EnumCardCategory.Deal); //if you have a deal, need to get rid of it.
        }
        public static bool PurchaseDeal(PaydayMainGameClass mainGame)
        {
            int dealCount = mainGame.SingleInfo!.Hand.Count(items => items.CardCategory == EnumCardCategory.Deal);
            if (mainGame.SingleInfo.CurrentMonth == mainGame.SaveRoot!.MaxMonths)
            {
                if (mainGame.SingleInfo.DayNumber > 13)
                    return false;
                if (dealCount > 1)
                    return false;
                return true;
            }
            if (mainGame.SingleInfo.CurrentMonth + 1 == mainGame.SaveRoot.MaxMonths)
            {
                if (dealCount > 3)
                    return false;
                return true;
            }
            if (dealCount > 4)
                return false;
            return true;
        }
        public static int NumberChosen(PaydayMainGameClass mainGame)
        {
            int index = mainGame.ThisMod!.PopUpList!.ItemToChoose(true);
            return int.Parse(mainGame.ThisMod.PopUpList.GetText(index));
        }
        public static int BuyerSelected(PaydayMainGameClass mainGame)
        {
            var thisList = mainGame.SingleInfo!.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            return thisList.OrderByDescending(Items => Items.Value).ThenByDescending(Items => Items.Profit()).Take(1).Single().Deck;
        }
    }
}