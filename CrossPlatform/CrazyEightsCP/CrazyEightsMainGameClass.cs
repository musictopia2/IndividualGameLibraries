using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using mm = BasicGameFramework.Extensions.CommonMessageStrings;
namespace CrazyEightsCP
{
    [SingletonGame]
    public class CrazyEightsMainGameClass : CardGameClass<RegularSimpleCard, CrazyEightsPlayerItem, CrazyEightsSaveInfo>, IChoosePieceNM
    {
        private readonly CrazyEightsComputerAI _aI = new CrazyEightsComputerAI();
        public CrazyEightsMainGameClass(IGamePackageResolver container) : base(container) { }

        private CrazyEightsViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<CrazyEightsViewModel>();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            //anything else needed is here.
            SaveRoot!.ThisMod = _thisMod;
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == false)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
                throw new BasicBlankException("The computer player can't go for anybody else on this game");
            if (ThisTest!.ComputerEndsTurn == true)
                throw new BasicBlankException("The computer player was suppposed to end turn.  Rethink");
            await Delay!.DelayMilli(200);
            if (SaveRoot!.ChooseSuit == true)
            {
                EnumSuitList thisSuit = _aI.SuitToChoose(SingleInfo);
                await SuitChosenAsync(thisSuit);
                return;
            }
            await Delay.DelaySeconds(.5);
            int Nums = _aI.CardToPlay(SaveRoot);
            if (Nums > 0)
            {
                await PlayCardAsync(Nums);
                return;
            }
            if (ThisData!.MultiPlayer == true)
                await SendDrawMessageAsync();
            await DrawAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync() //try here instead.
        {
            var tempCard = _thisMod!.Pile1!.GetCardInfo();
            SaveRoot!.CurrentNumber = tempCard.Value;
            if (tempCard.DisplaySuit == EnumSuitList.None)
                SaveRoot.CurrentSuit = tempCard.Suit;
            else
                SaveRoot.CurrentSuit = tempCard.DisplaySuit;
            SaveRoot.CurrentCard = tempCard.Deck; //i think
            SaveRoot!.ThisMod = _thisMod; ; //i think this is needed too.
            SaveRoot.UpdateSuits();
            return Task.CompletedTask;
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.ChooseSuit = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects();
            if (SingleInfo.MainHandList.Count == 0 && PlayerCanWin() == true)
            {
                await ShowWinAsync();
                return; //because game is over now
            }
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync(); //i think here is fine.
        }
        public async Task ChoosePieceReceivedAsync(string data)
        {
            EnumSuitList Suit = await js.DeserializeObjectAsync<EnumSuitList>(data);
            await SuitChosenAsync(Suit);
        }
        public async Task SuitChosenAsync(EnumSuitList chosen)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync(mm.ChosenPiece, chosen);
            SaveRoot!.CurrentSuit = chosen;
            SaveRoot.ChooseSuit = false;
            var thisCard = _thisMod!.Pile1!.CurrentDisplayCard;
            thisCard.DisplaySuit = chosen;
            await EndTurnAsync();
        }
        protected async override Task AfterDrawingAsync()
        {
            AlreadyDrew = false;
            PlayerDraws = 0;
            await base.AfterDrawingAsync();
        }
        public bool IsValidMove(int deck)
        {
            var ThisCard = DeckList!.GetSpecificItem(deck);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self && ThisTest!.AllowAnyMove == true)
                return true;
            if (ThisCard.Value == EnumCardValueList.Eight)
                return true;
            if (ThisCard.Value == SaveRoot!.CurrentNumber)
                return true;
            if (ThisCard.Suit == SaveRoot.CurrentSuit)
                return true;
            return false;
        }
        public async Task PlayCardAsync(int deck)
        {
            var ThisCard = DeckList!.GetSpecificItem(deck);
            ThisCard.Drew = false;
            ThisCard.IsSelected = false; //i think
            await SendDiscardMessageAsync(deck);
            await DiscardAsync(ThisCard);
        }
        public override async Task DiscardAsync(RegularSimpleCard thisCard)
        {
            int firstCount;
            firstCount = SingleInfo!.MainHandList.Count;
            SingleInfo.MainHandList.RemoveSpecificItem(thisCard); //for now, okay.
            int secondCount = SingleInfo.MainHandList.Count;
            if (secondCount + 1 != firstCount)
                throw new BasicBlankException("Warning, second count was " + secondCount + " and first count was " + firstCount + ".  Something is not right");
            await AnimatePlayAsync(thisCard);

            SaveRoot!.CurrentNumber = thisCard.Value;
            if (SingleInfo.ObjectCount != SingleInfo.MainHandList.Count)
                throw new BasicBlankException("Failed to update count.  Rethink");
            if (thisCard.Value == EnumCardValueList.Eight && SingleInfo.MainHandList.Count > 0)
            {
                SaveRoot.ChooseSuit = true;
                await ContinueTurnAsync();
                return;
            }
            SaveRoot.CurrentSuit = thisCard.Suit;
            var TempCard = _thisMod!.Pile1!.GetCardInfo();
            if (TempCard.Deck != thisCard.Deck)
                throw new BasicBlankException("Failed To Add To Deck");
            await EndTurnAsync();
        }
    }

    //most did not even use this one. FinishStartAsync
}