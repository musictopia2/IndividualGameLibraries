using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace ConnectTheDotsCP
{
    public class ConnectTheDotsViewModel : SimpleBoardGameVM<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectTheDotsPlayerItem, ConnectTheDotsMainGameClass, int>
    {
        internal GameBoardProcesses? GameBoard1;
        public ConnectTheDotsViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<int>(this, async dot =>
            {
                if (GameBoard1!.IsValidMove(dot) == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    return;
                }
                if (MainGame!.ThisData!.MultiPlayer == true)
                    await MainGame.ThisNet!.SendMoveAsync(dot);
                await GameBoard1.MakeMoveAsync(dot);
            }, items =>
            {
                return true;
            }, this, CommandContainer!);
            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>();
        }
    }
}