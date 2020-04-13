using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
//i think this is the most common things i like to do
namespace AggravationCP.Data
{
	[SingletonGame]
	public class AggravationSaveInfo : BasicSavedBoardDiceGameClass<AggravationPlayerItem>
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
		private AggravationVMData? _model;
		internal void LoadMod(AggravationVMData model)
		{
			_model = model;
			_model.Instructions = Instructions;
		}
		public int PreviousSpace { get; set; }
		public CustomBasicList<MoveInfo> MoveList { get; set; } = new CustomBasicList<MoveInfo>();
		public EnumColorChoice OurColor { get; set; } //decided to use different enum.
		public int DiceNumber { get; set; } //hopefully this is fine too.
	}
}