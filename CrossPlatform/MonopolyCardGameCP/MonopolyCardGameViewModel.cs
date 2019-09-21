using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MonopolyCardGameCP
{
    public class MonopolyCardGameViewModel : BasicCardGamesVM<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem, MonopolyCardGameMainGameClass>
    {
        public MonopolyCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumWhatStatus.DrawOrTrade || MainGame.SaveRoot.GameStatus == EnumWhatStatus.Either;
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
        public override bool CanEnableAlways()
        {
            return true;
        }
        public DetailCardViewModel? AdditionalInfo1;
        public BasicGameCommand? GoOutCommand { get; set; }
        public BasicGameCommand? ResumeCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            GoOutCommand = new BasicGameCommand(this, async items =>
            {
                if (MainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                    throw new BasicBlankException("Not Self.  Rethink");
                var newList = MainGame.SingleInfo.MainHandList.ToRegularDeckDict();
                if (MainGame.CanGoOut(newList) == false)
                {
                    await ShowGameMessageAsync("Sorry, you cannot go out");
                    return;
                }
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("goout");
                await MainGame.ProcessGoingOutAsync();
            }, items => CanEnableDeck(), this, CommandContainer!);
            ResumeCommand = new BasicGameCommand(this, async items => await MainGame!.EndTurnAsync(), items => MainGame!.SaveRoot!.GameStatus == EnumWhatStatus.LookOnly, this, CommandContainer!);
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            AdditionalInfo1 = new DetailCardViewModel(); //hopefully can do this too.
        }
        protected override Task OnConsiderSelectOneCardAsync(MonopolyCardGameCardInformation payLoad)
        {
            if (payLoad.Deck == AdditionalInfo1!.CurrentCard.Deck)
                AdditionalInfo1.Clear();
            else
                AdditionalInfo1.AdditionalInfo(payLoad.Deck);
            return Task.CompletedTask;
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return;
            MainGame!.PlayerList!.ForEach(thisPlayer =>
            {
                if (MainGame.SaveRoot!.GameStatus == EnumWhatStatus.LookOnly)
                    thisPlayer!.TradePile!.IsEnabled = false;
                else if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
                    thisPlayer.TradePile!.IsEnabled = true;
                else
                    thisPlayer.TradePile!.IsEnabled = MainGame.SaveRoot.GameStatus == EnumWhatStatus.Discard || MainGame.SingleInfo!.MainHandList.Count == 9;
            });
        }
    }
}