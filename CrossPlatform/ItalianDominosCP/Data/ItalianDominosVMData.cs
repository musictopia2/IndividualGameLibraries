using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Threading.Tasks; 
namespace ItalianDominosCP.Data
{
	[SingletonGame]
	//[AutoReset]
	public class ItalianDominosVMData : ObservableObject, IDominoGamesData<SimpleDominoInfo>
	{
		//for mexican train dominos, will go ahead and change the type of dominos used.

		public ItalianDominosVMData(CommandContainer command,
			IGamePackageResolver resolver,
			DominosBasicShuffler<SimpleDominoInfo> shuffle
			)
		{
			PlayerHand1 = new HandObservable<SimpleDominoInfo>(command);
			BoneYard = new DominosBoneYardClass<SimpleDominoInfo>(this, command, resolver, shuffle);
			PlayerHand1.ObjectClickedAsync += PlayerHand1_ObjectClickedAsync;
			PlayerHand1.BoardClickedAsync += PlayerHand1_BoardClickedAsync;
		}

		private Task PlayerHand1_BoardClickedAsync()
		{
			if (PlayerBoardClickedAsync == null)
			{
				throw new BasicBlankException("Board clicked was never created.  Rethink");
			}
			return PlayerBoardClickedAsync.Invoke();
		}

		private Task PlayerHand1_ObjectClickedAsync(SimpleDominoInfo domino, int index)
		{
			if (HandClickedAsync == null)
			{
				throw new BasicBlankException("The hand clicked was never done.  Rethink");
			}
			return HandClickedAsync.Invoke(domino, index);
		}

		private string _normalTurn = "";
		[VM]
		public string NormalTurn
		{
			get { return _normalTurn; }
			set
			{
				if (SetProperty(ref _normalTurn, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		private string _status = "";
		[VM] //use this tag to transfer to the actual view model.  this is being done to avoid overflow errors.
		public string Status
		{
			get { return _status; }
			set
			{
				if (SetProperty(ref _status, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		public Func<SimpleDominoInfo, Task>? DrewDominoAsync { get; set; }
		public HandObservable<SimpleDominoInfo> PlayerHand1 { get; set; }
		public DominosBoneYardClass<SimpleDominoInfo> BoneYard { get; set; }
		public Func<Task>? PlayerBoardClickedAsync { get; set; }
		public Func<SimpleDominoInfo, int, Task>? HandClickedAsync { get; set; }

		public bool CanEnableBasics()
		{
			return true;
		}

		public bool CanEnableAlways()
		{
			return true;
		}

		//any other ui related properties will be here.
		//can copy/paste for the actual view model.
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

	}
}
