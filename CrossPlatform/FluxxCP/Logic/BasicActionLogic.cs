using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class BasicActionLogic
    {
        private readonly FluxxGameContainer _gameContainer;
        private readonly ActionContainer _actionContainer;
        private readonly FluxxDelegates _delegates;



        public BasicActionLogic(FluxxGameContainer gameContainer, ActionContainer actionContainer, FluxxDelegates delegates)
        {
            _gameContainer = gameContainer;
            _actionContainer = actionContainer;
            _delegates = delegates;
        }

        
        public async Task ShowMainScreenAgainAsync()
        {
            if (_delegates.LoadMainScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is loading the main screen.  Rethink");
            }
            await _delegates.LoadMainScreenAsync.Invoke();
        }

        public void LoadPlayers(bool includingSelf)
        {
            int oldSelectedIndex = _actionContainer.IndexPlayer;
            SimpleLoadPlayers(includingSelf);
            if (_gameContainer.CurrentAction!.Deck == EnumActionMain.UseWhatYouTake)
            {
                if (_actionContainer.Loads == 1)
                    _actionContainer.IndexPlayer = _gameContainer.SaveRoot!.SavedActionData.SelectedIndex;
                else
                    _actionContainer.IndexPlayer = oldSelectedIndex;
                if (_actionContainer.IndexPlayer > -1)
                    _actionContainer.Player1!.SelectSpecificItem(_actionContainer.IndexPlayer);
            }
        }

        internal void SavedPlayers()
        {
            SimpleLoadPlayers(true);
        }

        private void SimpleLoadPlayers(bool includingSelf)
        {
            var tempLists = _gameContainer.PlayerList!.ToCustomBasicList();
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            if (includingSelf == false)
                tempLists.RemoveSpecificItem(_gameContainer.SingleInfo);
            _actionContainer.PlayerIndexList.Clear();
            CustomBasicList<string> firstList = new CustomBasicList<string>();
            _actionContainer.IndexPlayer = -1;
            tempLists.ForEach(thisPlayer =>
            {
                _actionContainer.PlayerIndexList.Add(thisPlayer.Id);
                firstList.Add($"{thisPlayer.NickName}, {thisPlayer.MainHandList.Count} hand, # {_actionContainer.PlayerIndexList.Last()}");
            });
            _actionContainer.Player1!.LoadTextList(firstList);
        }

        public int GetPlayerIndex(int selectedIndex)
        {
            return _actionContainer.PlayerIndexList[selectedIndex];
        }

        public void LoadOtherCards(FluxxPlayerItem thisPlayer)
        {
            var thisList = thisPlayer.MainHandList.Select(items => items.Deck).ToCustomBasicList();
            DeckRegularDict<FluxxCardInformation> newList = new DeckRegularDict<FluxxCardInformation>();
            thisList.ForEach(thisItem =>
            {
                var thisCard = FluxxDetailClass.GetNewCard(thisItem);
                thisCard.Populate(thisItem); //i think this too.
                thisCard.IsUnknown = true;
                newList.Add(thisCard);
            });
            newList.ShuffleList();
            _actionContainer.OtherHand!.HandList.ReplaceRange(newList);
            _actionContainer.ButtonChooseCardVisible = true;
            _actionContainer.OtherHand.Visible = true;
        }

        public void LoadTempCards()
        {
            //TempHand!.Visible = true; //iffy.
            if (_gameContainer.TempActionHandList.Count == 0)
                throw new BasicBlankException("There are no cards left for the temp cards");
            _actionContainer.TempHand.AutoSelect = HandObservable<FluxxCardInformation>.EnumAutoType.SelectOneOnly;
            var firstList = _gameContainer.TempActionHandList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
            firstList.ForEach(thisCard => thisCard.IsUnknown = false);
            _actionContainer.TempHand.HandList.ReplaceRange(firstList);
        }


    }
}
