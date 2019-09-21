using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SolitaireBoardGameCP
{
    public class SolitaireBoardGameViewModel : SimpleGameVM
    {
        SolitaireBoardGameSaveInfo? _games;
        SolitaireBoardGameMainGameClass? _boards;

        public PlainCommand<GameSpace>? SpaceCommand { get; set; }
        //public Command SpaceCommand { get; set; }
        private bool _BoardEnabled;

        public bool BoardEnabled
        {
            get { return _BoardEnabled; }
            set
            {
                if (SetProperty(ref _BoardEnabled, value))
                {
                    //can decide what to do when property changes
                    SpaceCommand!.ReportCanExecuteChange();
                }

            }
        }
        public SolitaireBoardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        public override void Init()
        {
            _games = MainContainer!.Resolve<SolitaireBoardGameSaveInfo>();
            SpaceCommand = new PlainCommand<GameSpace>(async Items =>
            {
                await _boards!.ProcessCommandAsync(Items);
            }, Items => BoardEnabled, this, CommandContainer!);
            _boards = MainContainer.Resolve<SolitaireBoardGameMainGameClass>();
            _boards.FinishLoading(); //because something else needs the board first.
        }
        public override async Task StartNewGameAsync()
        {
            //this has no autoresume
            if (_games!.SpaceList.Count() == 0)
                throw new BasicBlankException("Cannot have 0 items in the gameboard collection");
            //since no autoresume, just go directly to load the board.
            await _boards!.NewGameAsync();
            NewGameVisible = true;
            BoardEnabled = true;
            CommandContainer!.IsExecuting = false;
        }
    }
}