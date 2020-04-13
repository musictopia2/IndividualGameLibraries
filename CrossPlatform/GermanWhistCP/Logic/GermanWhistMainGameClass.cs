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
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using GermanWhistCP.Cards;
using GermanWhistCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GermanWhistCP.Logic
{
    [SingletonGame]
    public class GermanWhistMainGameClass
        : TrickGameClass<EnumSuitList, GermanWhistCardInformation, GermanWhistPlayerItem, GermanWhistSaveInfo>
    {


        private readonly GermanWhistVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly IAdvancedTrickProcesses _aTrick;

        public GermanWhistMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            GermanWhistVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<GermanWhistCardInformation> cardInfo,
            CommandContainer command,
            GermanWhistGameContainer gameContainer,
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
            });
            SaveRoot!.WasEnd = false;
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            var thisCard = _model!.Deck1!.RevealCard();
            SaveRoot!.TrumpSuit = thisCard.Suit;
            _aTrick!.ClearBoard(); //i think.
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
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await ContinueTurnAsync();
        }

        private int WhoWonTrick(DeckObservableDict<GermanWhistCardInformation> thisCol)
        {
            GermanWhistCardInformation leadCard = thisCol.First();
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
            GermanWhistPlayerItem ThisPlayer = PlayerList![wins];
            if (SaveRoot.WasEnd == true)
                ThisPlayer.TricksWon++;
            else if (_model!.Deck1!.IsEndOfDeck() == true)
            {
                SaveRoot.WasEnd = true;
                ThisPlayer.TricksWon++;
            }
            await _aTrick!.AnimateWinAsync(wins);
            if (SingleInfo!.MainHandList.Count == 0)
            {
                await GameOverAsync();
                return;
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
                GermanWhistCardInformation thisCard;
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
                GermanWhistPlayerItem TempPlayer;
                if (WhoTurn == 1)
                    TempPlayer = PlayerList[2];
                else
                    TempPlayer = PlayerList[1];
                thisCard = _model.Deck1.DrawCard();
                TempPlayer.MainHandList.Add(thisCard);
                if (TempPlayer.PlayerCategory == EnumPlayerCategory.Self)
                {
                    thisCard.Drew = true;
                    SortCards(); //i think.
                }
            }
            await StartNewTurnAsync(); //hopefully this simple.
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderByDescending(Items => Items.TricksWon).Take(1).Single();
            await this.ShowWinAsync();
        }

    }
}
