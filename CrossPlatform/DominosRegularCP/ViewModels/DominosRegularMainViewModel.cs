using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using DominosRegularCP.Logic;
namespace DominosRegularCP.ViewModels
{
    [InstanceGame]
    public class DominosRegularMainViewModel : DominoGamesVM<SimpleDominoInfo>
    {
        private readonly IDominoGamesData<SimpleDominoInfo> _viewModel;

        public DominosRegularMainViewModel(
            CommandContainer commandContainer,
            DominosRegularMainGameClass mainGame,
            IDominoGamesData<SimpleDominoInfo> viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver) : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _viewModel = viewModel;
            viewModel.PlayerHand1.Maximum = 8;
            viewModel.PlayerHand1.IgnoreMaxRules = true; //after 8 scrollbars.
        }
        protected override bool CanEnableBoneYard()
        {
            return true;
        }
        public override bool CanEndTurn()
        {
            if (_viewModel.BoneYard!.HasBone())
            {
                return _viewModel.BoneYard.HasDrawn();
            }
            return true;
        }
    }
}