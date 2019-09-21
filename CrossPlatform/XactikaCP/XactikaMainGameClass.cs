using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.BasicDrawables.Dictionary;

namespace XactikaCP
{
    [SingletonGame]
    public class XactikaMainGameClass : TrickGameClass<EnumShapes, XactikaCardInformation,
        XactikaPlayerItem, XactikaSaveInfo>, IMiscDataNM, IStartNewGame
    {
        private IAdvancedTrickProcesses? _aTrick;
        public XactikaMainGameClass(IGamePackageResolver container) : base(container) { }
        private bool ShowedOnce = false;
        internal XactikaViewModel? ThisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
			base.Init();
            ThisMod = MainContainer.Resolve<XactikaViewModel>();
        }
        protected override bool CanEnableTrickAreas => SaveRoot!.GameStatus == EnumStatusList.Normal;
        public new SeveralPlayersTrickViewModel<EnumShapes, XactikaCardInformation, XactikaPlayerItem, XactikaSaveInfo>? TrickArea1;
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            ShowedOnce = false;
            SaveRoot!.LoadMod(ThisMod!); //hopefully does not have to clear out the cardlisis.
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadTrickAreas();
            _aTrick = MainContainer.Resolve<IAdvancedTrickProcesses>(); //decided to be here.
            TrickArea1 = MainContainer.Resolve<SeveralPlayersTrickViewModel<EnumShapes, XactikaCardInformation, XactikaPlayerItem, XactikaSaveInfo>>();
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (SaveRoot!.GameStatus == EnumStatusList.ChooseGameType)
                return; //hopefully this simple.
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SaveRoot.GameStatus == EnumStatusList.Bidding)
            {
                int bids = ComputerAI.HowManyToBid(this);
                ThisMod!.BidChosen = bids;
                await ProcessBidAsync();
                return;
            }
            if (SaveRoot.GameStatus == EnumStatusList.CallShape)
            {
                await ShapeChosenAsync(ComputerAI.GetShapeChosen());
                return;
            }
            await PlayCardAsync(ComputerAI.CardToPlay(this));
        }
        public override async Task ContinueTrickAsync()
        {
            if (SaveRoot!.ShapeChosen == EnumShapes.None)
            {
                SaveRoot.GameStatus = EnumStatusList.CallShape;
                await FirstCallShapeAsync();
                return;
            }
            await base.ContinueTrickAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            LoadVM();
            ShowedOnce = false; //try this way.
            if (isBeginning)
            {
                SaveRoot!.GameStatus = EnumStatusList.ChooseGameType;
                SaveRoot.GameMode = EnumGameMode.None;
                SaveRoot.RoundNumber = 1;
                SaveRoot.LoadMod(ThisMod!);
            }
            else if (SaveRoot!.GameMode == EnumGameMode.ToBid)
                SaveRoot.GameStatus = EnumStatusList.Bidding;
            else if (SaveRoot.GameMode != EnumGameMode.None)
            {
                SaveRoot.GameStatus = EnumStatusList.Normal; //try this way
                SaveRoot.ClearTricks = true;
            }
            SaveRoot.ShapeChosen = EnumShapes.None;
            SaveRoot.Value = 0;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TricksWon = 0;
                thisPlayer.BidAmount = -1;
            });
            return base.StartSetUpAsync(isBeginning);
        }
        public override async Task ContinueTurnAsync()
        {
            if (ShowedOnce == false)
            {
                await BeginningProcessesAsync();
                return;
            }
            await base.ContinueTurnAsync();
        }
        private async Task BeginningProcessesAsync()
        {
            ShowedOnce = true;
            if (SaveRoot!.GameMode == EnumGameMode.None)
            {
                await EnableOptionsAsync();
                return;
            }
            await ProcessGameOptionChosenAsync(SaveRoot.GameMode, false); //otherwise, does it every round.  we don't want that.
        }
        private async Task EnableOptionsAsync()
        {
            await SaveStateAsync();
            ThisMod!.MainOptionsVisible = false; //just in case.
            if (ThisData!.MultiPlayer)
            {
                if (ThisData.Client)
                {
                    ThisCheck!.IsEnabled = true; //has to wait for host to choose game option.
                    return;
                }
            }
            await ShowHumanCanPlayAsync(); //hopefully this simple.
        }
        public async Task ProcessGameOptionChosenAsync(EnumGameMode optionChosen, bool doShow)
        {
            if (doShow)
            {
                ThisMod!.ModeChoose1!.SelectSpecificItem((int)optionChosen);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.5);
                SaveRoot!.GameMode = optionChosen;
            }
            else if (optionChosen != SaveRoot!.GameMode)
            {
                throw new BasicBlankException("Had to show");
            }
            if (SaveRoot.GameStatus != EnumStatusList.ChooseGameType)
            {
                if (SaveRoot.GameStatus == EnumStatusList.Bidding)
                {
                    TrickArea1!.Visible = false; //just in case.
                    await BeginBiddingAsync();
                    return;
                }
                if (SaveRoot.GameStatus == EnumStatusList.CallShape)
                {
                    ThisMod!.ShapeChoose1!.Visible = true;//just in case.
                    await FirstCallShapeAsync();
                    return;
                }
                if (SaveRoot.ClearTricks)
                {
                    SaveRoot.ClearTricks = false;
                    await EndBidAsync();
                    return; //try this way.
                }
                if (SaveRoot.ShapeChosen != EnumShapes.None)
                    ThisMod!.ShapeChoose1!.ChoosePiece(SaveRoot.ShapeChosen); //hopefully this simple.
                SingleInfo = PlayerList!.GetWhoPlayer();
                this.ShowTurn();
                await ContinueTurnAsync();
                return;
            }
            if (optionChosen == EnumGameMode.ToBid)
            {
                SingleInfo = PlayerList!.GetWhoPlayer();
                SaveRoot.GameStatus = EnumStatusList.Bidding;
                await PopulateBidAmountsAsync();
                return;
            }
            await EndBidAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "bid":
                    ThisMod!.BidChosen = int.Parse(content);
                    await ProcessBidAsync();
                    return;
                case "shapeused":
                    EnumShapes thisShape = await js.DeserializeObjectAsync<EnumShapes>(content);
                    await ShapeChosenAsync(thisShape);
                    return;
                case "gameoptionchosen":
                    EnumGameMode thisMode = await js.DeserializeObjectAsync<EnumGameMode>(content);
                    await ProcessGameOptionChosenAsync(thisMode, true);
                    return;
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
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await ContinueTurnAsync();
        }
        private int WhoWonTrick(DeckObservableDict<XactikaCardInformation> thisCol)
        {
            var thisCard = SaveRoot!.ShapeChosen switch
            {
                EnumShapes.Balls => thisCol.Where(items => items.HowManyBalls == SaveRoot.Value).OrderByDescending(items => items.Value).ThenByDescending(items => items.OrderPlayed).First(),
                EnumShapes.Cubes => thisCol.Where(items => items.HowManyCubes == SaveRoot.Value).OrderByDescending(items => items.Value).ThenByDescending(items => items.OrderPlayed).First(),
                EnumShapes.Cones => thisCol.Where(items => items.HowManyCones == SaveRoot.Value).OrderByDescending(items => items.Value).ThenByDescending(items => items.OrderPlayed).First(),
                EnumShapes.Stars => thisCol.Where(items => items.HowManyStars == SaveRoot.Value).OrderByDescending(items => items.Value).ThenByDescending(items => items.OrderPlayed).First(),
                _ => throw new BasicBlankException("Cannot figure out what the card should be"),
            };
            return thisCard.Player;
        }
        public override bool CanMakeMainOptionsVisibleAtBeginning => false;
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            XactikaPlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon++;
            await _aTrick!.AnimateWinAsync(wins);
            if (SingleInfo!.MainHandList.Count == 0)
            {
                await EndRoundAsync();
                return; //most of the time its in rounds.
            }
            ThisMod!.PlayerHand1!.EndTurn();
            WhoTurn = wins; //most of the time, whoever wins leads again.
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            ThisMod!.ShapeChoose1!.Reset();
            SaveRoot!.ShapeChosen = EnumShapes.None;
            SaveRoot.Value = 0;
            await StartNewTurnAsync(); //hopefully this simple.
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.CurrentScore = 0;
            });
            SaveRoot!.GameStatus = EnumStatusList.ChooseGameType; //i think back to that again.
            SaveRoot.RoundNumber = 1;
            SaveRoot.GameMode = EnumGameMode.None;
            return Task.CompletedTask;
        }
        public override bool IsValidMove(int deck)
        {
            if (SaveRoot!.ShapeChosen == EnumShapes.None)
                return true; //at first, can play anything.
            var thisCard = DeckList!.GetSpecificItem(deck);
            if (SaveRoot.ShapeChosen == EnumShapes.Balls)
            {
                if (thisCard.HowManyBalls == SaveRoot.Value)
                    return true;
                return !SingleInfo!.MainHandList.Any(items => items.HowManyBalls == SaveRoot.Value);
            }
            if (SaveRoot.ShapeChosen == EnumShapes.Cones)
            {
                if (thisCard.HowManyCones == SaveRoot.Value)
                    return true;
                return !SingleInfo!.MainHandList.Any(items => items.HowManyCones == SaveRoot.Value);
            }
            if (SaveRoot.ShapeChosen == EnumShapes.Cubes)
            {
                if (thisCard.HowManyCubes == SaveRoot.Value)
                    return true;
                return !SingleInfo!.MainHandList.Any(items => items.HowManyCubes == SaveRoot.Value);
            }
            if (SaveRoot.ShapeChosen == EnumShapes.Stars)
            {
                if (thisCard.HowManyStars == SaveRoot.Value)
                    return true;
                return !SingleInfo!.MainHandList.Any(items => items.HowManyStars == SaveRoot.Value);
            }
            throw new BasicBlankException("Cannot figure out whether the card can be played or not");
        }
        private async Task BeginBiddingAsync()
        {
            await PopulateBidAmountsAsync();
            await StartNewTurnAsync();
        }
        public async Task FirstCallShapeAsync()
        {
            var thisCard = SaveRoot!.TrickList.Single();
            ThisMod!.ShapeChoose1!.LoadPieceList(thisCard);
            await ContinueTurnAsync();
        }
        public async Task ShapeChosenAsync(EnumShapes shape)
        {
            var thisCard = SaveRoot!.TrickList.Single();
            SaveRoot.ShapeChosen = shape;
            SaveRoot.Value = shape switch
            {
                EnumShapes.Balls => thisCard.HowManyBalls,
                EnumShapes.Cubes => thisCard.HowManyCubes,
                EnumShapes.Cones => thisCard.HowManyCones,
                EnumShapes.Stars => thisCard.HowManyStars,
                _ => throw new BasicBlankException("Don't know what to use"),
            };
            SaveRoot.GameStatus = EnumStatusList.Normal;
            ThisMod!.ShapeChoose1!.ChoosePiece(shape);
            await EndTurnAsync();
        }
        private async Task PopulateBidAmountsAsync()
        {
            var nextPlayer = await PlayerList!.CalculateWhoTurnAsync(true);
            int nonNumber = -1;
            if (nextPlayer == WhoStarts)
            {
                var total = PlayerList.Where(items => items.BidAmount > -1).Sum(items => items.BidAmount);
                if (total > 8)
                    nonNumber = -1;
                else
                    nonNumber = 8 - total;
            }
            int x;
            CustomBasicList<int> tempList = new CustomBasicList<int>();
            for (x = 0; x <= 8; x++)
            {
                if (x != nonNumber)
                    tempList.Add(x);
            }
            ThisMod!.Bid1!.UnselectAll();
            ThisMod.BidChosen = -1;
            ThisMod.Bid1.LoadNumberList(tempList);
            await ContinueTurnAsync();
        }
        public async Task ProcessBidAsync()
        {
            int bidAmount = ThisMod!.BidChosen;
            SingleInfo!.BidAmount = bidAmount;
            ThisMod!.Bid1!.SelectNumberValue(bidAmount);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            ThisMod.Bid1.UnselectAll();
            ThisMod.BidChosen = -1;
            await ContinueBidProcessAsync();
        }
        private async Task ContinueBidProcessAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            if (WhoTurn == WhoStarts)
            {
                await EndBidAsync();
                return;
            }
            SingleInfo = PlayerList.GetWhoPlayer();
            await BeginBiddingAsync();
        }
        private async Task EndBidAsync()
        {
            SaveRoot!.GameStatus = EnumStatusList.Normal;
            await StartNewTrickAsync(); //not sure about the turn proceeses (?)
        }
        private int CalculateScore(int bidAmount, int amountWon)
        {
            if (bidAmount == amountWon)
                return bidAmount;
            if (bidAmount > amountWon)
                return amountWon - bidAmount;
            else
                return bidAmount - amountWon;
        }
        public override async Task EndRoundAsync()
        {
            if (SaveRoot!.GameMode == EnumGameMode.ToBid)
            {
                PlayerList!.ForEach(thisPlayer =>
                {
                    var bidAmount = thisPlayer.BidAmount;
                    var amountWon = thisPlayer.TricksWon;
                    thisPlayer.CurrentScore = CalculateScore(bidAmount, amountWon);
                    thisPlayer.TotalScore += thisPlayer.CurrentScore;
                });
                if (SaveRoot.RoundNumber == 8)
                {
                    SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
                    await ShowWinAsync();
                    return;
                }
                SaveRoot.RoundNumber++;
                this.RoundOverNext();
                return;
            }
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentScore = thisPlayer.TricksWon;
                thisPlayer.TotalScore += thisPlayer.CurrentScore;
            });
            if (SaveRoot.RoundNumber == 8)
            {
                if (SaveRoot.GameMode == EnumGameMode.ToWin)
                    SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
                else
                    SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
                await ShowWinAsync();
                return;
            }
            SaveRoot.RoundNumber++;
            this.RoundOverNext();
        }
    }

    //most did not even use this one. FinishStartAsync
}