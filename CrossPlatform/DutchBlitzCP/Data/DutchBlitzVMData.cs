using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.SpecializedGameTypes.StockClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using DutchBlitzCP.Cards;
using DutchBlitzCP.Piles;
using System.Linq;
namespace DutchBlitzCP.Data
{
	[SingletonGame]
	[AutoReset] //usually needs autoreset
	public class DutchBlitzVMData : ObservableObject, IBasicCardGamesData<DutchBlitzCardInformation>
	{

		public DutchBlitzVMData(IEventAggregator aggregator, CommandContainer command, DutchBlitzGameContainer gameContainer)
		{
			Deck1 = new DeckObservablePile<DutchBlitzCardInformation>(aggregator, command);
			Pile1 = new PileObservable<DutchBlitzCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<DutchBlitzCardInformation>(command);
			Stops = new CustomStopWatchCP();
			_aggregator = aggregator;
			_command = command;
			_gameContainer = gameContainer;
			Stops.MaxTime = 7000;
			Pile1.Text = "Waste";
			StockPile = new StockViewModel(command, aggregator);
			PublicPiles1 = new PublicViewModel(gameContainer); //hopefully no overflows using the game container (?)
			OtherPile = Pile1;
		}

		public StockViewModel StockPile;
		public DiscardPilesVM<DutchBlitzCardInformation>? DiscardPiles;
		public PublicViewModel PublicPiles1;
		internal bool DidStartTimer { get; set; } //maybe here.
		internal void LoadDiscards()
		{
			if (_gameContainer!.PlayerList.Count() == 2)
				_gameContainer.MaxDiscard = 5;
			else if (_gameContainer.PlayerList.Count() == 3)
				_gameContainer.MaxDiscard = 4;
			else
				_gameContainer.MaxDiscard = 3;
			DiscardPiles = new DiscardPilesVM<DutchBlitzCardInformation>(_command, _aggregator);
			DiscardPiles.Init(_gameContainer.MaxDiscard);

		}

		public DeckObservablePile<DutchBlitzCardInformation> Deck1 { get; set; }
		public PileObservable<DutchBlitzCardInformation> Pile1 { get; set; }
		public HandObservable<DutchBlitzCardInformation> PlayerHand1 { get; set; }
		public PileObservable<DutchBlitzCardInformation>? OtherPile { get; set; }
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

		public CustomStopWatchCP Stops;
		private readonly IEventAggregator _aggregator;
		private readonly CommandContainer _command;
		private readonly DutchBlitzGameContainer _gameContainer;
	}
}
