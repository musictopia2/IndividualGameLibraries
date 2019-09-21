using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace CandylandCP
{
    public class CandylandViewModel : BasicMultiplayerVM<CandylandPlayerItem, CandylandMainGameClass>
    {
        public CandylandViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand? CastleClickCommand { get; set; }
        public BasicGameCommand<int>? SpaceClickCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            CastleClickCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.GameOverAsync();
            }, items =>
            {
                return true;
            }, this, CommandContainer!);
            SpaceClickCommand = new BasicGameCommand<int>(this, async items =>
            {
                await MainGame!.MakeMoveAsync(items); //i think this simple.  if i am wrong, rethink.
            }, items =>
            {
                return true;
            }, this, CommandContainer!);
        }
    }
}