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
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace RackoCP
{
    [SingletonGame]
    public class RackoMainGameClass : CardGameClass<RackoCardInformation, RackoPlayerItem, RackoSaveInfo>, IMiscDataNM, IStartNewGame
    {
        public RackoMainGameClass(IGamePackageResolver container) : base(container) { }

        public RackoViewModel? ThisMod;
        private int PreviousUse; //for computer ai.
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<RackoViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
                    thisPlayer.CanShowValues = true;
                else
                    thisPlayer.CanShowValues = false;
            });
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            if (HasRacko() == true)
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("racko");
                await EndRoundAsync();
                return;
            }
            int Nums;
            if (AlreadyDrew == false)
            {
                Nums = ComputerAI.PickUp(this);
                if (Nums > 0)
                {
                    PreviousUse = Nums;
                    await PickupFromDiscardAsync();
                    return;
                }
                await DrawAsync();
                return;
            }
            RackoCardInformation thisCard;
            if (PreviousUse > 0)
            {
                thisCard = SingleInfo!.MainHandList[PreviousUse - 1]; //because 0 based.
                thisCard.WillKeep = true;
                if (ThisData!.MultiPlayer == true)
                    await SendDiscardMessageAsync(thisCard.Deck);
                await DiscardAsync(thisCard.Deck);
                return;
            }
            Nums = ComputerAI.CardToPlay(this);
            if (Nums > 0)
            {
                thisCard = SingleInfo!.MainHandList[Nums - 1];
                thisCard.WillKeep = true;
                if (ThisData!.MultiPlayer == true)
                    await SendDiscardMessageAsync(thisCard.Deck);
                await DiscardAsync(thisCard.Deck);
                return;
            }
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("discardcurrent");
            await DiscardCurrentAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
                    thisPlayer.CanShowValues = true;
                else
                    thisPlayer.CanShowValues = false;
            });
            return base.StartSetUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "racko":
                    await EndRoundAsync();
                    return;
                case "discardcurrent":
                    await DiscardCurrentAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            PreviousUse = 0;
            AlreadyDrew = false; //maybe has to be here.
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public override async Task EndRoundAsync()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.CanShowValues = true);
            PlayerList.ForEach(thisPlayer =>
            {
                thisPlayer.ScoreRound = WhatScore(thisPlayer);
                if (thisPlayer.ScoreRound == 0)
                    throw new BasicBlankException("Cannot be 0 points");
                thisPlayer.TotalScore += thisPlayer.ScoreRound;
            });
            PossibleWinProcess();
            if (SingleInfo != null)
            {
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        private void PossibleWinProcess()
        {
            SingleInfo = PlayerList.Where(items => items.TotalScore >= 300).FirstOrDefault();
        }
        private int HowManyHigher(RackoPlayerItem thisPlayer)
        {
            int x = 0;
            int thisNum = 0;
            foreach (var thisCard in thisPlayer.MainHandList)
            {
                if (thisCard.Value > thisNum)
                {
                    x++;
                    thisNum = thisCard.Value;
                }
                else
                    return x;
            }
            return x;
        }
        private int WhatScore(RackoPlayerItem thisPlayer)
        {
            if (thisPlayer.Id != WhoTurn)
                return HowManyHigher(thisPlayer) * 5;
            return 75 + BonusPoints(thisPlayer);
        }
        private int BonusPoints(RackoPlayerItem thisPlayer)
        {
            RummyProcesses<EnumSuitList, EnumSuitList, RackoCardInformation> thisInfo = new RummyProcesses<EnumSuitList, EnumSuitList, RackoCardInformation>();
            thisInfo.HasSecond = false;
            thisInfo.HasWild = false;
            thisInfo.NeedMatch = false; //try this too.
            thisInfo.LowNumber = 1;
            RackoDeckCount temps = MainContainer.Resolve<RackoDeckCount>();
            thisInfo.HighNumber = temps.GetDeckCount();
            DeckRegularDict<RackoCardInformation> tempCol = thisPlayer.MainHandList.ToRegularDeckDict();
            var newColls = thisInfo.WhatNewRummy(tempCol, 6, RummyProcesses<EnumSuitList, EnumSuitList, RackoCardInformation>.EnumRummyType.Runs, true);
            if (newColls.Count > 0)
                return 400;
            newColls = thisInfo.WhatNewRummy(tempCol, 5, RummyProcesses<EnumSuitList, EnumSuitList, RackoCardInformation>.EnumRummyType.Runs, true);
            if (newColls.Count > 0)
                return 200;
            newColls = thisInfo.WhatNewRummy(tempCol, 4, RummyProcesses<EnumSuitList, EnumSuitList, RackoCardInformation>.EnumRummyType.Runs, true);
            if (newColls.Count > 0)
                return 100;
            newColls = thisInfo.WhatNewRummy(tempCol, 3, RummyProcesses<EnumSuitList, EnumSuitList, RackoCardInformation>.EnumRummyType.Runs, true);
            if (newColls.Count > 0)
                return 50;
            return 0;
        }
        public override async Task DiscardAsync(RackoCardInformation thisCard)
        {
            var newCard = ThisMod!.Pile2!.GetCardInfo();
            newCard.WillKeep = thisCard.WillKeep;

            if (thisCard.Deck == newCard.Deck)
                throw new BasicBlankException("Cannot be duplicate for discard.  Rethink");
            if (SingleInfo!.MainHandList.Contains(thisCard) == true)
                thisCard = SingleInfo.MainHandList.GetSpecificItem(thisCard.Deck);
            else
                throw new BasicBlankException("Rethink for now");
            SingleInfo.MainHandList.ReplaceItem(thisCard, newCard);
            if (SingleInfo.MainHandList.ObjectExist(thisCard.Deck))
                throw new BasicBlankException("Failed To Replace Card");
            ThisMod.Pile2.ClearCards();
            await AnimatePlayAsync(thisCard); //has to do this way to animate the plays.
            await EndTurnAsync();
        }
        public bool HasRacko()
        {
            int thisNum = 0;
            foreach (var thisCard in SingleInfo!.MainHandList)
            {
                if (thisCard.Value < thisNum)
                    return false;
                thisNum = thisCard.Value;
            }
            return true;
        }
        public async Task DiscardCurrentAsync()
        {
            var thisCard = ThisMod!.Pile2!.GetCardInfo();
            ThisMod.Pile2.ClearCards();
            await AnimatePlayAsync(thisCard);
            await EndTurnAsync();
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.ScoreRound = 0;
                thisPlayer.TotalScore = 0;
            });
            return Task.CompletedTask;
        }
    }
}