using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class ActionEverybodyGetsOneViewModel : BasicActionScreen
    {
        public ActionEverybodyGetsOneViewModel(FluxxGameContainer gameContainer,
            ActionContainer actionContainer,
            KeeperContainer keeperContainer,
            FluxxDelegates delegates,
            IFluxxEvent fluxxEvent,
            BasicActionLogic basicActionLogic) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic)
        {
        }

        

        public bool CanGiveCards => ActionContainer.IndexPlayer > -1;
        [Command(EnumCommandCategory.Plain)]
        public async Task GiveCardsAsync()
        {
            CustomBasicList<int> thisList;
            if (ActionContainer.TempHand!.AutoSelect == HandObservable<FluxxCardInformation>.EnumAutoType.SelectOneOnly)
            {
                if (ActionContainer.TempHand.ObjectSelected() == 0)
                {
                    await UIPlatform.ShowMessageAsync("Must choose a card to give to a player");
                    return;
                }
                thisList = new CustomBasicList<int>() { ActionContainer.TempHand.ObjectSelected() };
                await BasicActionLogic.ShowMainScreenAgainAsync();
                await FluxxEvent.ChoseForEverybodyGetsOneAsync(thisList, ActionContainer.IndexPlayer);
                return;
            }
            if (ActionContainer.TempHand.HowManySelectedObjects > 2)
            {
                await UIPlatform.ShowMessageAsync("Cannot choose more than 2 cards to give to player");
                return;
            }
            int index = ActionContainer.GetPlayerIndex(ActionContainer.IndexPlayer);
            int howManySoFar = GameContainer.EverybodyGetsOneList.Count(items => items.Player == index);
            howManySoFar += ActionContainer.TempHand.HowManySelectedObjects;
            int extras = GameContainer.IncreaseAmount();
            int mosts = extras + 1;
            if (howManySoFar > mosts)
            {
                await UIPlatform.ShowMessageAsync($"Cannot choose more than 2 cards each for the player.  So far; you chose {howManySoFar} cards.");
                return;
            }
            var finalList = ActionContainer.TempHand.ListSelectedObjects();
            thisList = finalList.GetDeckListFromObjectList();
            await BasicActionLogic.ShowMainScreenAgainAsync();
            await FluxxEvent.ChoseForEverybodyGetsOneAsync(thisList, ActionContainer.IndexPlayer);
        }
    }
}