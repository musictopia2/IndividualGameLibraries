using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using CommonBasicStandardLibraries.Exceptions;
using LifeCardGameCP.Cards;
using LifeCardGameCP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCardGameCP.Logic
{
    public class LifeStoryHand : HandObservable<LifeCardGameCardInformation>
    {
        public int GetPlayerIndex { get; }
        public ILifeScroll? ThisScroll;
        private EnumSelectMode _mode = EnumSelectMode.ChooseCard;
        public EnumSelectMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                if ((int)_mode == (int)EnumSelectMode.ChoosePlayer)
                    AutoSelect = EnumAutoType.None;
                else
                    AutoSelect = EnumAutoType.SelectOneOnly;
            }
        }
        public bool IsCardSelected(int deck)
        {
            var thisCard = HandList.GetSpecificItem(deck);
            return thisCard.IsSelected;
        }
        public void ScrollToBottom() // i think
        {
            if (ThisScroll == null == true)
                return;
            ThisScroll!.ScrollToBottom();
        }
        public int TotalPoints() => HandList.Sum(items => items.Points);
        private bool _didProcess = false;
        private readonly int _myID;
        private readonly LifeCardGameGameContainer _gameContainer;
        private readonly LifeCardGameVMData _model;

        public void RemoveCard(LifeCardGameCardInformation thisCard)
        {
            HandList.RemoveObjectByDeck(thisCard.Deck);
        }
        public LifeStoryHand(LifeCardGameGameContainer gameContainer, LifeCardGameVMData model, int player) : base(gameContainer.Command)
        {
            _gameContainer = gameContainer;
            _model = model;
            GetPlayerIndex = player;
            AutoSelect = EnumAutoType.SelectOneOnly; //start with this.
            _myID = _gameContainer.PlayerList!.GetSelf().Id;
            HandList = new DeckObservableDict<LifeCardGameCardInformation>(); //try here too.
        }
        private bool CanPositionCard(LifeCardGameCardInformation thisCard)
        {
            if (_gameContainer.TradeList!.Count > 0)
                return _gameContainer.TradeList.ObjectExist(thisCard.Deck);
            if (_gameContainer.CardChosen == null == false)
            {
                return _gameContainer.CardChosen!.Deck == thisCard.Deck;
            }
            if (Mode == EnumSelectMode.ChoosePlayer)
                return true;
            if (_model.CurrentPile!.PileEmpty() == true)
                return true;
            var currentCard = _model.CurrentPile.GetCardInfo();
            if (currentCard.Action == EnumAction.None)
                return true;
            switch (currentCard.Action)
            {
                case EnumAction.AdoptBaby:
                    {
                        if ((int)thisCard.SwitchCategory == (int)EnumSwitchCategory.Baby)
                            return true;
                        else
                            return false;
                    }

                case EnumAction.CareerSwap:
                    {
                        if ((int)thisCard.SwitchCategory == (int)EnumSwitchCategory.Career)
                            return true;
                        else
                            return false;
                    }

                case EnumAction.DonateToCharity:
                    {
                        if ((int)thisCard.Category == (int)EnumFirstCardCategory.Wealth && thisCard.Points > 5 && (int)thisCard.SpecialCategory != (int)EnumSpecialCardCategory.Passport)
                            return true;
                        else
                            return false;
                    }

                case EnumAction.Lawsuit:
                    {
                        if (thisCard.Points >= 30 && (int)thisCard.SpecialCategory != (int)EnumSpecialCardCategory.Marriage)
                            return true;
                        else
                            return false;
                    }

                case EnumAction.LongLostRelative:
                    {
                        if ((int)thisCard.Category == (int)EnumFirstCardCategory.Family && (int)thisCard.SpecialCategory != (int)EnumSpecialCardCategory.Marriage && thisCard.Points > 5)
                            return true; // even if not married; can take the kid if they choose even though not married.  however; has to be married to put a kid down from your hand
                        else
                            return false;
                    }

                case EnumAction.MixUpAtVets:
                    {
                        if ((int)thisCard.SwitchCategory == (int)EnumSwitchCategory.Pet)
                            return true;
                        else
                            return false;
                    }

                case EnumAction.MovingHouse:
                    {
                        if ((int)thisCard.SpecialCategory == (int)EnumSpecialCardCategory.House)
                            return true;
                        else
                            return false;
                    }

                case EnumAction.YourStory:
                    {
                        if ((int)thisCard.Category == (int)EnumFirstCardCategory.Adventure && thisCard.Points > 5)
                            return true;
                        else
                            return false;
                    }

                case EnumAction.SecondChance:
                    {
                        if (thisCard.Points >= 10 && thisCard.Points <= 30 && (int)thisCard.SpecialCategory != (int)EnumSpecialCardCategory.Passport)
                            return true;
                        else
                            return false;
                    }
            }
            throw new BasicBlankException("Don't know whether can position card or not?");
        }
        public void RefreshCards()
        {
            if (ThisScroll == null)
                return;
            HandList.ForEach(thisCard => thisCard.Visible = CanPositionCard(thisCard));
            ThisScroll.RecalculatePositioning();
        }
        protected override void OnHandChange()
        {
            RefreshCards();
            HandList.Sort(); //hopefully this simple.
        }
        public void ClearBoard(LifeCardGameCardInformation thisCard)
        {
            HandList.ReplaceAllWithGivenItem(thisCard);
        }
        public void RemoveCards()
        {
            HandList.Clear();
        }
        public void AddCard(LifeCardGameCardInformation thisCard)
        {
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            HandList.Add(thisCard);
        }
        protected override async Task ProcessObjectClickedAsync(LifeCardGameCardInformation ThisObject, int Index)
        {
            if (_didProcess)
            {
                _didProcess = false;
                return;
            }
            await PlayerClickedAsync();
        }
        private async Task PlayerClickedAsync()
        {
            if (GetPlayerIndex == _myID)
                return;
            if (_gameContainer.BasicData!.MultiPlayer)
                await _gameContainer.Network!.SendAllAsync("choseplayer", GetPlayerIndex);
            if (_gameContainer.ChosePlayerAsync == null)
            {
                throw new BasicBlankException("Nobody is handling the chose player async.  Rethink");
            }
            await _gameContainer.ChosePlayerAsync.Invoke(GetPlayerIndex);
        }
        protected override async Task PrivateBoardSingleClickedAsync()
        {
            if (AutoSelect == HandObservable<LifeCardGameCardInformation>.EnumAutoType.SelectOneOnly)
                return;
            _didProcess = true;
            await PlayerClickedAsync();
        }
    }
}
