using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using RookCP.Cards;
using RookCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RookCP.Data
{
	[SingletonGame]
	[AutoReset] //usually needs autoreset
	public class RookVMData : ObservableObject, ITrickCardGamesData<RookCardInformation, EnumColorTypes>,
		ITrickDummyHand<EnumColorTypes, RookCardInformation>
	{
		private readonly RookGameContainer _gameContainer;
		public RookVMData(IEventAggregator aggregator,
			CommandContainer command,
			RookTrickAreaCP trickArea1,
			IGamePackageResolver resolver,
			RookGameContainer gameContainer
			)
		{
			Deck1 = new DeckObservablePile<RookCardInformation>(aggregator, command);
			Pile1 = new PileObservable<RookCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<RookCardInformation>(command);
			TrickArea1 = trickArea1;
			_gameContainer = gameContainer;
			Bid1 = new NumberPicker(command, resolver);
			Color1 = new SimpleEnumPickerVM<EnumColorTypes, CheckerChoiceCP<EnumColorTypes>>(command, new ColorListChooser<EnumColorTypes>());
			Dummy1 = new DummyHandCP(command);
			Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
			Color1.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
			Color1.ItemClickedAsync += Color1_ItemClickedAsync;

		}

		private Task Bid1_ChangedNumberValueAsync(int chosen)
		{
			BidChosen = chosen;
			return Task.CompletedTask;
		}
		private Task Color1_ItemClickedAsync(EnumColorTypes piece)
		{
			ColorChosen = piece; //hopefully this simple.
			TrumpSuit = piece;// try this too.
			return Task.CompletedTask;
		}

		public NumberPicker Bid1;

		public SimpleEnumPickerVM<EnumColorTypes, CheckerChoiceCP<EnumColorTypes>> Color1;

		public DummyHandCP Dummy1;
		public RookTrickAreaCP TrickArea1 { get; set; }

		BasicTrickAreaObservable<EnumColorTypes, RookCardInformation> ITrickCardGamesData<RookCardInformation, EnumColorTypes>.TrickArea1
		{
			get => TrickArea1;
			set => TrickArea1 = (RookTrickAreaCP)value;
		}

		public DeckObservablePile<RookCardInformation> Deck1 { get; set; }
		public PileObservable<RookCardInformation> Pile1 { get; set; }
		public HandObservable<RookCardInformation> PlayerHand1 { get; set; }
		public PileObservable<RookCardInformation>? OtherPile { get; set; }
		public EnumColorTypes ColorChosen { get; set; }
		public int BidChosen { get; set; } = -1;

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
		private EnumColorTypes _trumpSuit;
		[VM]
		public EnumColorTypes TrumpSuit
		{
			get { return _trumpSuit; }
			set
			{
				if (SetProperty(ref _trumpSuit, value)) { }
			}
		}

		private EnumStatusList _gameStatus;

		[VM]
		public EnumStatusList GameStatus
		{
			get { return _gameStatus; }
			set
			{
				if (SetProperty(ref _gameStatus, value))
				{
					//can decide what to do when property changes
				}

			}
		}


		public DeckObservableDict<RookCardInformation> GetCurrentHandList()
		{
			if (_gameContainer!.SaveRoot!.DummyPlay == true && _gameContainer.PlayerList.Count() == 2)
				return Dummy1!.HandList;
			return _gameContainer.SingleInfo!.MainHandList; //try this way.  hopefully won't cause other problems.
		}

		public int CardSelected()
		{
			if (_gameContainer!.SaveRoot!.DummyPlay == true && _gameContainer.PlayerList.Count() == 2)
				return Dummy1!.ObjectSelected();
			else if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
				throw new BasicBlankException("Only self can show card selected.  If I am wrong, rethink");
			return PlayerHand1!.ObjectSelected();
		}

		public void RemoveCard(int deck)
		{
			if (_gameContainer!.SaveRoot!.DummyPlay == true && _gameContainer.PlayerList.Count() == 2)
				Dummy1!.RemoveDummyCard(deck);
			else
				_gameContainer.SingleInfo!.MainHandList.RemoveObjectByDeck(deck); //because computer player does this too.
		}
		internal bool CanPass { get; set; }
	}
}
