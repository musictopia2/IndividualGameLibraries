using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using PaydayCP.Cards;
using PaydayCP.Data;
using System.Linq;

namespace PaydayCP.Logic
{
    public static class PaydayComputerAI
    {
        private static CustomBasicList<PaydayPlayerItem> GetPossiblePlayerList(PaydayGameContainer mainGame, PaydayVMData model)
        {
            CustomBasicList<PaydayPlayerItem> output = new CustomBasicList<PaydayPlayerItem>();
            CustomBasicList<string> tempList = model.PopUpList!.TextList.Select(items => items.DisplayText).ToCustomBasicList();
            tempList.ForEach(Name =>
            {
                output.Add(mainGame.PlayerList.Single(Items => Items.NickName == Name));
            });
            return output;
        }
        public static string PlayerChosen(PaydayGameContainer mainGame, PaydayVMData model)
        {
            if (model.PopUpList!.Count() == 1)
                return model.PopUpList.GetText(1); //i think.
            CustomBasicList<PaydayPlayerItem> tempList = GetPossiblePlayerList(mainGame, model);
            MailCard thisMail = model.MailPile.GetCardInfo();
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
        public static int NumberChosen(PaydayVMData model)
        {
            int index = model.PopUpList!.ItemToChoose(true);
            int output;
            do
            {
                output = int.Parse(model.PopUpList.GetText(index));
                if (output > -1)
                {
                    return output; //the computer will always have a chance of playing lottery.
                }

            } while (true);
        }
        public static int BuyerSelected(PaydayMainGameClass mainGame)
        {
            var thisList = mainGame.SingleInfo!.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            return thisList.OrderByDescending(Items => Items.Value).ThenByDescending(Items => Items.Profit()).Take(1).Single().Deck;
        }
    }
}