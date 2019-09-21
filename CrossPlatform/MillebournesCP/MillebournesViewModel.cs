using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MillebournesCP
{
    public class MillebournesViewModel : BasicCardGamesVM<MillebournesCardInformation, MillebournesPlayerItem, MillebournesMainGameClass>
    {
        public CustomStopWatchCP? Stops;
        private bool _CoupeVisible;
        public bool CoupeVisible
        {
            get { return _CoupeVisible; }
            set
            {
                if (SetProperty(ref _CoupeVisible, value))
                {
                    OnPropertyChanged(nameof(OthersVisible));
                }
            }
        }
        public bool OthersVisible => !CoupeVisible;
        public MillebournesViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return Pile1!.Visible;
        }
        protected override Task ProcessDiscardClickedAsync()
        {
            if (Pile1!.PileEmpty())
                throw new BasicBlankException("Since there is no current card, should have never been enabled");
            PlayerHand1!.UnselectAllObjects();
            if (Pile1.CardSelected() == 0)
                Pile1.IsSelected(true);
            else
                Pile1.IsSelected(false);
            CommandContainer!.ManuelFinish = false;
            CommandContainer.IsExecuting = false;
            return Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public BasicGameCommand? CoupeCommand { get; set; }
        public PileViewModel<MillebournesCardInformation>? Pile2;
        private async Task ProcessCoupeAsync()
        {
            Stops!.PauseTimer();
            MainGame!.SingleInfo = MainGame.PlayerList!.GetSelf();
            MainGame.CurrentCP = MainGame.FindTeam(MainGame.SingleInfo.Team);
            bool rets = MainGame.HasCoupe(out int newDeck);
            SendPlay thisSend;
            if (rets == false)
            {
                await ShowGameMessageAsync("No Coup Foures Here");
                if (ThisData!.MultiPlayer == true)
                {
                    thisSend = new SendPlay();
                    thisSend.Player = MainGame.SingleInfo.Id;
                    thisSend.Team = MainGame.SingleInfo.Team;
                    await ThisNet!.SendAllAsync("nocoupe", thisSend);
                }
                MainGame.CurrentCP.IncreaseWrongs();
                MainGame.UpdateGrid(MainGame.SingleInfo.Team);
                if (ThisData.MultiPlayer == false)
                {
                    await MainGame.EndPartAsync(false);
                    return;
                }
                await MainGame.EndCoupeAsync(MainGame.SingleInfo.Id);
                return;
            }
            if (ThisData!.MultiPlayer == false)
            {
                await MainGame.ProcessCoupeAsync(newDeck, MainGame.SingleInfo.Id);
                return;
            }
            thisSend = new SendPlay();
            thisSend.Player = MainGame.SingleInfo.Id;
            thisSend.Deck = newDeck; //i guess that team may not have mattered.
            await ThisNet!.SendAllAsync("hascoupe", thisSend);
            MainGame.CurrentCoupe.Player = MainGame.SingleInfo.Id;
            MainGame.CurrentCoupe.Card = newDeck;
            await MainGame.EndCoupeAsync(MainGame.SingleInfo.Id);
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = false; //if you want reshuffling, use this.  otherwise, comment or delete.
            CoupeCommand = new BasicGameCommand(this, async items =>
            {
                CoupeVisible = false; //here too.
                await ProcessCoupeAsync();
            }, items => CoupeVisible, this, CommandContainer!);
            PlayerHand1!.AutoSelect = BasicGameFramework.DrawableListsViewModels.HandViewModel<MillebournesCardInformation>.EnumAutoType.None;
            PlayerHand1.Maximum = 6;
            Stops = new CustomStopWatchCP();
            Stops.MaxTime = 3000;
            Stops.TimeUp += Stops_TimeUp;
            Pile2 = new PileViewModel<MillebournesCardInformation>(ThisE!, this);
            Pile1!.Text = "New Card";
            Pile1.Visible = false; //i think
            Pile2.Text = "Throw Away";
            MainGame!.OtherPile = Pile1;
            Pile2.CurrentOnly = true;
            Pile2.FirstLoad(new MillebournesCardInformation());
            Pile2.Visible = true;
            Pile2.PileClickedAsync += Pile2_PileClickedAsync;
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting || Pile1!.PileEmpty())
            {
                Pile1!.Visible = false;
            }
            else
                Pile1.Visible = true;
            Pile1.ReportCanExecuteChange(); //try this.
            MainGame!.TeamList.ForEach(thisTeam =>
            {
                thisTeam.EnableChange();
            });
        }
        private async Task Pile2_PileClickedAsync()
        {
            int deck = MainGame!.FindDeck;
            if (deck == 0)
            {
                await ShowGameMessageAsync("Sorry, needs to select a card to throw away");
                return;
            }
            await MainGame!.ThrowawayCardAsync(deck);
        }
        protected override async Task ProcessHandClickedAsync(MillebournesCardInformation thisCard, int index)
        {
            if (thisCard.IsSelected == true)
            {
                thisCard.IsSelected = false;
                return;
            }
            if (Pile1!.CardSelected() > 0)
            {
                Pile1.IsSelected(false);
                thisCard.IsSelected = true;
                return;
            }
            PlayerHand1!.UnselectAllObjects();
            thisCard.IsSelected = true;
            await Task.CompletedTask;
        }
        private async void Stops_TimeUp()
        {
            CoupeVisible = false;
            CommandContainer!.IsExecuting = true;
            CommandContainer.ManuelFinish = true;
            if (ThisData!.MultiPlayer == false)
            {
                await MainGame!.EndPartAsync(false);
                return;
            }
            MillebournesPlayerItem thisPlayer = MainGame!.PlayerList!.GetSelf();
            await ThisNet!.SendAllAsync("timeup", thisPlayer.Id);
            await MainGame.EndCoupeAsync(thisPlayer.Id);
        }
        internal async Task ProcessTeamClick(EnumPileType pileType, int team)
        {
            int newDeck = MainGame!.FindDeck;
            if (pileType == EnumPileType.None)
                throw new BasicBlankException("Must have a pile that was clicked on");
            if (team == 0)
                throw new BasicBlankException("No team sent when clicking a pile");
            if (newDeck == 0)
            {
                await ShowGameMessageAsync("Sorry, you must select a card first");
                return;
            }
            var thisTeam = MainGame!.FindTeam(team);
            thisTeam.CurrentCard = MainGame.DeckList!.GetSpecificItem(newDeck);
            string message;
            if (pileType == EnumPileType.Miles)
            {
                if (team != MainGame.SaveRoot!.CurrentTeam)
                    throw new BasicBlankException($"The miles should have been disabled since you cannot play miles for another team.  The team clicked was {team} .  However, the current team is {MainGame.SaveRoot.CurrentTeam}");
                if (thisTeam.CanPlaceMiles(out message) == false)
                {
                    await ShowGameMessageAsync(message);
                    PlayerHand1!.UnselectAllObjects();
                    return;
                }
            }
            if (pileType == EnumPileType.Hazard)
            {
                if (MainGame.SaveRoot!.CurrentTeam == team)
                {
                    if (thisTeam.CanFixHazard(out message) == false)
                    {
                        await ShowGameMessageAsync(message);
                        PlayerHand1!.UnselectAllObjects();
                        return;
                    }
                }
                else if (MainGame.SaveRoot.CurrentTeam != team && thisTeam.CanGiveHazard(out message) == false)
                {
                    await ShowGameMessageAsync(message);
                    PlayerHand1!.UnselectAllObjects();
                    return;
                }
            }
            if (pileType == EnumPileType.Speed)
            {
                if (MainGame.SaveRoot!.CurrentTeam == team)
                {
                    if (thisTeam.CanEndSpeedLimit(out message) == false)
                    {
                        await ShowGameMessageAsync(message);
                        PlayerHand1!.UnselectAllObjects();
                        return;
                    }
                }
                else if (MainGame.SaveRoot.CurrentTeam != team && thisTeam.CanGiveSpeedLimit(out message) == false)
                {
                    await ShowGameMessageAsync(message);
                    PlayerHand1!.UnselectAllObjects();
                    return;
                }
            }
            if (pileType == EnumPileType.Safety)
            {
                if (MainGame.SaveRoot!.CurrentTeam != team)
                    throw new BasicBlankException("Cannot place a safety for another team.  Therefore, this should have been disabled");
                if (thisTeam.CanPlaceSafety(out message) == false)
                {
                    await ShowGameMessageAsync(message);
                    PlayerHand1!.UnselectAllObjects();
                    return;
                }
            }
            if (ThisData!.MultiPlayer == true)
            {
                SendPlay thisSend = new SendPlay();
                thisSend.Deck = newDeck;
                thisSend.Pile = pileType;
                thisSend.Team = team;
                await ThisNet!.SendAllAsync("regularplay", thisSend);
            }
            await MainGame.PlayAsync(newDeck, pileType, team, false);
        }
    }
}