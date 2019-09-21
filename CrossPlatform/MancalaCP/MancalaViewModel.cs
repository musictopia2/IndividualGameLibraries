using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
namespace MancalaCP
{
    public class MancalaViewModel : BasicMultiplayerVM<MancalaPlayerItem, MancalaMainGameClass>
    {
        public MancalaViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        private string _Instructions = "";
        public string Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _PiecesAtStart;
        public int PiecesAtStart
        {
            get { return _PiecesAtStart; }
            set
            {
                if (SetProperty(ref _PiecesAtStart, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _PiecesLeft;

        public int PiecesLeft
        {
            get { return _PiecesLeft; }
            set
            {
                if (SetProperty(ref _PiecesLeft, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public GameBoardProcesses? GameBoard1;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<int>(this, async space =>
            {
                if (MainGame!.SingleInfo!.ObjectList.Any(Items => Items.Index == space) == false)
                {
                    return;
                }
                MainGame!.OpenMove();
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendMoveAsync(space + 7); //because reversed.
                await GameBoard1!.AnimateMoveAsync(space);
            }, items => true, this, CommandContainer!);
            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>();
            GameBoardGraphicsCP.CreateSpaceList(MainGame!); //i think i can try to do here.
        }
    }
}