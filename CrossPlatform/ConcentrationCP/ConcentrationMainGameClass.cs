using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConcentrationCP
{
    [SingletonGame]
    public class ConcentrationMainGameClass : CardGameClass<RegularSimpleCard, ConcentrationPlayerItem, ConcentrationSaveInfo>, IMiscDataNM
    {
        public ConcentrationMainGameClass(IGamePackageResolver container) : base(container) { }
        internal ConcentrationViewModel? ThisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<ConcentrationViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            ThisMod!.GameBoard1!.PileList = SaveRoot!.BoardList.ToCustomBasicList();
            return base.FinishGetSavedAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            var thisCol = ThisMod!.GameBoard1!.GetSelectedCards();
            if (thisCol.Count == 2)
            {
                await SaveStateAsync(); //so you can't cheat.
                await ProcessPlayAsync(thisCol);
                return;
            }
            await base.ContinueTurnAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        private async Task ProcessPlayAsync(DeckRegularDict<RegularSimpleCard> thisCol)
        {
            if (IsValidMove(thisCol) == true)
            {
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                RemoveComputer(thisCol);
                ThisMod!.GameBoard1!.SelectedCardsGone();
                SingleInfo!.Pairs++;
                if (ThisMod.GameBoard1.CardsGone == true)
                {
                    await GameOverAsync();
                    return;
                }
                await ContinueTurnAsync();
                return;
            }
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(5);
            ThisMod!.GameBoard1!.UnselectCards();
            AddComputer(thisCol);
            await EndTurnAsync();
        }
        private bool IsValidMove(DeckRegularDict<RegularSimpleCard> thisCol)
        {
            if (ThisTest!.AllowAnyMove == true && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                return true; //for testing.
            return thisCol.HasDuplicates(items => items.Value); //hopefully this simple now.
        }
        private void RemoveComputer(DeckRegularDict<RegularSimpleCard> ThisCol)
        {
            if (PlayerList.Any(items => items.PlayerCategory == EnumPlayerCategory.Computer) == false)
                return;
            ThisCol.ForEach(thisCard =>
            {
                if (SaveRoot!.ComputerList.ObjectExist(thisCard.Deck) == true)
                    SaveRoot.ComputerList.RemoveObjectByDeck(thisCard.Deck);
            });
        }
        private void AddComputer(DeckRegularDict<RegularSimpleCard> thisCol)
        {
            if (PlayerList.Any(items => items.PlayerCategory == EnumPlayerCategory.Computer) == false)
                return;
            thisCol.ForEach(thisCard =>
            {
                if (SaveRoot!.ComputerList.ObjectExist(thisCard.Deck) == false)
                    SaveRoot.ComputerList.Add(thisCard);
            });
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            await SelectCardAsync(ComputerAI.CardToTry(this));
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(thisPlayer => thisPlayer.Pairs = 0);
            SaveRoot!.ComputerList = new DeckRegularDict<RegularSimpleCard>();
            ThisMod!.GameBoard1!.ClearBoard();
            return base.StartSetUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "selectcard":
                    await SelectCardAsync(int.Parse(content));
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
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync(); //try this too.
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.Pairs).First();
            await ShowWinAsync();
        }
        public override async Task PopulateSaveRootAsync()
        {
            SaveRoot!.BoardList = ThisMod!.GameBoard1!.PileList.ToCustomBasicList();
            await base.PopulateSaveRootAsync();
        }
        internal async Task SelectCardAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("selectcard", deck);
            ThisMod!.GameBoard1!.SelectCard(deck);
            await ContinueTurnAsync(); //this will handle the rest.
        }
    }
}