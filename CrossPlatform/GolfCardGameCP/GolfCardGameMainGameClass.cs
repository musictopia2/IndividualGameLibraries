using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace GolfCardGameCP
{
    [SingletonGame]
    public class GolfCardGameMainGameClass : CardGameClass<RegularSimpleCard, GolfCardGamePlayerItem, GolfCardGameSaveInfo>, IMiscDataNM, IStartNewGame
    {
        internal async Task ChangeHandAsync(int whichOne)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("changehand", whichOne);
            var thisCard = SingleInfo.MainHandList[whichOne]; //i think.
            var newCard = _thisMod!.Pile2!.GetCardInfo();
            var tempList = SingleInfo.MainHandList.ToRegularDeckDict();
            if (whichOne == 0) //because 0 based.
            {
                tempList.RemoveFirstItem();
                tempList.InsertBeginning(newCard);
            }
            else
            {
                tempList.RemoveLastItem();
                tempList.Add(newCard);
            }
            SingleInfo.MainHandList.ReplaceRange(tempList);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _thisMod.GolfHand1!.ChangeCard(whichOne, newCard);
                _thisMod.Pile2.AddCard(thisCard);
            }
            await ContinueTurnAsync();
        }
        internal async Task ChangeUnknownAsync(int whichOne)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("changeunknown", whichOne);
            var thisCard = SingleInfo.TempSets[whichOne];
            var newCard = _thisMod!.Pile2!.GetCardInfo();
            var tempList = SingleInfo.TempSets.ToRegularDeckDict();
            if (whichOne == 0)
            {
                tempList.RemoveFirstItem();
                tempList.InsertBeginning(newCard);
                SingleInfo.FirstChanged = true;
            }
            else
            {
                tempList.RemoveLastItem();
                tempList.Add(newCard);
                SingleInfo.SecondChanged = true;
            }
            SingleInfo.TempSets.ReplaceRange(tempList);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                _thisMod.HiddenCards1!.ChangeCard(whichOne, newCard);
            thisCard.IsUnknown = false;
            await DiscardAsync(thisCard);
        }
        internal async Task SelectBeginningAsync(int player, DeckRegularDict<RegularSimpleCard> selectList, DeckRegularDict<RegularSimpleCard> unselectList)
        {
            if (selectList.Count != 2 || unselectList.Count != 2)
                throw new BasicBlankException("The select and unselect list must contain 2 cards");
            SingleInfo = PlayerList![player];
            SingleInfo.MainHandList.ReplaceRange(selectList);
            SingleInfo.TempSets.ReplaceRange(unselectList);
            SingleInfo.FinishedChoosing = true;
            _thisMod!.CommandContainer!.ManuelFinish = true;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _thisMod.HiddenCards1!.ClearBoard();
                _thisMod.GolfHand1!.ClearBoard();
            }
            if (!ThisData!.MultiPlayer)
            {
                SaveRoot!.GameStatus = EnumStatusType.Normal;
                WhoStarts = WhoTurn;
                await StartNewTurnAsync();
                return;
            }
            if (PlayerList.Any(items => !items.FinishedChoosing))
            {
                _thisMod.Instructions = "Waiting for the other players to finish choosing the 2 cards";
                ThisCheck!.IsEnabled = true;
                return;
            }
            SaveRoot!.GameStatus = EnumStatusType.Normal;
            await StartNewTurnAsync();
        }
        internal async Task KnockAsync() //the message already sent on this one.
        {
            SingleInfo = PlayerList!.GetWhoPlayer(); //just in case.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
                await _thisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} has knocked.  Therefore; all players gets one more turn");
            SingleInfo.Knocked = true;
            SaveRoot!.GameStatus = EnumStatusType.Knocked;
            await EndTurnAsync();
        }
        public GolfCardGameMainGameClass(IGamePackageResolver container) : base(container) { }

        private GolfCardGameViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<GolfCardGameViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            if (SaveRoot!.GameStatus != EnumStatusType.Beginning)
                throw new BasicBlankException("No autoresume.  Therefore, had to be beginning");
            LoadControls();
            SaveRoot.LoadMod(_thisMod!);
            _thisMod!.Pile2!.Visible = false; //just in case.
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.FinishedChoosing = false;
                thisPlayer.Knocked = false;
                thisPlayer.FirstChanged = false;
                thisPlayer.SecondChanged = false;
            });
            SaveRoot!.Round++; //i think.
            SaveRoot.GameStatus = EnumStatusType.Beginning;  //i think.
            SaveRoot.LoadMod(_thisMod!); //just in case
            _thisMod!.Pile2!.Visible = false;
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            SingleInfo = PlayerList!.GetSelf();
            if (SaveRoot!.GameStatus != EnumStatusType.Beginning)
                throw new BasicBlankException("Must beginnings at first");
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "knock":
                    await KnockAsync();
                    break;
                case "selectbeginning":
                    SendBeginning thisB = await js.DeserializeObjectAsync<SendBeginning>(content);
                    await SelectBeginningAsync(thisB.Player, thisB.SelectList, thisB.UnsSelectList);
                    break;
                case "changeunknown":
                    await ChangeUnknownAsync(int.Parse(content));
                    break;
                case "changehand":
                    await ChangeHandAsync(int.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
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
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            SingleInfo = PlayerList.GetWhoPlayer();
            if (SingleInfo.Knocked)
            {
                await EndRoundAsync();
                return;
            }
            await StartNewTurnAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.GameStatus == EnumStatusType.Beginning)
            {
                SingleInfo = PlayerList!.GetSelf();
                _thisMod!.NormalTurn = "None";
                _thisMod.Instructions = "Choose the 2 cards to put into your hand";
                _thisMod.Beginnings1!.ClearBoard(SingleInfo.MainHandList);
                if (ThisData!.MultiPlayer) //forgot its true, not false.
                    ThisCheck!.IsEnabled = false;
                await ShowHumanCanPlayAsync();
                return;
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SaveRoot.GameStatus == EnumStatusType.Knocked)
                _thisMod!.Instructions = "Take your last turn because a player knocked";
            else
                _thisMod!.Instructions = "Take turn";
            await base.ContinueTurnAsync();
        }
        private int WhoKnocked
        {
            get
            {
                if (SaveRoot!.GameStatus == EnumStatusType.Normal)
                    return 0;
                GolfCardGamePlayerItem thisPlayer = PlayerList.FirstOrDefault(items => items.Knocked);
                if (thisPlayer == null)
                    return 0;
                return thisPlayer.Id;
            }
        }
        private int WonRound(CustomBasicList<int> thisCol)
        {
            int leasts = 1000;
            int wins = 0;
            for (int x = 1; x <= thisCol.Count; x++)
            {
                int temps = thisCol[x - 1]; // because 0 based
                if (temps == leasts)
                {
                    wins = 0;
                }
                else if (temps < leasts)
                {
                    leasts = temps;
                    wins = x;
                }
            }
            return wins;
        }
        private int ScoreCard(RegularSimpleCard thisCard)
        {
            if (thisCard.CardType == EnumCardTypeList.Joker || thisCard.Value == EnumCardValueList.Jack)
                return 0;
            if (thisCard.Value == EnumCardValueList.Queen || thisCard.Value == EnumCardValueList.King)
                return 10;
            return (int)thisCard.Value;
        }
        private int ScorePlayer()
        {
            int nums = SingleInfo!.MainHandList.Sum(items => ScoreCard(items));
            int temps = SingleInfo.TempSets.Sum(items => ScoreCard(items));
            //var tempList = SingleInfo.MainHandList.ToRegularDeckDict();
            //tempList.AddRange(SingleInfo.TempSets.ToRegularDeckDict());
            return nums + temps;
        }
        private CustomBasicList<int> ListScores()
        {
            CustomBasicList<int> output = new CustomBasicList<int>();
            PlayerList!.ForEach(thisPlayer =>
            {
                SingleInfo = thisPlayer;
                output.Add(ScorePlayer());
            });
            return output;
        }
        private void Scoring(int knocks, int wins, CustomBasicList<int> scoreList)
        {
            int scores;
            for (int x = 1; x <= PlayerList.Count(); x++)
            {
                if (knocks == 0)
                {
                    scores = scoreList[x - 1];
                }
                else if (knocks > 0 && wins == knocks && knocks == x)
                {
                    scores = 0;
                }
                else if (knocks > 0 && wins != knocks && knocks == x)
                {
                    scores = scoreList[x - 1];
                    scores *= 2;
                }
                else
                {
                    scores = scoreList[x - 1];
                }

                var thisPlayer = PlayerList![x];
                thisPlayer.PreviousScore = scores;
                thisPlayer.TotalScore += scores;
            }
        }
        public override async Task EndRoundAsync()
        {
            int knocks = WhoKnocked;
            var scoreList = ListScores();
            int wins = WonRound(scoreList);
            Scoring(knocks, wins, scoreList);
            _thisMod!.HiddenCards1!.RevealCards();
            if (SaveRoot!.Round == 9)
            {
                SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
                _thisMod.Instructions = "Game Over";
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        Task IStartNewGame.ResetAsync()
        {
            SaveRoot!.Round = 0; //i think.
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PreviousScore = 0;
                thisPlayer.TotalScore = 0;
            });
            return Task.CompletedTask;
        }
    }
}