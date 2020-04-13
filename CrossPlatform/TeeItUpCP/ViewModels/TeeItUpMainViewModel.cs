using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TeeItUpCP.Cards;
using TeeItUpCP.Data;
using TeeItUpCP.Logic;

namespace TeeItUpCP.ViewModels
{
    [InstanceGame]
    public class TeeItUpMainViewModel : BasicCardGamesVM<TeeItUpCardInformation>
    {
        private readonly TeeItUpMainGameClass _mainGame; //if we don't need, delete.
        private readonly TeeItUpVMData _model;
        private readonly TeeItUpGameContainer _gameContainer; //if not needed, delete.

        public TeeItUpMainViewModel(CommandContainer commandContainer,
            TeeItUpMainGameClass mainGame,
            TeeItUpVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            TeeItUpGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            _model.OtherPile!.SendEnableProcesses(this, () => _mainGame!.SaveRoot!.GameStatus != EnumStatusType.Beginning);
            LoadPlayerBoards(); //hopefully this simple (?)
        }
        //anything else needed is here.


        private void LoadPlayerBoards()
        {
            //if (_mainGame.PlayerList.First().PlayerBoard != null)
            //    return;
            _mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.LoadPlayerBoard(_gameContainer!);
                thisPlayer.PlayerBoard!.SendEnableProcesses(this!, () =>
                {

                    //if (_mainGame.BasicData!.MultiPlayer == false)
                    //    return true;
                    if (_mainGame.SaveRoot!.GameStatus != EnumStatusType.Beginning)
                        return true;
                    return _mainGame.WhoTurn == thisPlayer.Id; //iffy
                    //return thisPlayer.PlayerCategory == EnumPlayerCategory.Self;
                });
            });
            //PlayerList!.ForEach(thisPlayer => thisPlayer.PlayerBoard!.ClearBoard(thisPlayer.MainHandList));
        }
        protected override bool CanEnableDeck()
        {
            return _mainGame!.SaveRoot!.GameStatus != EnumStatusType.Beginning && _model.OtherPile!.PileEmpty();
        }

        protected override bool CanEnablePile1()
        {
            return _mainGame!.SaveRoot!.GameStatus != EnumStatusType.Beginning;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int oldDeck;
            if (_model.OtherPile!.PileEmpty() == true)
                oldDeck = 0;
            else
                oldDeck = _model.OtherPile.GetCardInfo().Deck;
            if (oldDeck == 0)
            {
                await _mainGame!.PickupFromDiscardAsync();
                return;
            }
            if (oldDeck == _gameContainer!.PreviousCard)
            {
                await UIPlatform.ShowMessageAsync("Cannot discard the same card that was picked up");
                return;
            }
            TeeItUpPlayerItem tempItem;
            if (_gameContainer.BasicData!.MultiPlayer == true)
                tempItem = _mainGame!.PlayerList!.GetSelf();
            else
                tempItem = _mainGame!.PlayerList!.GetWhoPlayer();
            try
            {
                int matches = tempItem.PlayerBoard!.ColumnMatched(oldDeck);
                if (matches > 0)
                {
                    await UIPlatform.ShowMessageAsync("Cannot discard a card because there is a match");
                    return;
                }

            }
            catch (Exception ex)
            {
                throw new BasicBlankException($"Exception when trying to find out about column matching.  Message was {ex.Message}");
            }
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(oldDeck);
            if (thisCard.IsMulligan == true)
            {
                await UIPlatform.ShowMessageAsync("Cannot discard a Mulligan");
                return;
            }
            if (_mainGame.BasicData.MultiPlayer == true)
                await _gameContainer.SendDiscardMessageAsync(oldDeck);
            await _mainGame.DiscardAsync(oldDeck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }

        private int _round;
        [VM]
        public int Round
        {
            get { return _round; }
            set
            {
                if (SetProperty(ref _round, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _instructions = "";
        [VM]
        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

    }
}