using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using OldMaidCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace OldMaidCP.Logic
{
    [SingletonGame]
    public class OldMaidMainGameClass : CardGameClass<RegularSimpleCard, OldMaidPlayerItem, OldMaidSaveInfo>, IMiscDataNM
    {


        private readonly OldMaidVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly OldMaidGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly IOtherPlayerProcess _process;

        public OldMaidMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            OldMaidVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<RegularSimpleCard> cardInfo,
            CommandContainer command,
            OldMaidGameContainer gameContainer, IOtherPlayerProcess process)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _process = process;
            _gameContainer.SortCards = SortCards;
        }
        private async Task LinkOtherPlayerAsync()
        {
            if (SaveRoot!.PlayOrder.OtherTurn > 0)
            {
                _gameContainer!.OtherPlayer = PlayerList![SaveRoot.PlayOrder.OtherTurn];
            }
            if (SaveRoot.RemovePairs == false)
            {
                _process.SortOtherCards();
                if (_gameContainer.ShowOtherCardsAsync != null)
                {
                    await _gameContainer.ShowOtherCardsAsync.Invoke();
                }
            }
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            await LinkOtherPlayerAsync();
            //anything else needed is here.
            await base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task LastPartOfSetUpBeforeBindingsAsync()
        {
            await LinkOtherPlayerAsync();
            await base.LastPartOfSetUpBeforeBindingsAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
                await Delay!.DelayMilli(700);
            if (SaveRoot!.RemovePairs == false && SaveRoot.AlreadyChoseOne == false)
            {
                int deck = _gameContainer!.OtherPlayer!.MainHandList.GetRandomItem().Deck;
                await _process.SelectCardAsync(deck);
                return;
            }
            var tempList = ComputerPairList();
            if (tempList.Count() == 0)
            {
                await EndTurnAsync();
                return;
            }
            int deck1 = tempList.First().Deck;
            int deck2 = tempList.Last().Deck;
            await ProcessPlayAsync(deck1, deck2);
        }
        private DeckRegularDict<RegularSimpleCard> ComputerPairList()
        {
            DeckRegularDict<RegularSimpleCard> output = new DeckRegularDict<RegularSimpleCard>();
            foreach (var firstCard in SingleInfo!.MainHandList)
            {
                foreach (var secondCard in SingleInfo.MainHandList)
                {
                    if (firstCard.Deck != secondCard.Deck)
                    {
                        if (firstCard.Value == secondCard.Value)
                        {
                            output.Add(firstCard);
                            output.Add(secondCard);
                            return output;
                        }
                    }
                }
            }
            return output;
        }
        protected override async Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            if (_gameContainer.DeckList.Count != 49)
            {
                throw new BasicBlankException("The deck must be 49 cards for old maid");
            }
            if (_gameContainer.DeckList.Any(x => x.Value == EnumCardValueList.Queen && x.Suit != EnumSuitList.Spades))
            {
                throw new BasicBlankException("Only queen allowed is queen of spades");
            }
            SaveRoot!.PlayOrder.IsReversed = true;
            SaveRoot.PlayOrder.OtherTurn = await PlayerList!.CalculateWhoTurnAsync();
            SaveRoot.PlayOrder.IsReversed = false; //try this way.
            SaveRoot.RemovePairs = true;
            SaveRoot.AlreadyChoseOne = false;
            PlayerList.ForEach(thisPlayer => thisPlayer.InGame = true);
            await base.StartSetUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "processplay":
                    SendPlay thiss = await js.DeserializeObjectAsync<SendPlay>(content);
                    await ProcessPlayAsync(thiss.Card1, thiss.Card2);
                    break;
                case "cardselected":
                    await _process.SelectCardAsync(int.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.AlreadyChoseOne = false;
            await LinkOtherPlayerAsync();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            int lost = WhoLost();
            if (lost > 0)
            {
                SingleInfo = PlayerList[lost];
                await ShowLossAsync();
                return;
            }
            TakeOutGame();
            if (SingleInfo.MainHandList.Count > 0)
                SaveRoot!.PlayOrder.OtherTurn = WhoTurn;
            else
            {
                SaveRoot!.PlayOrder.IsReversed = true;
                SaveRoot.PlayOrder.OtherTurn = await PlayerList.CalculateWhoTurnAsync();
                SaveRoot.PlayOrder.IsReversed = false;
            }

            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            if (SaveRoot.RemovePairs == true && WhoTurn == WhoStarts)
            {
                SaveRoot.RemovePairs = false;
            }
            await StartNewTurnAsync();
        }


        public async Task ProcessPlayAsync(int card1, int card2)
        {
            if (SingleInfo!.CanSendMessage(BasicData!) == true)
            {
                SendPlay thiss = new SendPlay();
                thiss.Card1 = card1;
                thiss.Card2 = card2;
                await Network!.SendAllAsync("processplay", thiss);
            }
            SingleInfo!.MainHandList.RemoveObjectByDeck(card1);
            var secondCard = SingleInfo.MainHandList.RemoveObjectByDeck(card2);
            secondCard.Drew = false;
            secondCard.IsSelected = false;
            _model!.Pile1!.AddCard(secondCard);
            await ContinueTurnAsync();
        }
        internal bool IsValidMove(IDeckDict<RegularSimpleCard> thisCol)
        {
            return thisCol.HasDuplicates(items => items.Value); //hopefully its easier this way and still works.
        }
        private void TakeOutGame()
        {
            PlayerList!.ForConditionalItems(items => items.MainHandList.Count == 0, items => items.InGame = false);
        }
        private int WhoLost() //okay to return id this time.
        {
            int x = 0;
            foreach (var thisPlayer in PlayerList!)
            {
                x++;
                if (thisPlayer.MainHandList.Count == 1)
                {
                    var thisCard = thisPlayer.MainHandList.Single();
                    if (thisCard.Value == EnumCardValueList.Queen && thisCard.Suit == EnumSuitList.Spades)
                        return x;
                }
            }
            return 0;
        }

    }
}
