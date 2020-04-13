using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace PassOutDiceGameCP.Data
{
	[SingletonGame]
	public class PassOutDiceGameSaveInfo : BasicSavedBoardDiceGameClass<PassOutDiceGamePlayerItem>
	{ //anything needed for autoresume is here.
		private string _instructions = "";

		public string Instructions
		{
			get { return _instructions; }
			set
			{
				if (SetProperty(ref _instructions, value))
				{
					//can decide what to do when property changes
					if (_model != null)
					{
						_model.Instructions = value;
					}
				}

			}
		}
		private PassOutDiceGameVMData? _model;
		internal void LoadMod(PassOutDiceGameVMData model)
		{
			_model = model;
			_model.Instructions = Instructions;
		}
		public int PreviousSpace { get; set; }
		public CustomBasicList<int> SpacePlayers { get; set; } = new CustomBasicList<int>();
		public bool DidRoll { get; set; }
	}
}