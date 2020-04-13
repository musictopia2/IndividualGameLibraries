using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using MilkRunCP.Cards;
using MilkRunCP.Logic;
using Newtonsoft.Json;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MilkRunCP.Data
{
    public class MilkRunPlayerItem : PlayerSingleHand<MilkRunCardInformation>
    { //anything needed is here
        //this time, decided to go ahead and just do all here to be easier for organizing.
        public CustomBasicList<BasicPileInfo<MilkRunCardInformation>> ChocolateSavedList { get; set; } = new CustomBasicList<BasicPileInfo<MilkRunCardInformation>>();
        public CustomBasicList<BasicPileInfo<MilkRunCardInformation>> StrawberrySavedList { get; set; } = new CustomBasicList<BasicPileInfo<MilkRunCardInformation>>();
        [JsonIgnore]
        public BasicMultiplePilesCP<MilkRunCardInformation>? StrawberryPiles;
        [JsonIgnore]
        public BasicMultiplePilesCP<MilkRunCardInformation>? ChocolatePiles;
        private int _chocolateDeliveries;
        public int ChocolateDeliveries
        {
            get
            {
                return _chocolateDeliveries;
            }
            set
            {
                if (SetProperty(ref _chocolateDeliveries, value) == true)
                    // code to run
                    OnPropertyChanged(nameof(ReachedChocolateGoal));
            }
        }
        public bool ReachedChocolateGoal
        {
            get
            {
                if (ChocolateDeliveries >= 30)
                    return true;
                return false;
            }
        }
        private int _strawberryDeliveries;
        public int StrawberryDeliveries
        {
            get
            {
                return _strawberryDeliveries;
            }
            set
            {
                if (SetProperty(ref _strawberryDeliveries, value) == true)
                    // code to run
                    OnPropertyChanged(nameof(ReachedStrawberryGoal));
            }
        }
        public bool ReachedStrawberryGoal
        {
            get
            {
                if (StrawberryDeliveries >= 30)
                    return true;
                return false;
            }
        }
        private EventAggregator? _thisE;
        private MilkRunMainGameClass? _mainGame;
        private MilkRunGameContainer? _gameContainer;
        public void LoadPiles(MilkRunMainGameClass mainGame, MilkRunGameContainer gameContainer)
        {
            _mainGame = mainGame;
            _gameContainer = gameContainer;
            StrawberryPiles = new BasicMultiplePilesCP<MilkRunCardInformation>(_gameContainer.Command, _mainGame.Aggregator);
            StrawberryPiles.PileClickedAsync += StrawberryPiles_PileClickedAsync;
            StrawberryPiles.Rows = 1;
            StrawberryPiles.Columns = 3;
            StrawberryPiles.HasText = true;
            StrawberryPiles.HasFrame = true;
            //i don't think we need to prove true anymore.
            StrawberryPiles.Style = BasicMultiplePilesCP<MilkRunCardInformation>.EnumStyleList.HasList;
            StrawberryPiles.LoadBoard(); //hopefully i don't have to send enables for true.
            int x = 0;
            StrawberryPiles.PileList!.ForEach(thisPile =>
            {
                x++;
                thisPile.IsEnabled = true;
                if (x == 1)
                {
                    thisPile.Text = "S. Limit";
                }
                else if (x == 2)
                {
                    thisPile.Text = "S. Go";
                }
                else if (x == 3)
                {
                    thisPile.Text = "S #.";
                    if (PlayerCategory != EnumPlayerCategory.Self)
                        thisPile.IsEnabled = false;
                }
                else
                {
                    throw new BasicBlankException("There should only be 3 piles for strawberries");
                }
            });
            ChocolatePiles = new BasicMultiplePilesCP<MilkRunCardInformation>(_gameContainer.Command, _mainGame.Aggregator);
            ChocolatePiles.PileClickedAsync += ChocolatePiles_PileClickedAsync;
            x = 0;
            ChocolatePiles.Rows = 1;
            ChocolatePiles.Columns = 3;
            ChocolatePiles.HasText = true;
            ChocolatePiles.HasFrame = true;
            ChocolatePiles.Style = BasicMultiplePilesCP<MilkRunCardInformation>.EnumStyleList.HasList;
            ChocolatePiles.LoadBoard();
            ChocolatePiles.PileList!.ForEach(thisPile =>
            {
                x++;
                thisPile.IsEnabled = true;
                if (x == 1)
                {
                    thisPile.Text = "C. Limit";
                }
                else if (x == 2)
                {
                    thisPile.Text = "C. Go";
                }
                else if (x == 3)
                {
                    thisPile.Text = "C #.";
                    if (PlayerCategory != EnumPlayerCategory.Self)
                        thisPile.IsEnabled = false;
                }
                else
                {
                    throw new BasicBlankException("There should only be 3 piles for chocolate");
                }
            });
            _thisE = mainGame.MainContainer.Resolve<EventAggregator>();
        }
        private async Task ChocolatePiles_PileClickedAsync(int index, BasicPileInfo<MilkRunCardInformation> mainPile)
        {
            PileInfo thisPile = new PileInfo();
            thisPile.Milk = EnumMilkType.Chocolate;
            thisPile.Pile = index + 1.ToEnum<EnumPileType>();
            await _mainGame!.PlayerPileClickedAsync(this, thisPile);
        }
        private async Task StrawberryPiles_PileClickedAsync(int index, BasicPileInfo<MilkRunCardInformation> mainPile)
        {
            PileInfo thisPile = new PileInfo();
            thisPile.Milk = EnumMilkType.Strawberry;
            thisPile.Pile = index + 1.ToEnum<EnumPileType>();
            await _mainGame!.PlayerPileClickedAsync(this, thisPile);
        }
        public DeckObservableDict<MilkRunCardInformation> GetPileCardList()
        {
            DeckObservableDict<MilkRunCardInformation> output = new DeckObservableDict<MilkRunCardInformation>();
            int x;
            MilkRunCardInformation thisCard;
            for (x = 1; x <= 2; x++)
            {
                if (StrawberryPiles!.HasCard(x) == true)
                {
                    thisCard = StrawberryPiles.GetLastCard(x);
                    output.Add(thisCard);
                }
                if (ChocolatePiles!.HasCard(x) == true)
                {
                    thisCard = ChocolatePiles.GetLastCard(x);
                    output.Add(thisCard);
                }
            }
            return output;
        }
        public void ClearBoard()
        {
            ChocolatePiles!.ClearBoard();
            StrawberryPiles!.ClearBoard();
            ChocolateDeliveries = 0;
            StrawberryDeliveries = 0;
        }
        public void LoadSavedData()
        {
            ChocolatePiles!.PileList!.ReplaceRange(ChocolateSavedList);
            StrawberryPiles!.PileList!.ReplaceRange(StrawberrySavedList);
            int x = 0;
            StrawberryPiles.PileList.ForEach(thisPile =>
            {
                x++;
                thisPile.IsEnabled = true;
                if (x == 3 && PlayerCategory != EnumPlayerCategory.Self)
                    thisPile.IsEnabled = false;
            });
            x = 0;
            ChocolatePiles.PileList.ForEach(thisPile =>
            {
                x++;
                thisPile.IsEnabled = true;
                if (x == 3 && PlayerCategory != EnumPlayerCategory.Self)
                    thisPile.IsEnabled = false;
            });
        }
        public void SavePileData()
        {
            ChocolateSavedList = ChocolatePiles!.PileList!.ToCustomBasicList();
            StrawberrySavedList = StrawberryPiles!.PileList!.ToCustomBasicList();
        }
        public bool HasCard(EnumMilkType milk)
        {
            if (milk == EnumMilkType.Chocolate)
                return ChocolatePiles!.HasCard(2);
            return StrawberryPiles!.HasCard(2);
        }
        public int LastDelivery(EnumMilkType milk)
        {
            MilkRunCardInformation thisCard;
            if (milk == EnumMilkType.Chocolate)
            {
                if (ChocolatePiles!.HasCard(2) == false)
                    return 0;
                thisCard = ChocolatePiles.GetLastCard(2);
                return thisCard.Points;
            }
            if (milk == EnumMilkType.Strawberry)
            {
                if (StrawberryPiles!.HasCard(2) == false)
                    return 0;
                thisCard = StrawberryPiles.GetLastCard(2);
                return thisCard.Points;
            }
            throw new BasicBlankException("Must be chocolate or strawberry");
        }
        public bool HasGo(EnumMilkType milk)
        {
            MilkRunCardInformation thisCard;
            if (milk == EnumMilkType.Chocolate)
            {
                if (ChocolatePiles!.HasCard(1) == false)
                    return false;
                thisCard = ChocolatePiles.GetLastCard(1);
                return thisCard.CardCategory == EnumCardCategory.Go;
            }
            if (milk == EnumMilkType.Strawberry)
            {
                if (StrawberryPiles!.HasCard(1) == false)
                    return false;
                thisCard = StrawberryPiles.GetLastCard(1);
                return thisCard.CardCategory == EnumCardCategory.Go;
            }
            throw new BasicBlankException("Must be chocolate or strawberry");
        }
        public void StealCard(EnumMilkType milk, out int deck)
        {
            MilkRunCardInformation thisCard;
            if (milk == EnumMilkType.Chocolate)
            {
                if (ChocolatePiles!.HasCard(2) == false)
                    throw new BasicBlankException("Cannot steal a chocolate card because there are none");
                thisCard = ChocolatePiles.GetLastCard(2);
                deck = thisCard.Deck;
                ChocolateDeliveries -= thisCard.Points;
                ChocolatePiles.RemoveCardFromPile(2);
                return;
            }
            if (milk == EnumMilkType.Strawberry)
            {
                if (StrawberryPiles!.HasCard(2) == false)
                    throw new BasicBlankException("Cannot steal a chocolate card because there are none");
                thisCard = StrawberryPiles.GetLastCard(2);
                StrawberryDeliveries -= thisCard.Points;
                StrawberryPiles.RemoveCardFromPile(2);
                deck = thisCard.Deck;
                return;
            }
            throw new BasicBlankException("Must be chocolate or strawberry");
        }
        public void AddToDeliveries(int deck, EnumMilkType milk)
        {
            TempAddCard(deck, milk, 3);
        }
        private void TempAddCard(int deck, EnumMilkType milk, int pile)
        {
            var thisCard = _gameContainer!.DeckList!.GetSpecificItem(deck);
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            if (milk != EnumMilkType.Chocolate && milk != EnumMilkType.Strawberry)
                throw new BasicBlankException("Can only add to strawberry or chocolcate at the beginning of adding card");
            if (pile == 3 || pile == 1)
            {
                if (thisCard.CardCategory != EnumCardCategory.Points)
                    throw new BasicBlankException("Must Add Points Only For Deliveries Or Limits");
            }
            else if (pile == 2)
            {
                if (thisCard.CardCategory == EnumCardCategory.Points)
                    throw new BasicBlankException("Cannot play points to the stop piles");
            }
            else
            {
                throw new BasicBlankException("The piles must be 1 to 3");
            }
            if (milk == EnumMilkType.Strawberry)
            {
                if (pile == 3)
                    StrawberryDeliveries += thisCard.Points;
                StrawberryPiles!.AddCardToPile(pile - 1, thisCard);
                return;
            }
            if (milk == EnumMilkType.Chocolate)
            {
                if (pile == 3)
                    ChocolateDeliveries += thisCard.Points;
                ChocolatePiles!.AddCardToPile(pile - 1, thisCard);
                return;
            }
            throw new BasicBlankException("Can only add cards to chocolate or strawberry");
        }
        public void AddLimit(int deck, EnumMilkType milk)
        {
            TempAddCard(deck, milk, 1);
        }
        public void AddGo(int deck, EnumMilkType milk)
        {
            TempAddCard(deck, milk, 2);
        }
        public int DeliveryLimit(EnumMilkType milk)
        {
            MilkRunCardInformation thisCard;
            if (milk == EnumMilkType.Strawberry)
            {
                if (StrawberryPiles!.HasCard(0) == false)
                    return 0;
                thisCard = StrawberryPiles.GetLastCard(0);
                return thisCard.Points;
            }
            if (milk == EnumMilkType.Chocolate)
            {
                if (ChocolatePiles!.HasCard(0) == false)
                    return 0;
                thisCard = ChocolatePiles.GetLastCard(0);
                return thisCard.Points;
            }
            throw new BasicBlankException("Must be strawberry of chocolate");
        }
        public async Task AnimatePlayAsync(MilkRunCardInformation thisCard, EnumMilkType milk, EnumPileType pile, EnumAnimcationDirection direction)
        {
            BasicPileInfo<MilkRunCardInformation> tempPile;
            if (milk == EnumMilkType.Strawberry)
                tempPile = StrawberryPiles!.PileList![(int)pile - 1];
            else
                tempPile = ChocolatePiles!.PileList![(int)pile - 1];
            await _thisE!.AnimateCardAsync(thisCard, direction, $"{milk}{NickName}", tempPile);
        }
    }
}
