using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using FillOrBustCP.Cards;
namespace FillOrBustCP.Data
{
	[SingletonGame]
	[AutoReset] //usually needs autoreset
	public class FillOrBustVMData : ObservableObject, IBasicCardGamesData<FillOrBustCardInformation>, ICup<SimpleDice>
	{

		public FillOrBustVMData(IEventAggregator aggregator, CommandContainer command, IGamePackageResolver resolver)
		{
			Deck1 = new DeckObservablePile<FillOrBustCardInformation>(aggregator, command);
			Pile1 = new PileObservable<FillOrBustCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<FillOrBustCardInformation>(command);
			_command = command;
			_resolver = resolver;
		}

		public void LoadCup(FillOrBustSaveInfo saveRoot, bool autoResume) //could be iffy.
		{
			if (Cup != null && autoResume)
				return;
			Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, _resolver, _command);
			if (autoResume == true)
			{
				Cup.CanShowDice = true;
			}
			else
				Cup.HowManyDice = 6;
		}

		public DiceCup<SimpleDice>? Cup { get; set; }
		public DeckObservablePile<FillOrBustCardInformation> Deck1 { get; set; }
		public PileObservable<FillOrBustCardInformation> Pile1 { get; set; }
		public HandObservable<FillOrBustCardInformation> PlayerHand1 { get; set; }
		public PileObservable<FillOrBustCardInformation>? OtherPile { get; set; }
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
		private int _diceScore;
		[VM]
		public int DiceScore
		{
			get { return _diceScore; }
			set
			{
				if (SetProperty(ref _diceScore, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private int _tempScore;
		[VM]
		public int TempScore
		{
			get { return _tempScore; }
			set
			{
				if (SetProperty(ref _tempScore, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private string _instructions = "";
		private readonly CommandContainer _command;
		private readonly IGamePackageResolver _resolver;

		[VM]
		public string Instructions
		{
			get { return _instructions; }
			set
			{
				if (SetProperty(ref _instructions, value))
				{
					//can decide what to do when property changes
				}

			}
		}
	}
}
