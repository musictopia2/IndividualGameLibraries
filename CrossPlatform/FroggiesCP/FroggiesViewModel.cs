using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FroggiesCP
{
    public class FroggiesViewModel : SimpleGameVM
    {

        private int _NumberOfFrogs = 3;

        public int NumberOfFrogs
        {
            get { return _NumberOfFrogs; }
            set
            {
                if (SetProperty(ref _NumberOfFrogs, value))
                {
                    //can decide what to do when property changes
                    //LevelPicker.SelectNumberValue(value);
                }

            }
        }

        private int _MovesLeft;

        public int MovesLeft
        {
            get { return _MovesLeft; }
            set
            {
                if (SetProperty(ref _MovesLeft, value))
                {
                    //can decide what to do when property changes

                }

            }
        }

        public override bool CanEnableBasics()
        {
            return true;
        }
        public FroggiesGameBoardCP? GameBoard1;

        public FroggiesViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC)
        {
        }
        public PlainCommand<LilyPadCP>? LilyClickedCommand { get; set; }
        public NumberPicker? LevelPicker;
        public PlainCommand? RedoGameCommand { get; set; }
        public override void Init()
        {
            GameBoard1 = Resolve<FroggiesGameBoardCP>();
            LevelPicker = new NumberPicker(this);
            BasicData data = Resolve<BasicData>();
            if (data.IsXamarinForms == false)
                LevelPicker.LoadNormalNumberRangeValues(3, 60);
            else
                LevelPicker.LoadNormalNumberRangeValues(3, 30);
            LevelPicker.SendEnableProcesses(this, () => NewGameVisible);
            LevelPicker.SelectNumberValue(3); //i think so it defaults at 3.
            LevelPicker.Visible = true;
            LevelPicker.ChangedNumberValueAsync += LevelPicker_ChangedNumberValueAsync;
            RedoGameCommand = new PlainCommand(items =>
            {
                GameBoard1.Redo();
            }, items => !NewGameVisible, this, CommandContainer!);

            LilyClickedCommand = new PlainCommand<LilyPadCP>(async thisLily =>
            {
                await GameBoard1.ProcessLilyClickAsync(thisLily);
            }, items => !NewGameVisible, this, CommandContainer!);
        }

        private Task LevelPicker_ChangedNumberValueAsync(int Chosen)
        {
            NumberOfFrogs = Chosen;
            return Task.CompletedTask;
        }
        public override Task StartNewGameAsync() //this did not require async.
        {
            NewGameVisible = false;
            GameBoard1!.NewGame();
            return Task.CompletedTask;

        }
    }
}