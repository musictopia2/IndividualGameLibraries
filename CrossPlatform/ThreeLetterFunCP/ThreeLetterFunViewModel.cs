using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ThreeLetterFunCP
{
    public class ThreeLetterFunViewModel : BasicMultiplayerVM<ThreeLetterFunPlayerItem, ThreeLetterFunMainGameClass>
    {
        public ThreeLetterFunViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public TileBoardViewModel? TileBoard1; //has to be public so it can hook up into the ui.

        public INewCard? NewUI;

        GlobalHelpers? _thisGlobal;
        private string _PlayerWon = "";

        public string PlayerWon
        {
            get { return _PlayerWon; }
            set
            {
                if (SetProperty(ref _PlayerWon, value))
                {
                    //can decide what to do when property changes
                    CalculateVisible();
                }

            }
        }

        private ThreeLetterFunCardData? _CurrentCard;

        public ThreeLetterFunCardData? CurrentCard
        {
            get { return _CurrentCard; }
            set
            {
                if (SetProperty(ref _CurrentCard, value))
                {
                    //can decide what to do when property changes
                    CalculateVisible();
                    if (NewUI != null)
                        NewUI.ShowNewCard();
                }

            }
        }
        internal void CalculateVisible()
        {
            if (CurrentCard != null)
            {
                _thisGlobal!.GameBoard1!.Visible = false;
                return;
            }
            if (PlayerWon != "")
            {
                _thisGlobal!.GameBoard1!.Visible = false;
                return;
            }
            _thisGlobal!.GameBoard1!.Visible = true;

        }

        public BasicGameCommand? PlayCommand { get; set; }
        public BasicGameCommand? GiveUpCommand { get; set; }
        public BasicGameCommand? TakeBackCommand { get; set; }

        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            _thisGlobal = MainContainer!.Resolve<GlobalHelpers>();
            _thisGlobal.PopulateWords(); //i think this too.
            PlayCommand = new BasicGameCommand(this, async items =>
            {
                _thisGlobal.PauseContinueTimer();
                if (ThisData!.MultiPlayer == true && MainGame!.ThisCheck!.IsEnabled == true)
                {
                    await ShowGameMessageAsync("Should not have enabled the network helpers since you had to take your turn.");
                    return;
                }
                var thisCard = _thisGlobal.GameBoard1!.GetCompletedCard();
                if (thisCard == null)
                {
                    _thisGlobal.PauseContinueTimer();
                    await ShowGameMessageAsync("You must pick the tiles before playing");
                    return;
                }
                if (thisCard.IsValidWord() == false)
                {
                    var thisWord = thisCard.GetWord();
                    if (MainGame!.SaveRoot!.Level == EnumLevel.Easy)
                    {

                        _thisGlobal.GameBoard1.UnDo();
                        await ShowGameMessageAsync($"{thisWord} is not a word or is too hard. Please try again");
                        _thisGlobal.PauseContinueTimer();
                        return;
                    }
                    if (ThisData.MultiPlayer == false)
                    {
                        await ShowGameMessageAsync($"{thisWord} does not exist.  Therefore; its going to the next one");
                        await SelfGiveUpAsync(true);
                        return;
                    }
                    await ShowGameMessageAsync($"{thisWord} does not exist.  Therefore; waiting for other players to decide if they have a word");
                    await SelfGiveUpAsync(true);
                    return;
                }
                if (ThisData.MultiPlayer == true)
                {
                    MainGame!.SingleInfo = MainGame.PlayerList!.GetSelf();
                    TempWord thisWord = new TempWord();
                    thisWord.Player = MainGame.SingleInfo.Id;
                    thisWord.CardUsed = thisCard.Deck;
                    thisWord.TileList = MainGame.SingleInfo.TileList;
                    if (thisWord.TileList.Count == 0)
                        throw new BasicBlankException("Must have tiles to form a word to send");
                    MainGame.SingleInfo.TimeToGetWord = (int)_thisGlobal.Stops!.TimeTaken();
                    _thisGlobal.Stops.ManualStop(false);
                    thisWord.TimeToGetWord = MainGame.SingleInfo.TimeToGetWord;
                    if (MainGame.SingleInfo.TimeToGetWord == 0)
                        throw new BasicBlankException("Time Taken Cannot Be 0");
                    await ThisNet!.SendAllAsync("playword", thisWord);
                    MainGame.SaveRoot!.PlayOrder.WhoTurn = MainGame.SingleInfo.Id;
                }
                await MainGame!.PlayWordAsync(thisCard.Deck);
            }, Items => true, this, CommandContainer!);
            GiveUpCommand = new BasicGameCommand(this, async Items =>
            {
                await SelfGiveUpAsync(true);
            }, Items => true, this, CommandContainer!);
            TakeBackCommand = new BasicGameCommand(this, Items =>
            {
                _thisGlobal.GameBoard1!.UnDo(); //hopefully this is it.
            }, Items => true, this, CommandContainer!);
        }
        public async Task SelfGiveUpAsync(bool doStop)
        {
            if (ThisData!.MultiPlayer == true)
            {
                if (doStop == true)
                    _thisGlobal!.Stops!.ManualStop(false);
                //hopefully no need for custom runtime error message.
                ThreeLetterFunPlayerItem tempPlayer = MainGame!.PlayerList!.GetSelf();
                await ThisNet!.SendAllAsync("giveup", tempPlayer.Id);
                MainGame.SaveRoot!.PlayOrder.WhoTurn = tempPlayer.Id; //hopefully this works (?)
            }
            await MainGame!.GiveUpAsync();
        }
    }
}