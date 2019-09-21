using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CribbageCP
{
    public class CribbageViewModel : BasicCardGamesVM<CribbageCard, CribbagePlayerItem, CribbageMainGameClass>
    {
        private int _TotalScore;

        public int TotalScore
        {
            get { return _TotalScore; }
            set
            {
                if (SetProperty(ref _TotalScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _TotalCount;
        public int TotalCount
        {
            get { return _TotalCount; }
            set
            {
                if (SetProperty(ref _TotalCount, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _Dealer = "";
        public string Dealer
        {
            get { return _Dealer; }
            set
            {
                if (SetProperty(ref _Dealer, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public CribbageViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return false;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        public ScoreBoardCP? ScoreBoard1;
        public HandViewModel<CribbageCard>? CribFrame;
        public HandViewModel<CribbageCard>? MainFrame;
        public HiddenBoard? GameBoard1;
        internal void LoadControls() //needs playerlist first.
        {
            MainFrame = new HandViewModel<CribbageCard>(this);
            CribFrame = new HandViewModel<CribbageCard>(this);
            MainFrame.Visible = true; //not sure about isenabled (?)
            CribFrame.Visible = false;
            MainFrame.Text = "Card List";
            CribFrame.Text = "Crib";
            MainFrame.SendEnableProcesses(this, () => false);
            CribFrame.SendEnableProcesses(this, () => false);
            if (MainGame!.PlayerList.Count() == 2)
                CribFrame.Maximum = 4;
            else
                CribFrame.Maximum = 3;
            MainFrame.Maximum = 10;
            PlayerHand1!.Maximum = 6;
            ScoreBoard1 = new ScoreBoardCP();
            GameBoard1 = MainContainer!.Resolve<HiddenBoard>();
        }
        public BasicGameCommand? ContinueCommand { get; set; }
        public BasicGameCommand? PlayCommand { get; set; }
        public BasicGameCommand? CribCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = false; //if you want reshuffling, use this.  otherwise, comment or delete.
            Pile1!.Text = "Start Card";
            ContinueCommand = new BasicGameCommand(this, async items =>
            {
                MainGame!.SingleInfo = MainGame.PlayerList!.GetSelf();
                MainGame.SingleInfo.FinishedLooking = true;
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("endview", MainGame.SingleInfo.Id);
                await MainGame.EndViewAsync();
            }, items => MainGame!.SaveRoot!.WhatStatus == EnumGameStatus.GetResultsCrib
                || MainGame.SaveRoot.WhatStatus == EnumGameStatus.GetResultsHand, this, CommandContainer!);
            PlayCommand = new BasicGameCommand(this, async items =>
            {
                var thisList = PlayerHand1!.ListSelectedObjects();
                if (thisList.Count > 1)
                    throw new BasicBlankException("It should have only allowed selecting one at a time now");
                if (thisList.Count == 0)
                {
                    await ShowGameMessageAsync("You must choose a card to play");
                    return;
                }
                if (MainGame!.IsValidMove(thisList.Single()) == false)
                {
                    await ShowGameMessageAsync("You cannot play a card that makes it more than 31 points");
                    return;
                }
                await MainGame!.PlayCardAsync(thisList.Single());
            }, items => MainGame!.SaveRoot!.WhatStatus == EnumGameStatus.PlayCard, this, CommandContainer!);
            CribCommand = new BasicGameCommand(this, async items =>
            {
                var thisList = PlayerHand1!.ListSelectedObjects();
                if (MainGame!.PlayerList.Count() == 3 && thisList.Count > 1)
                    throw new BasicBlankException("It should have only allowed selecting one card at a time because 3 players");
                int maxs;
                if (MainGame.PlayerList.Count() == 3)
                    maxs = 1;
                else
                    maxs = 2;
                if (thisList.Count != maxs)
                {
                    await ShowGameMessageAsync($"Must select {maxs} cards for crib");
                    return;
                }
                MainGame.SingleInfo = MainGame.PlayerList!.GetSelf();
                await MainGame.ProcessCribAsync(thisList);
            }, items => MainGame!.SaveRoot!.WhatStatus == EnumGameStatus.CardsForCrib, this, CommandContainer!);
        }
        internal int PlayerCount => MainGame!.PlayerList.Count();
        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override bool AlwaysEnableHand()
        {
            return false;
        }
        protected override bool CanEnableHand()
        {
            return MainGame!.SaveRoot!.WhatStatus == EnumGameStatus.PlayCard || MainGame.SaveRoot.WhatStatus == EnumGameStatus.CardsForCrib;
        }
    }
}