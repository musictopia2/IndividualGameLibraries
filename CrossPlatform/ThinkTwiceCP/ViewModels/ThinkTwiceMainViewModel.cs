using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThinkTwiceCP.Data;
using ThinkTwiceCP.Logic;
namespace ThinkTwiceCP.ViewModels
{
    [InstanceGame]
    public class ThinkTwiceMainViewModel : DiceGamesVM<SimpleDice>
    {
        private readonly ThinkTwiceMainGameClass _mainGame; //if we don't need, delete.
        private readonly ThinkTwiceVMData _model;
        private readonly IGamePackageResolver _resolver;

        public ThinkTwiceMainViewModel(CommandContainer commandContainer,
            ThinkTwiceMainGameClass mainGame,
            ThinkTwiceVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _resolver = resolver;
        }
        //anything else needed is here.

        public ScoreViewModel? ScoreScreen { get; set; }

        protected override async Task ActivateAsync()
        {
            ScoreScreen = _resolver.Resolve<ScoreViewModel>();
            await LoadScreenAsync(ScoreScreen);
            await base.ActivateAsync();
        }
        protected override bool CanEnableDice()
        {
            return CanRollDice();
        }
        public override bool CanEndTurn()
        {
            return _model.ItemSelected > -1; //because you have to choose a score.
        }
        public override async Task EndTurnAsync()
        {
            if (_model.ItemSent == false && _mainGame.BasicData.MultiPlayer) //so it can send the scores to the other players.
            {
                await _mainGame.BeforeRollingAsync(); //even though you are not rolling.
            }
            await base.EndTurnAsync();
        }
        public override bool CanRollDice()
        {
            return RollNumber <= 2;
        }
        public bool CanRollMult
        {
            get
            {
                if (RollNumber == 0)
                {
                    return false;
                }
                return _mainGame.SaveRoot.WhichMulti == -1;
            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task RollMultAsync()
        {
            await _mainGame.RollMultsAsync();
        }
        private int _score;
        [VM]
        public int Score
        {
            get { return _score; }
            set
            {
                if (SetProperty(ref _score, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _categoryChosen = "None";
        [VM]
        public string CategoryChosen
        {
            get { return _categoryChosen; }
            set
            {
                if (SetProperty(ref _categoryChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}