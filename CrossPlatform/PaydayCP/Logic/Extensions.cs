using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using PaydayCP.Cards;
using PaydayCP.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    public static class Extensions
    {
        public static DeckRegularDict<C> GetMailOrDealList<C>(this IDeckDict<CardInformation> tempList, EnumCardCategory whichType) where C : CardInformation, new()
        {

            var firstList = tempList.Where(items => items.CardCategory == whichType);
            DeckRegularDict<C> output = new DeckRegularDict<C>();
            foreach (var thisItem in firstList)
            {
                output.Add((C)thisItem);
            }
            return output;
        }
        public static async Task StartProcessPopUpAsync(this PaydayGameContainer gameContainer, PaydayVMData model)
        {
            if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData))
            {
                await gameContainer.Network!.SendAllAsync("popupchosen", model.PopUpChosen);
            }
            model.PopUpList.ShowOnlyOneSelectedItem(model.PopUpChosen);
            if (gameContainer.Test.NoAnimations == false)
            {
                await gameContainer.Delay.DelaySeconds(1.5);
            }
        }
        internal static void PopulateDeals(this PaydayGameContainer gameContainer, PaydayVMData model)
        {
            var tempList = gameContainer.SingleInfo!.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            model!.CurrentDealList!.HandList.ReplaceRange(tempList);
        }
        internal static void ReduceFromPlayer(this PaydayPlayerItem thisPlayer, decimal amount)
        {
            thisPlayer.MoneyHas -= amount;
            if (thisPlayer.MoneyHas < 0)
            {
                thisPlayer.Loans += Math.Abs(thisPlayer.MoneyHas);
                thisPlayer.MoneyHas = 0;
            }
            if (thisPlayer.MoneyHas < 0 || thisPlayer.Loans < 0)
            {
                throw new BasicBlankException("The money and loans must be 0 or greater");
            }
        }
        internal static void MonthLabel(this PaydayGameContainer gameContainer, PaydayVMData model)
        {
            model!.MonthLabel = $"Payday, month {gameContainer.SingleInfo!.CurrentMonth} of {gameContainer.SaveRoot!.MaxMonths}";
        }

        internal static void ProcessExpense(this PaydayGameContainer gameContainer, GameBoardProcesses gameBoard, decimal amount)
        {
            gameBoard!.AddToJackPot(amount);
            gameContainer.SingleInfo!.ReduceFromPlayer(amount);
        }

        internal static bool ShouldReshuffle(this PaydayGameContainer gameContainer)
        {
            gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
            if (gameContainer.BasicData!.MultiPlayer == false)
            {
                return true;
            }
            if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                return true;
            if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
                return false;
            if (gameContainer.BasicData.Client == false)
                return true;
            return false;
        }

    }
}