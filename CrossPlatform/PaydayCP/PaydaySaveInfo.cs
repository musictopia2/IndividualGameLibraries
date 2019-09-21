using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace PaydayCP
{
    [SingletonGame]
    public class PaydaySaveInfo : BasicSavedBoardDiceGameClass<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, PaydayPlayerItem>
    {
        public int NumberHighlighted { get; set; } // i think this needs to be saved
        public decimal LotteryAmount { get; set; }
        public int MaxMonths { get; set; }
        public DealCard? YardSaleDealCard { get; set; }
        public DeckRegularDict<MailCard> MailListLeft { get; set; } = new DeckRegularDict<MailCard>();
        public DeckRegularDict<DealCard> DealListLeft { get; set; } = new DeckRegularDict<DealCard>();
        public DeckRegularDict<CardInformation> OutCards { get; set; } = new DeckRegularDict<CardInformation>();
        private EnumStatus _GameStatus;
        public EnumStatus GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                    {
                        _thisMod.GameStatus = value;
                        FigureOutUI();
                    }
                }
            }
        }
        public int RemainingMove { get; set; } // don't know if the other 3 has to be there.  better be safe than sorry
        public bool EndOfMonth { get; set; }
        public bool EndGame { get; set; }
        public int DiceNumber { get; set; }
        public MailCard? CurrentMail { get; set; }
        public DealCard? CurrentDeal { get; set; }
        private void FigureOutUI()
        {
            if (_thisMod!.PopUpList != null && _thisMod.CurrentMailList != null)
            {
                if (_mainGame!.DidChooseColors == false)
                {
                    _thisMod.PopUpList.Visible = false;
                    _thisMod.CurrentMailList.Visible = false;
                    return;
                }
                if (GameStatus == EnumStatus.ChoosePlayer || GameStatus == EnumStatus.DealOrBuy || GameStatus == EnumStatus.ChooseLottery || GameStatus == EnumStatus.ChooseDeal)
                {
                    _thisMod.PopUpList.Visible = true;
                    _thisMod.CurrentMailList.Visible = false;
                }
                else
                {
                    _thisMod.PopUpList.Visible = false;
                    _thisMod.CurrentMailList.Visible = true;
                }
            }
        }
        private PaydayViewModel? _thisMod; //this time you need a view model.
        private PaydayMainGameClass? _mainGame;
        public void LoadMod(PaydayViewModel thisMod, PaydayMainGameClass mainGame)
        {
            _thisMod = thisMod;
            thisMod.GameStatus = GameStatus;
            _mainGame = mainGame;
            FigureOutUI();
        }
    }
}