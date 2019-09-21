using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace MastermindCP
{
    public class MastermindViewModel : SimpleGameVM
    {

        private bool _GameFinished;

        public bool GameFinished
        {
            get { return _GameFinished; }
            set
            {
                if (SetProperty(ref _GameFinished, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public override bool CanEnableBasics()
        {
            return true;
        }
        public HintChooserViewModel? Guess1;
        public SimpleEnumPickerVM<EnumColorPossibilities, CirclePieceCP<EnumColorPossibilities>, CustomColorClass>? Color1;
        public int LevelChosen { get; set; } = 3; //defaults at level 3.
        public ListViewPicker? LevelPicker;

        private MastermindMainGameClass? _mainGame;

        public MastermindViewModel(ISimpleUI TempUI, IGamePackageResolver TempC) : base(TempUI, TempC)
        {
        }
        public PlainCommand? LevelCommand { get; set; }
        public PlainCommand? GiveUpCommand { get; set; }
        public PlainCommand? AcceptCommand { get; set; }
        public override void Init()
        {
            _mainGame = Resolve<MastermindMainGameClass>();
            LevelPicker = new ListViewPicker(this);
            LevelPicker.Visible = true;
            LevelPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
            GameFinished = true;
            LevelPicker.SendEnableProcesses(this, () =>
            {
                return GameFinished;
            });
            LevelPicker.LoadTextList(new CustomBasicList<string>() { "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6" });
            LevelPicker.SelectSpecificItem(3);
            LevelPicker.ItemSelectedAsync += LevelPicker_ItemSelectedAsync;
            LevelCommand = new PlainCommand(async items =>
            {
                await ShowGameMessageAsync("Level 1 has 4 possible colors that are only used once" + Constants.vbCrLf + "Level 2 has 4 possible colors that can be used more than once" + Constants.vbCrLf + "Level 3 has 6 possible colors that can only be used once" + Constants.vbCrLf + "Level 4 has 6 possible colors that can be used more than once" + Constants.vbCrLf + "Level 5 has 8 possible colors that can be used once" + Constants.vbCrLf + "Level 6 has 8 possible colors that can be used more than once");
            }, items => true, this, CommandContainer!);

            GiveUpCommand = new PlainCommand(async items =>
            {
                if (_thisGlobal!.Solution == null)
                    return;
                if (_thisGlobal.Solution.Count == 0)
                    return;
                await _mainGame.GiveUpAsync();
            }, items =>
            {
                if (GameFinished)
                    return false;
                if (_thisGlobal!.Solution == null)
                    return false;
                return _thisGlobal.Solution.Count > 0;
            }, this, CommandContainer!);
            AcceptCommand = new PlainCommand(async items =>
            {
                await Guess1!.SubmitGuessAsync();
            }, items =>
            {
                if (GameFinished)
                    return false;
                return Guess1!.DidFillInGuess();
            }, this, CommandContainer!);
        }

        private Task LevelPicker_ItemSelectedAsync(int SelectedIndex, string SelectedText)
        {
            LevelChosen = SelectedIndex;
            return Task.CompletedTask;
        }
        private GlobalClass? _thisGlobal;
        public void Finish()
        {
            Color1 = new SimpleEnumPickerVM<EnumColorPossibilities, CirclePieceCP<EnumColorPossibilities>, CustomColorClass>(this);
            Guess1 = new HintChooserViewModel(this);
            Color1.Visible = true;
            Color1.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
            Color1.SendEnableProcesses(this, () =>
            {
                return !GameFinished;
            });
            Color1.ItemClickedAsync += Color1_ItemClickedAsync;
            _thisGlobal = MainContainer!.Resolve<GlobalClass>();
        }

        private Task Color1_ItemClickedAsync(EnumColorPossibilities ThisPiece)
        {
            Guess1!.SelectedColorForCurrentGuess(ThisPiece);
            return Task.CompletedTask;
        }

        public override Task StartNewGameAsync()
        {
            _mainGame!.NewGame();
            return Task.CompletedTask;
        }
    }
}