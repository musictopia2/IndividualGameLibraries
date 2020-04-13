using A21DiceGameCP.Data;
using A21DiceGameCP.Logic;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
namespace A21DiceGameCP.ViewModels
{
    [InstanceGame]
    public class A21DiceGameMainViewModel : DiceGamesVM<SimpleDice>
    {
        private readonly A21DiceGameMainGameClass _mainGame; //if we don't need, delete.
        public A21DiceGameMainViewModel(CommandContainer commandContainer,
            A21DiceGameMainGameClass mainGame,
            A21DiceGameVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
        }
        //anything else needed is here.
        protected override bool CanEnableDice()
        {
            return false; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return _mainGame!.SingleInfo!.NumberOfRolls > 0;
        }
        public override bool CanRollDice()
        {
            return base.CanRollDice(); //anything you need to figure out if you can roll is here.
        }
        
    }
}