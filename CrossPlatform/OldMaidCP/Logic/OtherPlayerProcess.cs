using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using OldMaidCP.Data;
using System.Threading.Tasks;

namespace OldMaidCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class OtherPlayerProcess : IOtherPlayerProcess
    {
        private readonly OldMaidGameContainer _gameContainer;
        private readonly OldMaidVMData _model;

        public OtherPlayerProcess(OldMaidGameContainer gameContainer, OldMaidVMData model)
        {
            _gameContainer = gameContainer;
            _model = model;
        }

        public void SortOtherCards()
        {
            DeckRegularDict<RegularSimpleCard> output = new DeckRegularDict<RegularSimpleCard>();
            _gameContainer!.OtherPlayer!.MainHandList.ForEach(thisCard =>
            {
                RegularSimpleCard newCard = new RegularSimpleCard();
                newCard.Populate(thisCard.Deck);
                newCard.IsUnknown = true;
                output.Add(newCard);
            });
            output.ShuffleList();
            _model.OpponentCards1!.HandList.ReplaceRange(output);
        }

        async Task IOtherPlayerProcess.SelectCardAsync(int deck)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
                await _gameContainer.Network!.SendAllAsync("cardselected", deck);
            if (_gameContainer!.OtherPlayer!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.PlayerHand1!.SelectOneFromDeck(deck);
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay!.DelaySeconds(.75);
            }
            _gameContainer.OtherPlayer.MainHandList.RemoveObjectByDeck(deck);
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            thisCard.IsUnknown = false;
            thisCard.IsSelected = false;
            SortOtherCards();
            _gameContainer.SingleInfo!.MainHandList.Add(thisCard);
            if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisCard.Drew = true;
                if (_gameContainer.SortCards == null)
                {
                    throw new BasicBlankException("Nobody is handling sort cards.  Rethink");
                }
                _gameContainer.SortCards.Invoke();
            }
            _gameContainer.SaveRoot!.AlreadyChoseOne = true;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
    }
}