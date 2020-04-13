using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SixtySix2PlayerCP.Cards;
namespace SixtySix2PlayerCP.Data
{
	[SingletonGame]
	[AutoReset] //usually needs autoreset
	public class SixtySix2PlayerVMData : ObservableObject, ITrickCardGamesData<SixtySix2PlayerCardInformation, EnumSuitList>
	{

		public SixtySix2PlayerVMData(IEventAggregator aggregator,
			CommandContainer command,
			TwoPlayerTrickObservable<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo> trickArea1
			)
		{
			Deck1 = new DeckObservablePile<SixtySix2PlayerCardInformation>(aggregator, command);
			Pile1 = new PileObservable<SixtySix2PlayerCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<SixtySix2PlayerCardInformation>(command);
			TrickArea1 = trickArea1;
			Marriage1 = new HandObservable<SixtySix2PlayerCardInformation>(command);
			Deck1.DrawInCenter = true;
			PlayerHand1.Maximum = 6;
			Marriage1.Visible = false;
			Marriage1.Text = "Cards For Marriage";

		}
		public HandObservable<SixtySix2PlayerCardInformation> Marriage1;
		public TwoPlayerTrickObservable<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo> TrickArea1 { get; set; }
		public DeckObservablePile<SixtySix2PlayerCardInformation> Deck1 { get; set; }
		public PileObservable<SixtySix2PlayerCardInformation> Pile1 { get; set; }
		public HandObservable<SixtySix2PlayerCardInformation> PlayerHand1 { get; set; }
		public PileObservable<SixtySix2PlayerCardInformation>? OtherPile { get; set; }
		BasicTrickAreaObservable<EnumSuitList, SixtySix2PlayerCardInformation> ITrickCardGamesData<SixtySix2PlayerCardInformation, EnumSuitList>.TrickArea1
		{
			get => TrickArea1;
			set => TrickArea1 = (TwoPlayerTrickObservable<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>)value;
		}
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

		private int _bonusPoints;
		[VM]
		public int BonusPoints
		{
			get { return _bonusPoints; }
			set
			{
				if (SetProperty(ref _bonusPoints, value))
				{
					//can decide what to do when property changes
				}

			}
		}


		public CustomBasicList<ScoreValuePair> GetDescriptionList()
		{
			return new CustomBasicList<ScoreValuePair>()
			{
				new ScoreValuePair("Marriage In Trumps (K, Q announced)", 40),
				new ScoreValuePair("Marriage In Any Other Suit (K, Q announced)", 20),
				new ScoreValuePair("Each Ace", 11),
				new ScoreValuePair("Each 10", 10),
				new ScoreValuePair("Each King", 4),
				new ScoreValuePair("Each Queen", 3),
				new ScoreValuePair("Each Jack", 2),
				new ScoreValuePair("Last Trick", 10)
			};
		}

	}
}
