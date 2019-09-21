using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace MinesweeperCP
{
    public class MinesweeperViewModel : SimpleGameVM
    {

        public enum EnumLevel
        {
            Easy = 1, Medium, Hard
        }

        private int _NumberOfMinesLeft;
        public int NumberOfMinesLeft
        {
            get
            {
                return _NumberOfMinesLeft;
            }

            set
            {
                if (SetProperty(ref _NumberOfMinesLeft, value) == true)
                {
                }
            }
        }

        private int _HowManyMinesNeeded = 10;
        public int HowManyMinesNeeded
        {
            get
            {
                return _HowManyMinesNeeded;
            }

            set
            {
                if (SetProperty(ref _HowManyMinesNeeded, value) == true)
                {
                }
            }
        }

        private int _Rows = 9;
        public int Rows
        {
            get
            {
                return _Rows;
            }

            set
            {
                if (SetProperty(ref _Rows, value) == true)
                {
                }
            }
        }

        private int _Columns = 9;
        public int Columns
        {
            get
            {
                return _Columns;
            }

            set
            {
                if (SetProperty(ref _Columns, value) == true)
                {
                }
            }
        }

        private EnumLevel _Level = EnumLevel.Easy;
        public EnumLevel LevelChosen
        {
            get
            {
                return _Level;
            }

            set
            {
                if (SetProperty(ref _Level, value) == true)
                {
                    // code to run
                    if ((int)value == (int)EnumLevel.Easy)
                        HowManyMinesNeeded = 10;
                    else if ((int)value == (int)EnumLevel.Medium)
                        HowManyMinesNeeded = 20;
                    else
                        HowManyMinesNeeded = 30;
                }
            }
        }

        private bool _IsFlagging;
        public bool IsFlagging
        {
            get
            {
                return _IsFlagging;
            }

            set
            {
                if (SetProperty(ref _IsFlagging, value) == true)
                {
                }
            }
        }
        private bool _ToggleVisible;
        public bool ToggleVisible
        {
            get
            {
                return _ToggleVisible;
            }

            set
            {
                if (SetProperty(ref _ToggleVisible, value) == true)
                {
                }
            }
        }


        public async Task GameOverMessageAsync(string message)
        {
            await ShowGameMessageAsync(message);
            NewGameVisible = true;
            ToggleVisible = false;
        }
        public override bool CanEnableBasics()
        {
            return true;
        }

        public MinesweeperGameboardCP? GameBoard1;

        public MinesweeperViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC)
        {
        }
        public ListViewPicker? LevelPicker;
        public PlainCommand? ChangeFlagCommand { get; set; }
        public PlainCommand<MineSquareCP>? SquareClickCommand { get; set; }
        public override void Init()
        {
            GameBoard1 = Resolve<MinesweeperGameboardCP>();
            LevelPicker = new ListViewPicker(this);
            LevelPicker.Visible = true;
            LevelPicker.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
            LevelPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
            LevelPicker.SendEnableProcesses(this, () => NewGameVisible);
            LevelPicker.ItemSelectedAsync += LevelPicker_ItemSelectedAsync;
            LevelPicker.LoadTextList(new CustomBasicList<string>() { "Easy", "Medium", "Hard" });
            LevelPicker.SelectSpecificItem(1);
            ChangeFlagCommand = new PlainCommand(items => IsFlagging = !IsFlagging, items => !NewGameVisible, this, CommandContainer!);
            SquareClickCommand = new PlainCommand<MineSquareCP>(async thisSquare =>
            {
                if (IsFlagging)
                {
                    GameBoard1.FlagSingleSquare(thisSquare);
                    return;
                }
                await GameBoard1.ClickSingleSquareAsync(thisSquare);
            }, items => !NewGameVisible, this, CommandContainer!);
        }

        private Task LevelPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            LevelChosen = (EnumLevel)selectedIndex;
            return Task.CompletedTask;
        }

        public override async Task StartNewGameAsync()
        {
            NewGameVisible = false;
            ToggleVisible = true;
            GameBoard1!.NumberOfColumns = Columns;
            GameBoard1.NumberOfRows = Rows;
            GameBoard1.NumberOfMines = HowManyMinesNeeded;
            await GameBoard1.NewGameAsync(); //i think this simple (?)
        }
    }
}