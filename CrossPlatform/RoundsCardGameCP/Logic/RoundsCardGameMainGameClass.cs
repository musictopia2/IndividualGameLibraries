using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RoundsCardGameCP.Cards;
using RoundsCardGameCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace RoundsCardGameCP.Logic
{
    [SingletonGame]
    public class RoundsCardGameMainGameClass
        : TrickGameClass<EnumSuitList, RoundsCardGameCardInformation, RoundsCardGamePlayerItem, RoundsCardGameSaveInfo>
        , IStartNewGame
    {


        private readonly RoundsCardGameVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly IAdvancedTrickProcesses _aTrick;

        public RoundsCardGameMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            RoundsCardGameVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<RoundsCardGameCardInformation> cardInfo,
            CommandContainer command,
            RoundsCardGameGameContainer gameContainer,
            ITrickData trickData,
            ITrickPlay trickPlay,
            IAdvancedTrickProcesses aTrick
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay)
        {
            _model = currentMod;
            _command = command;
            _aTrick = aTrick;
        }


        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            _model.TrickArea1!.Visible = true;
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }

        private bool IsGameOver
        {
            get
            {
                if (PlayerList.Count() != 2)
                    throw new BasicBlankException("Supposed to be a 2 player game.");
                SingleInfo = PlayerList.Where(Items => Items.TotalScore >= 200).OrderByDescending(Items => Items.TotalScore).FirstOrDefault();
                return SingleInfo != null;
            }
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            var moveList = SingleInfo!.MainHandList.Where(items => IsValidMove(items.Deck)).ToRegularDeckDict();
            await PlayCardAsync(moveList.GetRandomItem().Deck);
        }
        private void CardScoring()
        {
            if (SaveRoot.CardList.Any(x => x.Player == 0))
            {
                throw new BasicBlankException("Every card has to be won by somebody.  Rethink");
            }
            SaveRoot.CardList.ForEach(thisCard =>
            {
                if (thisCard.Value == EnumCardValueList.Two && thisCard.Suit == EnumSuitList.Hearts)
                    thisCard.Points = 10;
                else if (thisCard.Value <= EnumCardValueList.Ten)
                    thisCard.Points = 0;
                else
                {
                    thisCard.Points = 10 - (int)thisCard.Value;
                    thisCard.Points *= -1;
                }
            });
        }

        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            LoadVM();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TricksWon = 0;
                thisPlayer.RoundsWon = 0;
            });
            _model.TrickArea1!.Visible = true;
            _aTrick!.ClearBoard(); //try this too.
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            SaveRoot.CardList.Clear();
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }

        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.

            //anything else is here.  varies by game.


            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await StartNewTurnAsync(); //i think.
        }

        private int WhoWonTrick(DeckObservableDict<RoundsCardGameCardInformation> thisCol)
        {
            var leadCard = thisCol.First();
            var thisCard = thisCol.Last();
            var tempCard = _model!.Pile1!.GetCardInfo();
            var trumpSuit = tempCard.Suit;
            var trumpNumber = tempCard.Value;
            if (thisCard.Value == trumpNumber)
                return WhoTurn;
            if (leadCard.Value == trumpNumber)
                return leadCard.Player;
            if (thisCard.Suit == trumpSuit && thisCard.Suit != leadCard.Suit)
                return WhoTurn;
            if (leadCard.Suit == trumpSuit && thisCard.Suit != leadCard.Suit)
                return leadCard.Player;
            if (thisCard.Suit == leadCard.Suit)
            {
                if (thisCard.Value > leadCard.Value)
                    return WhoTurn;
            }
            return leadCard.Player;
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            RoundsCardGamePlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            await _aTrick!.AnimateWinAsync(wins);
            trickList.ForEach(card =>
            {
                card.Player = wins; //hopefully this helps fix both things.
                SaveRoot.CardList.Add(card);
            });

            if (SingleInfo!.MainHandList.Count == 0)
            {
                thisPlayer.TricksWon++; //whoever wins the last one gets a double one.
                var thisList = _model!.Pile1!.DiscardList();
                thisList.Add(_model!.Pile1!.GetCardInfo());
                //thisList.ForEach(thisCard => thisCard.Player = wins);
                thisList.ForEach(card =>
                {
                    card.Player = wins;
                    SaveRoot.CardList.Add(card);
                });
                CalculateMajority();
                await EndRoundAsync();
                return; //most of the time its in rounds.
            }
            _model!.PlayerHand1!.EndTurn();
            SingleInfo = PlayerList.GetSelf();
            if (SingleInfo.MainHandList.Count == 4)
            {
                if (_model!.Deck1!.IsEndOfDeck() == false)
                    await StartNextPartAsync(true);
                else
                    await StartNextPartAsync(false);
            }
            WhoTurn = wins; //most of the time, whoever wins leads again.
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            _command!.ManuelFinish = true; //because it could be somebody else's turn.
            await StartNewTurnAsync(); //hopefully this simple.
        }

        private void CalculateMajority()
        {
            int score1 = PlayerList.First().TricksWon;
            if (score1 >= 3)
                PlayerList.First().RoundsWon++;
            else
                PlayerList.Last().RoundsWon++;
        }
        private async Task StartNextPartAsync(bool drawExtraCards)
        {
            CalculateMajority();
            await UIPlatform.ShowMessageAsync("Round is over.  Look at the scores then continue");
            if (drawExtraCards == true)
            {
                int x;
                for (x = 1; x <= 5; x++)
                {
                    foreach (var thisPlayer in PlayerList!)
                    {
                        var thisCard = _model!.Deck1!.DrawCard();
                        thisPlayer.MainHandList.Add(thisCard);
                    }
                }
                var newCard = _model!.Deck1!.DrawCard();
                _model!.Pile1!.AddCard(newCard);
            }
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TricksWon = 0;
            });
            SingleInfo = PlayerList.GetSelf();
            SortCards();
        }
        private void CalculateScores()
        {
            int player1Points = PlayerList.First().RoundsWon * 10; //its rounds, not tricks.
            int player2Points = PlayerList.Last().RoundsWon * 10;
            player1Points += SaveRoot.CardList.Where(items => items.Player == 1).Sum(items => items.Points);
            player2Points += SaveRoot.CardList.Where(items => items.Player == 2).Sum(items => items.Points);
            PlayerList.First().CurrentPoints = player1Points;
            PlayerList.First().TotalScore += player1Points;
            PlayerList.Last().CurrentPoints = player2Points;
            PlayerList.Last().TotalScore += player2Points;
        }
        public override async Task EndRoundAsync()
        {
            CardScoring();
            CalculateScores();
            if (IsGameOver == true)
            {
                await ShowWinAsync();
                return;
            }
            await this.RoundOverNextAsync();
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.CurrentPoints = 0;
            });
            return Task.CompletedTask;
        }

    }
}