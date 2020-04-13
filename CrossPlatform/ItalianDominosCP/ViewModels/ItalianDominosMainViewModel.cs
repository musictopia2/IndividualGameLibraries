using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using ItalianDominosCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ItalianDominosCP.ViewModels
{
	[InstanceGame]
	public class ItalianDominosMainViewModel : DominoGamesVM<SimpleDominoInfo>
	{
		private readonly ItalianDominosMainGameClass _mainGame; //if we don't need, delete.
		private readonly IDominoGamesData<SimpleDominoInfo> _viewModel;

		public ItalianDominosMainViewModel(
			CommandContainer commandContainer,
			ItalianDominosMainGameClass mainGame,
			IDominoGamesData<SimpleDominoInfo> viewModel,
			BasicData basicData,
			TestOptions test,
			IGamePackageResolver resolver) : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
		{
			_mainGame = mainGame;
			_viewModel = viewModel;
			viewModel.PlayerHand1.Maximum = 8;
			viewModel.PlayerHand1.AutoSelect = HandObservable<SimpleDominoInfo>.EnumAutoType.SelectOneOnly;

		}

		protected override bool CanEnableBoneYard()
		{
			return !_mainGame.SingleInfo!.DrewYet;
		}

		private int _upTo;
		[VM]
		public int UpTo
		{
			get { return _upTo; }
			set
			{
				if (SetProperty(ref _upTo, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private int _nextNumber;
		[VM]
		public int NextNumber
		{
			get { return _nextNumber; }
			set
			{
				if (SetProperty(ref _nextNumber, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		[Command(EnumCommandCategory.Game)]
		public async Task PlayAsync()
		{
			int deck = _viewModel.PlayerHand1.ObjectSelected();
			if (deck == 0)
			{
				await UIPlatform.ShowMessageAsync("You must choose one domino to play");
				return;
			}
			await _mainGame.PlayDominoAsync(deck);
		}
	}
}