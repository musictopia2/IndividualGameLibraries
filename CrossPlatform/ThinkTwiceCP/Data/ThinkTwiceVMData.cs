using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace ThinkTwiceCP.Data
{
	[SingletonGame]
	public class ThinkTwiceVMData : ObservableObject, IBasicDiceGamesData<SimpleDice>
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
		private readonly ThinkTwiceGameContainer _gameContainer;

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

		public string DisplayScoreText { get; set; } = "Category Information";


		public CustomBasicList<string> TextList;
		public DiceCup<SimpleDice>? Cup { get; set; }
		public ThinkTwiceVMData(IGamePackageResolver resolver, 
			CommandContainer command,
			ThinkTwiceGameContainer gameContainer
			)
		{
			_resolver = resolver;
			_command = command;
			_gameContainer = gameContainer;
			TextList = new CustomBasicList<string>
			{
				"Different (1, 2, 3, 4, 5, 6)",
				"Even (2, 4, 6)",
				"High (4, 5, 6)",
				"Low (1, 2, 3)",
				"Odd (1, 3, 5)",
				"Same (2, 2, 2)"
			};
		}
		public void LoadCup(ISavedDiceList<SimpleDice> saveRoot, bool autoResume)
		{
			if (Cup != null && autoResume)
			{
				return;
			}
			Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, _resolver, _command);
			if (autoResume == true)
			{
				Cup.CanShowDice = true;
			}
			Cup.HowManyDice = 6; //you specify how many dice here.
			Cup.Visible = true; //i think.
			Cup.ShowHold = true;
		}


		//any other ui related properties will be here.
		//can copy/paste for the actual view model.
		private int _score;
		[VM]
		public int Score
		{
			get { return _score; }
			set
			{
				if (SetProperty(ref _score, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private string _categoryChosen = "None";
		[VM]
		public string CategoryChosen
		{
			get { return _categoryChosen; }
			set
			{
				if (SetProperty(ref _categoryChosen, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private int _itemSelected;

		public int ItemSelected
		{
			get { return _itemSelected; }
			set
			{
				if (SetProperty(ref _itemSelected, value))
				{
					//can decide what to do when property changes
					if (ItemSelected == -1)
					{
						CategoryChosen = "None";
					}
					else
					{
						CategoryChosen = TextList[ItemSelected];
					}
					_gameContainer.SaveRoot.CategorySelected = value;
				}

			}
		}

		public void ClearBoard() //anything the gameboard uses has to be here.
		{
			ItemSelected = -1;
		}
		public bool ItemSent { get; set; }
	}
}
