using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using SequenceDiceCP.Logic;
namespace SequenceDiceCP.Data
{
	[SingletonGame]
	public class SequenceDiceSaveInfo : BasicSavedBoardDiceGameClass<SequenceDicePlayerItem>
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
		private SequenceDiceVMData? _model;
		internal void LoadMod(SequenceDiceVMData model)
		{
			_model = model;
			_model.Instructions = Instructions;
		}
		public EnumGameStatusList GameStatus { get; set; }//don't need previousspace because that is given from the spaceinfocp
		public SequenceBoardCollection GameBoard { get; set; } = new SequenceBoardCollection();
	}
}