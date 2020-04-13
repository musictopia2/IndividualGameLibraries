using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
namespace CrazyEightsCP.Data
{
	[SingletonGame]
	public class CrazyEightsSaveInfo : BasicSavedCardClass<CrazyEightsPlayerItem, RegularSimpleCard>
	{ //anything needed for autoresume is here.

		private bool _chooseSuit;

		public bool ChooseSuit
		{
			get { return _chooseSuit; }
			set
			{
				if (SetProperty(ref _chooseSuit, value))
				{
					//can decide what to do when property changes
					if (_model != null)
					{
						_model.ChooseSuit = value;
					}
				}

			}
		}

		public EnumSuitList CurrentSuit { get; set; }
		public EnumCardValueList CurrentNumber { get; set; } //decided to use the enum now.
		private CrazyEightsVMData? _model;
		public void LoadMod(CrazyEightsVMData model)
		{
			_model = model;
		}


	}
}