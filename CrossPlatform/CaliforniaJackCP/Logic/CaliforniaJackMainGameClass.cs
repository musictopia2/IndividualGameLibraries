using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CaliforniaJackCP.Cards;
using CaliforniaJackCP.Data;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace CaliforniaJackCP.Logic
{
    [SingletonGame]
    public class CaliforniaJackMainGameClass
        : TrickGameClass<EnumSuitList, CaliforniaJackCardInformation, CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>
        , IStartNewGame
    {


        private readonly CaliforniaJackVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly CaliforniaJackGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly IAdvancedTrickProcesses _aTrick;

        public CaliforniaJackMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            CaliforniaJackVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<CaliforniaJackCardInformation> cardInfo,
            CommandContainer command,
            CaliforniaJackGameContainer gameContainer,
            ITrickData trickData,
            ITrickPlay trickPlay,
            IAdvancedTrickProcesses aTrick
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _aTrick = aTrick;
        }


        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            //anything else needed is here.
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.DoubleCheck == true)
                return; //so will be stuck.  this way i can test the human player first.
            if (Test.NoAnimations == true)
                await Delay!.DelaySeconds(.75);
            var MoveList = SingleInfo!.MainHandList.Where(ThisCard => IsValidMove(ThisCard.Deck)).Select(Items => Items.Deck).ToCustomBasicList();
            await PlayCardAsync(MoveList.GetRandomItem());
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            LoadVM();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TricksWon = 0;
                thisPlayer.Points = 0; //i think.
            });
            return base.StartSetUpAsync(isBeginning);
        }
        protected override async Task LastPartOfSetUpBeforeBindingsAsync()
        {
            var thisCard = _model!.Deck1!.RevealCard();
            SaveRoot!.TrumpSuit = thisCard.Suit;
            SaveRoot.CardList.Clear();
            _aTrick!.ClearBoard(); //i think.
            await base.LastPartOfSetUpBeforeBindingsAsync();
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
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await ContinueTurnAsync();
        }
        private void CardScoring()
        {
            SaveRoot.CardList.ForEach(thisCard =>
            {
                if (thisCard.Value == EnumCardValueList.HighAce || thisCard.Value == EnumCardValueList.LowAce)
                    thisCard.Points = 4;
                else if (thisCard.Value == EnumCardValueList.Ten)
                    thisCard.Points = 10;
                else if (thisCard.Value == EnumCardValueList.Jack)
                    thisCard.Points = 1;
                else if (thisCard.Value == EnumCardValueList.Queen)
                    thisCard.Points = 2;
                else if (thisCard.Value == EnumCardValueList.King)
                    thisCard.Points = 3;
                else
                    thisCard.Points = 0;
            });
        }
        private void CalculatePointsSoFar(DeckObservableDict<CaliforniaJackCardInformation> trickList, int wins)
        {
            int points = trickList.Count(thisCard => thisCard.Suit == SaveRoot!.TrumpSuit && (thisCard.Value == EnumCardValueList.Two
                || thisCard.Value == EnumCardValueList.Jack ||
                thisCard.Value == EnumCardValueList.HighAce || thisCard.Value == EnumCardValueList.LowAce));
            PlayerList![wins].Points += points;
        }
        private int WhoWonTrick(DeckObservableDict<CaliforniaJackCardInformation> thisCol)
        {
            CaliforniaJackCardInformation leadCard = thisCol.First();
            var thisCard = thisCol.Last();
            if (thisCard.Suit == SaveRoot!.TrumpSuit && leadCard.Suit != SaveRoot.TrumpSuit)
                return WhoTurn;
            if (leadCard.Suit == SaveRoot.TrumpSuit && thisCard.Suit != SaveRoot.TrumpSuit)
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
            trickList.ForEach(thisCard =>
            {
                CaliforniaJackCardInformation newCard = new CaliforniaJackCardInformation();
                newCard.Populate(thisCard.Deck);
                newCard.Player = wins;
                SaveRoot.CardList.Add(newCard);
            });
            CaliforniaJackPlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            CalculatePointsSoFar(trickList, wins);
            await _aTrick!.AnimateWinAsync(wins);
            if (SingleInfo!.MainHandList.Count == 0)
            {
                await EndRoundAsync();
                return; //most of the time its in rounds.
            }
            _model!.PlayerHand1!.EndTurn();
            WhoTurn = wins; //most of the time, whoever wins leads again.
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            if (_model.Deck1!.IsEndOfDeck() == false)
            {
                CaliforniaJackCardInformation thisCard;
                thisCard = _model.Deck1.DrawCard();
                SingleInfo = PlayerList!.GetWhoPlayer();
                SingleInfo.MainHandList.Add(thisCard);
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                {
                    thisCard.Drew = true;
                    SortCards(); //i think.
                }
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                CaliforniaJackPlayerItem tempPlayer;
                if (WhoTurn == 1)
                    tempPlayer = PlayerList[2];
                else
                    tempPlayer = PlayerList[1];
                thisCard = _model.Deck1.DrawCard();
                tempPlayer.MainHandList.Add(thisCard);
                if (tempPlayer.PlayerCategory == EnumPlayerCategory.Self)
                {
                    thisCard.Drew = true;
                    SortCards(); //i think.
                }
            }
            await StartNewTurnAsync(); //hopefully this simple.
        }
        private int _game;
        public override async Task EndRoundAsync()
        {
            CardScoring();
            _game = WhoReceivesLastPoint();
            if (_game > 0)
                PlayerList![_game].Points++;
            AddToTotals();
            if (PlayerList.Any(items => items.TotalScore >= 10))
            {
                await GameOverAsync();
                return;
            }
            await this.RoundOverNextAsync();
        }
        private int WhoReceivesLastPoint()
        {
            int output = 0;
            int x;
            int previousPoints = 0;
            int points;
            for (x = 1; x <= 2; x++)
            {
                points = PointsReceived(x);
                if (points > previousPoints)
                {
                    previousPoints = points;
                    output = x;
                }
                else if ((points == previousPoints) & (previousPoints > 0))
                    output = 0;// i think at this point, it will be 0.
            }
            return output;
        }
        private int PointsReceived(int player)
        {
            return SaveRoot.CardList.Where(items => items.Player == player).Sum(x => x.Points);
        }
        private int WhoWonGame()
        {
            int firsts;
            int Seconds;
            firsts = PlayerList.First().TotalScore;
            Seconds = PlayerList.Last().TotalScore; // because its 2 player only.
            if (firsts > Seconds)
                return 1;
            if (Seconds > firsts)
                return 2;
            if (firsts == Seconds)
            {
                var thisPlayer = PlayerList![_game]; // not 0 based anymore.
                thisPlayer.Points -= 1; // somehow reduced by 1
                thisPlayer.TotalScore -= 1; // has to reduce by one i think
                if (_game == 1)
                    return 2;
                return 1;
            }
            return _gameContainer.Random.GetRandomNumber(2);
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList![WhoWonGame()];
            await this.ShowWinAsync();
        }
        private void AddToTotals()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore += thisPlayer.Points;
            });
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.Points = 0;
                thisPlayer.TricksWon = 0;
            });
            return Task.CompletedTask;
        }

    }
}
