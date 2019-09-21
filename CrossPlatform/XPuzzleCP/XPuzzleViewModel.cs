using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks;
namespace XPuzzleCP
{
    public class XPuzzleViewModel : SimpleGameVM
    {
        private XPuzzleGameBoardClass? _gameBoard;
        private XPuzzleGlobalMod? _thisGlobal;
        public PlainCommand<XPuzzleSpaceInfo>? SpaceCommand { get; set; } //this time, use plaincommand because you don't have to do new game.
        public XPuzzleViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        public override void Init()
        {
            _gameBoard = MainContainer!.Resolve<XPuzzleGameBoardClass>();
            _thisGlobal = MainContainer.Resolve<XPuzzleGlobalMod>();
            SpaceCommand = new PlainCommand<XPuzzleSpaceInfo>(async Items =>
            {
                await _gameBoard.MakeMoveAsync(Items);
                EnumMoveList NextMove = _gameBoard.Results();
                if (NextMove == EnumMoveList.TurnOver)
                    return; //will automatically enable it again.
                if (NextMove == EnumMoveList.Won)
                    await ThisMessage.ShowMessageBox("Congratulations, you won"); //no need for isbusy for this game.    
            }, Items => true, this, CommandContainer!);
        }
        public override async Task StartNewGameAsync()
        {
            NewGameVisible = true; //because on this game, you can always start another game.
            await _gameBoard!.NewGameAsync();
            NewGameVisible = true;
            _thisGlobal!.GameLoaded = true;
            CommandContainer!.IsExecuting = false;
        }
    }
}