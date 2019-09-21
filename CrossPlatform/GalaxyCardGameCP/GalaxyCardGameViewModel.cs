using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GalaxyCardGameCP
{
    public class GalaxyCardGameViewModel : TrickGamesVM<EnumSuitList, GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem, GalaxyCardGameMainGameClass>
    {
        public INewWinCard? WinUI; //do the views can implement and call it.
        public GalaxyCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }
        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            await Task.CompletedTask;
        }
        public override bool CanEndTurn()
        {
            return MainGame!.CanEndTurn();
        }
        public BasicGameCommand? MoonCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            PlayerHand1!.Maximum = 9;
            MoonCommand = new BasicGameCommand(this, async items =>
            {
                var thisList = PlayerHand1.ListSelectedObjects();
                if (MainGame!.HasValidMoon(thisList) == false)
                {
                    await ShowGameMessageAsync("Invalid Moon");
                    return;
                }
                if (ThisData!.MultiPlayer)
                {
                    var tempList = thisList.GetDeckListFromObjectList();
                    await ThisNet!.SendAllAsync("newmoon", tempList);
                }
                await MainGame.PlayNewMoonAsync(thisList);
            }, items => MainGame!.SingleInfo!.CanEnableMoon(), this, CommandContainer!);
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (MainGame!.SaveRoot!.GameStatus == EnumGameStatus.PlaceSets)
                PlayerHand1!.AutoSelect = HandViewModel<GalaxyCardGameCardInformation>.EnumAutoType.SelectAsMany;
            else
                PlayerHand1!.AutoSelect = HandViewModel<GalaxyCardGameCardInformation>.EnumAutoType.SelectOneOnly; //try this way.
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}