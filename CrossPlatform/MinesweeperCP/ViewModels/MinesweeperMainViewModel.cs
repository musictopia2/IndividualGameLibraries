using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using MinesweeperCP.Data;
using MinesweeperCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MinesweeperCP.ViewModels
{
    [InstanceGame]
    public class MinesweeperMainViewModel : Screen, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer, ILevelVM
    {
        private readonly IEventAggregator _aggregator;
        private readonly MinesweeperMainGameClass _mainGame;


        private int _numberOfMinesLeft;
        public int NumberOfMinesLeft
        {
            get
            {
                return _numberOfMinesLeft;
            }

            set
            {
                if (SetProperty(ref _numberOfMinesLeft, value) == true)
                {
                }
            }
        }

        private int _howManyMinesNeeded = 10;
        public int HowManyMinesNeeded
        {
            get
            {
                return _howManyMinesNeeded;
            }

            set
            {
                if (SetProperty(ref _howManyMinesNeeded, value) == true)
                {
                }
            }
        }

        private int _rows = 9;
        public int Rows
        {
            get
            {
                return _rows;
            }

            set
            {
                if (SetProperty(ref _rows, value) == true)
                {
                }
            }
        }

        private int _columns = 9;
        public int Columns
        {
            get
            {
                return _columns;
            }

            set
            {
                if (SetProperty(ref _columns, value) == true)
                {
                }
            }
        }


        public EnumLevel LevelChosen { get; set; }



        private bool _isFlagging;
        public bool IsFlagging
        {
            get
            {
                return _isFlagging;
            }

            set
            {
                if (SetProperty(ref _isFlagging, value) == true)
                {
                }
            }
        }
        [Command(EnumCommandCategory.Plain)]
        public void ChangeFlag()
        {
            IsFlagging = !IsFlagging; //no need for game visible because that is not responsible for it.
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task MakeMoveAsync(MineSquareCP square)
        {
            if (IsFlagging)
            {
                _mainGame.FlagSingleSquare(square);
                NumberOfMinesLeft = _mainGame.GetMinesLeft();
                return;
            }
            await _mainGame.ClickSingleSquareAsync(square);
        }
        public MinesweeperMainViewModel(IEventAggregator aggregator,
            CommandContainer commandContainer,
            IGamePackageResolver resolver,
            LevelClass level
            )
        {
            _aggregator = aggregator;
            LevelChosen = level.Level; //at this point, can't choose level because its already chosen.
            this.PopulateMinesNeeded();
            CommandContainer = commandContainer;
            _mainGame = resolver.ReplaceObject<MinesweeperMainGameClass>(); //hopefully this works.  means you have to really rethink.
        }

        public CommandContainer CommandContainer { get; set; }

        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        public bool CanEnableBasics()
        {
            return true; //because maybe you can't enable it.
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            NumberOfMinesLeft = _mainGame.NumberOfMines; //i think.
            _mainGame.NumberOfColumns = Columns;
            _mainGame.NumberOfRows = Rows;
            _mainGame.NumberOfMines = HowManyMinesNeeded;
            await _mainGame.NewGameAsync();
        }
    }
}
