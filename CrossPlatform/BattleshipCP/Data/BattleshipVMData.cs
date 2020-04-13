using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace BattleshipCP.Data
{
	[SingletonGame]
	public class BattleshipVMData : ObservableObject, IViewModelData
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

		private EnumShipList _shipSelected; // i think it needs to be here.
		[VM]
		public EnumShipList ShipSelected
		{
			get
			{
				return _shipSelected;
			}
			set
			{
				if (SetProperty(ref _shipSelected, value) == true)
				{
				}
			}
		}

		private bool _shipDirectionsVisible;
		[VM]
		public bool ShipDirectionsVisible
		{
			get { return _shipDirectionsVisible; }
			set
			{
				if (SetProperty(ref _shipDirectionsVisible, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		public BattleshipVMData()
		{

		}
		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
