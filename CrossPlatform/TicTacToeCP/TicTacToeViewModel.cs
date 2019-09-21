using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace TicTacToeCP
{
    public class TicTacToeViewModel : BasicMultiplayerVM<TicTacToePlayerItem, TicTacToeMainGameClass>
    {
        public TicTacToeViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand<SpaceInfoCP>? SpaceCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<SpaceInfoCP>(this, async items =>
            {
                await MainGame!.MakeMoveAsync(items);
            }, items =>
            {
                if (MainOptionsVisible == false)
                    return false;
                if (items.Status == EnumSpaceType.Blank)
                    return true;
                return false;
            }, this, CommandContainer!);
        }
    }
}