using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Messenging;
using CountdownCP.Data;
using CountdownCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CountdownCP.ViewModels
{
    [InstanceGame]
    public class CountdownMainViewModel : DiceGamesVM<CountdownDice>
    {
        private readonly CountdownVMData _model;
        private readonly IEventAggregator _aggregator;

        public CountdownMainViewModel(CommandContainer commandContainer,
            CountdownMainGameClass mainGame,
            CountdownVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller,
            IEventAggregator aggregator
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller)
        {
            _model = viewModel;
            _aggregator = aggregator;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        }

        private void CommandContainer_ExecutingChanged()
        {
            if (_model.Cup!.Visible == false)
            {
                return;
            }
            _aggregator.RepaintBoard();
        }
        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }
        //anything else needed is here.
        protected override bool CanEnableDice()
        {
            return false; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return true; //if you can't or has extras, do here.
        }
        public override bool CanRollDice()
        {
            return false;
        }
        private int _round; //this is needed because the game has to end at some point no matter what even if tie.
        [VM]
        public int Round
        {
            get { return _round; }
            set
            {
                if (SetProperty(ref _round, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        [Command(EnumCommandCategory.Game)]
        public void Hint()
        {
            CountdownVMData.ShowHints = true; //hopefully this simple this time because of the execute change event.
        }

    }
}