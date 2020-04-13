using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    public abstract class KeeperActionViewModel : BasicKeeperScreen
    {
        private readonly IFluxxEvent _fluxxEvent;

        public abstract string ButtonText { get; }

        public KeeperActionViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent) : base(gameContainer, keeperContainer)
        {
            _fluxxEvent = fluxxEvent;
            keeperContainer.ButtonText = ButtonText;
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task ProcessAsync()
        {
            var thisList = GetSelectList();
            if (thisList.Count == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose a keeper");
                return;
            }
            if (KeeperContainer.Section == EnumKeeperSection.Exchange)
            {
                if (thisList.Count != 2)
                {
                    await UIPlatform.ShowMessageAsync("Must choose a keeper from yourself and from another player for exchange");
                    return;
                }
                if (ContainsCurrentPlayer(thisList) == false)
                {
                    await UIPlatform.ShowMessageAsync("Must choose a keeper from your keeper list in order to exchange");
                    return;
                }
                KeeperPlayer keeperFrom = new KeeperPlayer { Player = GameContainer.WhoTurn };
                var thisHand = GetCurrentPlayerKeeperHand(thisList);
                keeperFrom.Card = thisHand.ObjectSelected();
                if (keeperFrom.Card == 0)
                    throw new BasicBlankException("Keeper was never selected for current player");
                thisList.RemoveSpecificItem(thisHand);
                KeeperPlayer keeperTo = new KeeperPlayer();
                keeperTo.Player = GetPlayerOfKeeperHand(thisList.Single());
                keeperTo.Card = thisList.Single().ObjectSelected();
                if (keeperTo.Card == 0)
                    throw new BasicBlankException("Keeper was never selected for player");
                await _fluxxEvent.KeepersExchangedAsync(keeperFrom, keeperTo);
                return;
            }
            if (thisList.Count != 1)
            {
                await UIPlatform.ShowMessageAsync("Must choose only one keeper");
                return;
            }
            int index = GetPlayerOfKeeperHand(thisList.Single());
            bool isTrashed;
            if (index == GameContainer.WhoTurn && KeeperContainer.Section != EnumKeeperSection.Trash)
            {
                await UIPlatform.ShowMessageAsync("Cannot steal a keeper from yourself");
                return;
            }
            isTrashed = KeeperContainer.Section == EnumKeeperSection.Trash;
            KeeperPlayer tempKeep = new KeeperPlayer();
            tempKeep.Player = index;
            tempKeep.Card = thisList.Single().ObjectSelected();
            if (tempKeep.Card == 0)
                throw new BasicBlankException("Keeper was never selected");
            await _fluxxEvent.StealTrashKeeperAsync(tempKeep, isTrashed);
        }

        private int GetPlayerOfKeeperHand(HandObservable<KeeperCard> thisHand)
        {
            int index = KeeperContainer.KeeperHandList!.IndexOf(thisHand);
            return index + 1;
        }
        private bool ContainsCurrentPlayer(CustomBasicList<HandObservable<KeeperCard>> thisList)
        {
            return thisList.Any(items => GetPlayerOfKeeperHand(items) == GameContainer.WhoTurn);
        }
        private HandObservable<KeeperCard> GetCurrentPlayerKeeperHand(CustomBasicList<HandObservable<KeeperCard>> thisList)
        {
            return thisList.Single(items => GetPlayerOfKeeperHand(items) == GameContainer.WhoTurn);
        }

        private CustomBasicList<HandObservable<KeeperCard>> GetSelectList()
        {
            return KeeperContainer.KeeperHandList.Where(items => items.ObjectSelected() > 0).ToCustomBasicList();
        }


    }
}
