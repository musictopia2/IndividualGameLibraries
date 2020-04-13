using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using PaydayCP.Cards;
namespace PaydayCP.Data
{
	[SingletonGame]
	public class PaydaySaveInfo : BasicSavedBoardDiceGameClass<PaydayPlayerItem>
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

		private EnumStatus _gameStatus;

		public EnumStatus GameStatus
		{
			get { return _gameStatus; }
			set
			{
				if (SetProperty(ref _gameStatus, value))
				{
					if (_model != null)
					{
						_model.GameStatus = value;
					}
				}

			}
		}


		private PaydayVMData? _model;
		internal void LoadMod(PaydayVMData model)
		{
			_model = model;
			_model.Instructions = Instructions;
			_model.GameStatus = GameStatus;
		}

		public int NumberHighlighted { get; set; } // i think this needs to be saved
		public decimal LotteryAmount { get; set; }
		public int MaxMonths { get; set; }
		public DealCard? YardSaleDealCard { get; set; }
		public DeckRegularDict<MailCard> MailListLeft { get; set; } = new DeckRegularDict<MailCard>();
		public DeckRegularDict<DealCard> DealListLeft { get; set; } = new DeckRegularDict<DealCard>();
		public DeckRegularDict<CardInformation> OutCards { get; set; } = new DeckRegularDict<CardInformation>();

		public int RemainingMove { get; set; } // don't know if the other 3 has to be there.  better be safe than sorry
		public bool EndOfMonth { get; set; }
		public bool EndGame { get; set; }
		public int DiceNumber { get; set; }
		public MailCard? CurrentMail { get; set; }
		public DealCard? CurrentDeal { get; set; }
	}
}