using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplePilesViewModels;
using BasicGameFramework.SpecializedGameTypes.StockClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static SkipboCP.GlobalConstants;
namespace SkipboCP
{
    public class SkipboPlayerItem : PlayerSingleHand<SkipboCardInformation>
    { //anything needed is here
        [JsonIgnore]
        public DiscardPilesVM<SkipboCardInformation>? DiscardPiles;

        public CustomBasicList<BasicPileInfo<SkipboCardInformation>> DiscardList { get; set; } = new CustomBasicList<BasicPileInfo<SkipboCardInformation>>();

        [JsonIgnore]
        public StockViewModel? StockPile;

        public CustomBasicList<BasicPileInfo<SkipboCardInformation>> StockList { get; set; } = new CustomBasicList<BasicPileInfo<SkipboCardInformation>>();

        private string _InStock = "";

        public string InStock
        {
            get { return _InStock; }
            set
            {
                if (SetProperty(ref _InStock, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _StockLeft;

        public int StockLeft
        {
            get { return _StockLeft; }
            set
            {
                if (SetProperty(ref _StockLeft, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _Discard1 = "";
        public string Discard1
        {
            get
            {
                return _Discard1; // for now, can do copy.  may later do something else (?)
            }

            set
            {
                if (SetProperty(ref _Discard1, value) == true)
                {
                }
            }
        }

        private string _Discard2 = "";
        public string Discard2
        {
            get
            {
                return _Discard2;
            }

            set
            {
                if (SetProperty(ref _Discard2, value) == true)
                {
                }
            }
        }

        private string _Discard3 = "";
        public string Discard3
        {
            get
            {
                return _Discard3;
            }

            set
            {
                if (SetProperty(ref _Discard3, value) == true)
                {
                }
            }
        }

        private string _Discard4 = "";
        public string Discard4
        {
            get
            {
                return _Discard4;
            }

            set
            {
                if (SetProperty(ref _Discard4, value) == true)
                {
                }
            }
        }
        //this is used so i can copy/paste to flinch.
        private string _Discard5 = "";

        public string Discard5
        {
            get { return _Discard5; }
            set
            {
                if (SetProperty(ref _Discard5, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private SkipboViewModel? _thisMod;
        private SkipboMainGameClass? _mainGame;
        private EventAggregator? _thisE;
        public void LoadPlayerPiles(SkipboMainGameClass mainGame)
        {
            _mainGame = mainGame; //even if doing autoresume, needs this.
            _thisMod = mainGame.MainContainer.Resolve<SkipboViewModel>();
            _thisE = mainGame.MainContainer.Resolve<EventAggregator>();
            StockPile = new StockViewModel(_thisMod);
            StockPile.StockClickedAsync += StockPile_StockClickedAsync;
            DiscardPiles = new DiscardPilesVM<SkipboCardInformation>(_thisMod);
            DiscardPiles.PileClickedAsync += DiscardPiles_PileClickedAsync;
            DiscardPiles.Init(HowManyDiscards);
            DiscardPiles.Visible = false;
            StockPile.StockFrame.Visible = false;
            DiscardPiles.IsEnabled = false; //i think
            StockPile.StockFrame.IsEnabled = false;
        }
        private async Task DiscardPiles_PileClickedAsync(int index, BasicPileInfo<SkipboCardInformation> thisPile)
        {
            int playerDeck = _thisMod!.PlayerHand1!.ObjectSelected();
            if (playerDeck > 0)
            {
                await _mainGame!.AddToDiscardAsync(index, playerDeck);
                return;
            }
            if (DiscardPiles!.HasCard(index) == false)
                return;
            if (DiscardPiles.PileList![index].IsSelected == true)
            {
                DiscardPiles.PileList[index].IsSelected = false;
                return;
            }
            _mainGame!.UnselectAllCards();
            DiscardPiles.SelectUnselectSinglePile(index);
        }
        private Task StockPile_StockClickedAsync()
        {
            int nums = StockPile!.CardSelected();
            _mainGame!.UnselectAllCards();
            if (nums > 0)
            {
                StockPile.UnselectCard();
                return Task.CompletedTask;
            }
            StockPile.SelectCard();
            return Task.CompletedTask;
        }
        public async Task AnimateDiscardAsync(SkipboCardInformation thisCard, int Pile, EnumAnimcationDirection direction)
        {
            await _thisE!.AnimateCardAsync(thisCard, direction, $"discard{NickName}", DiscardPiles!.PileList![Pile]);
        }
        public async Task AnimateStockAsync(SkipboCardInformation thisCard)
        {
            var thisPile = StockPile!.StockFrame.PileList.Single();
            await _thisE!.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartCardToUp, $"stock{NickName}", thisPile);
        }
    }
}