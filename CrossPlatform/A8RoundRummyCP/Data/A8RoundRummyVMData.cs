using A8RoundRummyCP.Cards;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
//i think this is the most common things i like to do
namespace A8RoundRummyCP.Data
{
	[SingletonGame]
	[AutoReset] //usually needs autoreset
	public class A8RoundRummyVMData : ObservableObject, IBasicCardGamesData<A8RoundRummyCardInformation>
	{

		public A8RoundRummyVMData(IEventAggregator aggregator, CommandContainer command)
		{
			Deck1 = new DeckObservablePile<A8RoundRummyCardInformation>(aggregator, command);
			Pile1 = new PileObservable<A8RoundRummyCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<A8RoundRummyCardInformation>(command);
		}
		public DeckObservablePile<A8RoundRummyCardInformation> Deck1 { get; set; }
		public PileObservable<A8RoundRummyCardInformation> Pile1 { get; set; }
		public HandObservable<A8RoundRummyCardInformation> PlayerHand1 { get; set; }
		public PileObservable<A8RoundRummyCardInformation>? OtherPile { get; set; }
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

		private string _nextTurn = "";
		[VM]
		public string NextTurn
		{
			get { return _nextTurn; }
			set
			{
				if (SetProperty(ref _nextTurn, value))
				{
					//can decide what to do when property changes
				}
			}
		}

		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
