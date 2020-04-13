using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using PickelCardGameCP.Cards;
namespace PickelCardGameCP.Data
{
	[SingletonGame]
	[AutoReset] //usually needs autoreset
	public class PickelCardGameVMData : ObservableObject, ITrickCardGamesData<PickelCardGameCardInformation, EnumSuitList>
	{

		public PickelCardGameVMData(IEventAggregator aggregator,
			CommandContainer command,
			BasicTrickAreaObservable<EnumSuitList, PickelCardGameCardInformation> trickArea1,
			IGamePackageResolver resolver
			)
		{
			Deck1 = new DeckObservablePile<PickelCardGameCardInformation>(aggregator, command);
			Pile1 = new PileObservable<PickelCardGameCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<PickelCardGameCardInformation>(command);
			PlayerHand1.Text = "Your Cards";
			TrickArea1 = trickArea1;
			Bid1 = new NumberPicker(command, resolver);
			Suit1 = new SimpleEnumPickerVM<EnumSuitList, DeckPieceCP>(command, new SuitListChooser());
			Suit1.AutoSelectCategory = EnumAutoSelectCategory.AutoSelect;
			//rethink on the next part.
		}
		public NumberPicker Bid1;
		public SimpleEnumPickerVM<EnumSuitList, DeckPieceCP> Suit1;
		public BasicTrickAreaObservable<EnumSuitList, PickelCardGameCardInformation> TrickArea1 { get; set; }
		public DeckObservablePile<PickelCardGameCardInformation> Deck1 { get; set; }
		public PileObservable<PickelCardGameCardInformation> Pile1 { get; set; }
		public HandObservable<PickelCardGameCardInformation> PlayerHand1 { get; set; }
		public PileObservable<PickelCardGameCardInformation>? OtherPile { get; set; }
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
		private EnumSuitList _trumpSuit;
		[VM]
		public EnumSuitList TrumpSuit
		{
			get { return _trumpSuit; }
			set
			{
				if (SetProperty(ref _trumpSuit, value)) { }
			}
		}
		//try to keep here.

		private int _bidAmount = -1;

		public int BidAmount
		{
			get { return _bidAmount; }
			set
			{
				if (SetProperty(ref _bidAmount, value))
				{
					//can decide what to do when property changes
				}

			}
		}



		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
