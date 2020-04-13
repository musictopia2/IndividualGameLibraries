using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using System.Linq;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class LoadActionProcesses : ILoadActionProcesses
    {
        private readonly ActionContainer _actionContainer;
        private readonly FluxxVMData _model;
        private readonly BasicActionLogic _basicActionLogic;
        private readonly FluxxGameContainer _gameContainer;

        public LoadActionProcesses(ActionContainer actionContainer,
            FluxxVMData model,
            BasicActionLogic basicActionLogic,
            FluxxGameContainer gameContainer
            )
        {
            _actionContainer = actionContainer;
            _model = model;
            _basicActionLogic = basicActionLogic;
            _gameContainer = gameContainer;
        }
        void ILoadActionProcesses.LoadDirections()
        {
            _actionContainer.ActionCategory = EnumActionCategory.Directions;
            _actionContainer.Direction1!.UnselectAll();
            _actionContainer.IndexDirection = -1;
        }

        void ILoadActionProcesses.LoadDoAgainCards()
        {
            _actionContainer.ActionCategory = EnumActionCategory.DoAgain;
            var firstList = _model.Pile1!.DiscardList();
            DeckRegularDict<FluxxCardInformation> secondList = new DeckRegularDict<FluxxCardInformation>();
            firstList.ForEach(x =>
            {
                if (x.Deck == 0)
                {
                    throw new BasicBlankException("Deck cannot be 0 for load action again.  Rethink");
                }
                var card = FluxxDetailClass.GetNewCard(x.Deck);
                card.Populate(x.Deck); //maybe this is needed too.
                secondList.Add(card);
            });
            var tempList = secondList.Where(items => items.CanDoCardAgain()).Select(items => items.Text()).ToCustomBasicList();
            if (tempList.Count == 0)
                throw new BasicBlankException("Must have at least one card to do again.  Otherwise; can't do again");
            _actionContainer.IndexCard = -1;
            _actionContainer.CardList1!.LoadTextList(tempList);
        }

        void ILoadActionProcesses.LoadDrawUse()
        {
            _actionContainer.ActionCategory = EnumActionCategory.DrawUse;
            _basicActionLogic.LoadTempCards();
            _actionContainer.ButtonChooseCardVisible = true;
        }

        void ILoadActionProcesses.LoadEverybodyGetsOne()
        {
            _actionContainer.ActionCategory = EnumActionCategory.Everybody1;
            _basicActionLogic.LoadTempCards();
            if (_gameContainer.EverybodyGetsOneList.Count == 0)
            {
                _basicActionLogic.LoadPlayers(true);
                return;
            }
            var tempLists = _gameContainer.PlayersLeftForEverybodyGetsOne();
            if (tempLists.Count < 2)
                throw new BasicBlankException("Cannot load everybody gets one because less than 2 players");
            _actionContainer.PlayerIndexList.Clear();
            if (_gameContainer.IncreaseAmount() == 1)
                _actionContainer.TempHand!.AutoSelect = HandObservable<FluxxCardInformation>.EnumAutoType.SelectAsMany;
            _actionContainer.IndexPlayer = -1;
            CustomBasicList<string> firstList = new CustomBasicList<string>();
            tempLists.ForEach(thisPlayer =>
            {
                _actionContainer.PlayerIndexList.Add(thisPlayer.Id);
                firstList.Add($"{thisPlayer.NickName}, {thisPlayer.MainHandList.Count} cards in hand, {thisPlayer.Id}");
            });
            _actionContainer.Player1!.LoadTextList(firstList);
        }

        void ILoadActionProcesses.LoadFirstRandom()
        {
            _actionContainer.ActionCategory = EnumActionCategory.FirstRandom;
            _actionContainer.OtherHand.Visible = true;
            var turnPlayer = _gameContainer.PlayerList!.GetWhoPlayer();
            _basicActionLogic.LoadOtherCards(turnPlayer);
            _actionContainer.ActionFrameText = $"{_gameContainer.SingleInfo!.NickName} will choose a card for {turnPlayer.NickName}";
        }

        void ILoadActionProcesses.LoadRules()
        {
            _actionContainer.ActionCategory = EnumActionCategory.Rules;
            var tempList = _gameContainer.SaveRoot!.RuleList.Where(items => items.Category != EnumRuleCategory.Basic).Select(items => items.Text()).ToCustomBasicList();
            if (tempList.Count == 0)
                throw new BasicBlankException("Cannot load the rules because there are no rules");
            if (_gameContainer.CurrentAction!.Deck == EnumActionMain.TrashANewRule)
            {
                _actionContainer.Rule1!.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
                _actionContainer.RulesToDiscard = 1;
            }
            else
            {
                _actionContainer.Rule1!.SelectionMode = ListViewPicker.EnumSelectionMode.MultipleItems;
                _actionContainer.RulesToDiscard = tempList.RulesThatCanBeDiscarded();
            }
            _actionContainer.Rule1.LoadTextList(tempList);
        }

        void ILoadActionProcesses.LoadTradeHands()
        {
            _actionContainer.ActionCategory = EnumActionCategory.TradeHands;
            _basicActionLogic.LoadPlayers(false);
            _actionContainer.ButtonChoosePlayerVisible = true;
        }

        void ILoadActionProcesses.LoadUseTake()
        {
            _actionContainer.ActionCategory = EnumActionCategory.UseTake;
            _basicActionLogic.LoadPlayers(false);
            if (_actionContainer.IndexPlayer > -1)
            {
                _actionContainer.OtherHand!.Visible = true;
                int index = _actionContainer.GetPlayerIndex(_actionContainer.IndexPlayer);
                var thisPlayer = _gameContainer.PlayerList![index];
                _actionContainer.ActionFrameText = "Other Cards";
                _basicActionLogic.LoadOtherCards(thisPlayer);
            }
            else
            {
                _actionContainer.ButtonChoosePlayerVisible = true;
            }
        }
    }
}