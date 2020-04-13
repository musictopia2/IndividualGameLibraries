using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using DeadDie96CP.Data;
using DeadDie96CP.Logic;
namespace DeadDie96CP.ViewModels
{
    [InstanceGame]
    public class DeadDie96MainViewModel : DiceGamesVM<TenSidedDice>
    {
        public DeadDie96MainViewModel(CommandContainer commandContainer,
            DeadDie96MainGameClass mainGame,
            DeadDie96VMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller)
        {
        }
        //anything else needed is here.
        protected override bool CanEnableDice()
        {
            return false; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return false; //if you can't or has extras, do here.
        }


    }
}