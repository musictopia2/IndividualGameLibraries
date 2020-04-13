using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GoFishCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GoFishCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class AskProcesses : IAskProcesses
    {
        private readonly GoFishGameContainer _gameContainer;
        private readonly GoFishVMData _model;

        public AskProcesses(GoFishGameContainer gameContainer, GoFishVMData model)
        {
            _gameContainer = gameContainer;
            _model = model;
        }

        void IAskProcesses.LoadAskList()
        {
            if (_gameContainer.LoadAskScreenAsync != null)
            {
                //maybe here.
                _gameContainer.LoadAskScreenAsync.Invoke();
            }
            if (_gameContainer!.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.AskList.LoadFromHandCardValues(_gameContainer.SingleInfo);
            }
            else
                _model.AskList.LoadEntireList(); //i think
            _model.CardYouAsked = EnumCardValueList.None;
            _model.AskList.UnselectAll(); //just in case.
        }

        async Task IAskProcesses.NumberToAskAsync(EnumCardValueList asked)
        {
            _gameContainer.SaveRoot!.NumberAsked = true;
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                _model!.AskList!.SelectSpecificItem(asked); //i think.
            GoFishPlayerItem otherPlayer = GetPlayer();
            await NumberToAskAsync(asked, otherPlayer);
        }

        private async Task NumberToAskAsync(EnumCardValueList whichOne, GoFishPlayerItem otherPlayer)
        {
            if (otherPlayer.MainHandList.Count == 0)
            {
                await _gameContainer.ContinueTurnAsync!();
                return;
            }
            _model!.AskList!.SelectSpecificItem(whichOne); //i think.
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(1);
            DeckRegularDict<RegularSimpleCard> thisList;
            if (_gameContainer.Test.AllowAnyMove == false || otherPlayer.MainHandList.Count != 1)
                thisList = otherPlayer.MainHandList.Where(items => items.Value == whichOne).ToRegularDeckDict();
            else
                thisList = otherPlayer.MainHandList.ToRegularDeckDict();
            if (thisList.Count == 0)
            {
                if (_model.Deck1!.IsEndOfDeck() == false)
                {
                    if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                        await UIPlatform.ShowMessageAsync("Go Fish");
                    _gameContainer.LeftToDraw = 0;
                    _gameContainer.PlayerDraws = 0;
                    await _gameContainer.DrawAsync!.Invoke();
                    return;
                }
                if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    await UIPlatform.ShowMessageAsync("No more cards left to draw.  Therefore; will have to end your turn");
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            if (otherPlayer.PlayerCategory == EnumPlayerCategory.Self && _gameContainer.Test.NoAnimations == false)
            {
                thisList.ForEach(items =>
                {
                    _model.PlayerHand1!.SelectOneFromDeck(items.Deck);
                });
                await _gameContainer.Delay!.DelaySeconds(1); //so you can see what you have to get rid of.
            }
            thisList.ForEach(items =>
            {
                otherPlayer.MainHandList.RemoveObjectByDeck(items.Deck);
                if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    items.Drew = true;
                _gameContainer.SingleInfo.MainHandList.Add(items);
            });
            if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                _gameContainer.SortAfterDrawing!.Invoke(); //i think.
            if (otherPlayer.MainHandList.Count == 0)
            {
                int cards = _model.Deck1!.CardsLeft();
                if (cards < 5 && cards > 0)
                {
                    _gameContainer.LeftToDraw = cards;
                    _gameContainer.PlayerDraws = otherPlayer.Id;
                    await _gameContainer.DrawAsync!.Invoke();
                    return;
                }
                else if (cards > 0)
                {
                    _gameContainer.LeftToDraw = 5;
                    _gameContainer.PlayerDraws = otherPlayer.Id;
                    await _gameContainer.DrawAsync!.Invoke();
                    return;
                }
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();

        }
        private GoFishPlayerItem GetPlayer()
        {
            if (_gameContainer.PlayerList.Count() > 2)
                throw new BasicBlankException("Since there are more than 2 players; needs to know the nick name");
            int nums;
            GoFishPlayerItem tempPlayer;
            if (_gameContainer.WhoTurn == 1)
                nums = 2;
            else
                nums = 1;
            _gameContainer.SaveRoot!.PlayOrder.OtherTurn = nums;
            tempPlayer = _gameContainer.PlayerList![nums]; //one based now
            return tempPlayer;
        }


    }
}