using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace DeadDie96CP.Data
{
	[SingletonGame]
	public class DeadDie96VMData : ObservableObject, IBasicDiceGamesData<TenSidedDice>
	{
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
		private int _rollNumber;
		private readonly IGamePackageResolver _resolver;
		private readonly CommandContainer _command;

		[VM]
		public int RollNumber
		{
			get { return _rollNumber; }
			set
			{
				if (SetProperty(ref _rollNumber, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		public DiceCup<TenSidedDice>? Cup { get; set; }
		public DeadDie96VMData(IGamePackageResolver resolver, CommandContainer command)
		{
			_resolver = resolver;
			_command = command;
		}
		public void LoadCup(ISavedDiceList<TenSidedDice> saveRoot, bool autoResume)
		{
			if (Cup != null && autoResume)
			{
				return;
			}
			Cup = new DiceCup<TenSidedDice>(saveRoot.DiceList, _resolver, _command);
			if (autoResume == true)
			{
				Cup.CanShowDice = true;
			}
			Cup.HowManyDice = 5; //you specify how many dice here.
			Cup.Visible = true; //i think.
			Cup.ShowHold = false;
		}


		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
