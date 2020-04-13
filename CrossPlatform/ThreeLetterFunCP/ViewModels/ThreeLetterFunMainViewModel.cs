using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.Logic;
namespace ThreeLetterFunCP.ViewModels
{
    [InstanceGame]
    public class ThreeLetterFunMainViewModel : BasicMultiplayerMainVM
    {
        private readonly ThreeLetterFunMainGameClass _mainGame; //if we don't need, delete.
        private readonly BasicData _basicData;
        private readonly GiveUpClass _giveUp;
        private readonly GameBoard _board; //if something needs it, has to decide which one will show it.
        private readonly GlobalHelpers _global;

        public ThreeLetterFunMainViewModel(CommandContainer commandContainer,
            ThreeLetterFunMainGameClass mainGame,
            IViewModelData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            GiveUpClass giveUp,
            GameBoard board,
            GlobalHelpers global
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _basicData = basicData;
            _giveUp = giveUp;
            _board = board;
            _global = global;
        }
        //anything else needed is here.
        //for version 1, only focus on the single player parts.
        [Command(EnumCommandCategory.Game)]
        public async Task PlayAsync()
        {
            _global.PauseContinueTimer();
            if (_basicData.MultiPlayer == true && _mainGame!.Check!.IsEnabled == true)
            {
                await UIPlatform.ShowMessageAsync("Should not have enabled the network helpers since you had to take your turn.");
                return;
            }
            var thisCard = _board.GetCompletedCard();
            if (thisCard == null)
            {
                _global.PauseContinueTimer();
                await UIPlatform.ShowMessageAsync("You must pick the tiles before playing");
                return;
            }
            if (thisCard.IsValidWord() == false)
            {
                var thisWord = thisCard.GetWord();
                if (_mainGame.SaveRoot!.Level == EnumLevel.Easy)
                {

                    _board.UnDo();
                    await UIPlatform.ShowMessageAsync($"{thisWord} is not a word or is too hard. Please try again");
                    _global.PauseContinueTimer();
                    return;
                }
                if (_basicData.MultiPlayer == false)
                {
                    await UIPlatform.ShowMessageAsync($"{thisWord} does not exist.  Therefore; its going to the next one");
                    await _giveUp.SelfGiveUpAsync(true);
                    return;
                }
                await UIPlatform.ShowMessageAsync($"{thisWord} does not exist.  Therefore; waiting for other players to decide if they have a word");
                await _giveUp.SelfGiveUpAsync(true);
                return;
            }
            if (_basicData.MultiPlayer == true)
            {
                _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
                TempWord thisWord = new TempWord();
                thisWord.Player = _mainGame.SingleInfo.Id;
                thisWord.CardUsed = thisCard.Deck;
                thisWord.TileList = _mainGame.SingleInfo.TileList;
                if (thisWord.TileList.Count == 0)
                    throw new BasicBlankException("Must have tiles to form a word to send");
                _mainGame.SingleInfo.TimeToGetWord = (int)_global.Stops!.TimeTaken();
                _global.Stops.ManualStop(false);
                thisWord.TimeToGetWord = _mainGame.SingleInfo.TimeToGetWord;
                if (_mainGame.SingleInfo.TimeToGetWord == 0)
                    throw new BasicBlankException("Time Taken Cannot Be 0");
                await _mainGame.Network!.SendAllAsync("playword", thisWord);
                _mainGame.SaveRoot!.PlayOrder.WhoTurn = _mainGame.SingleInfo.Id;
            }
            await _mainGame!.PlayWordAsync(thisCard.Deck);
        }
        [Command(EnumCommandCategory.Game)]
        public async Task GiveUpAsync()
        {
            await _giveUp.SelfGiveUpAsync(true);
        }
        [Command(EnumCommandCategory.Game)]
        public void TakeBack()
        {
            _board.UnDo();
        }

        private string _playerWon = "";
        [VM]
        public string PlayerWon
        {
            get { return _playerWon; }
            set
            {
                if (SetProperty(ref _playerWon, value))
                {
                    CalculateVisible();
                }

            }
        }

        private ThreeLetterFunCardData? _currentCard;
        [VM]
        public ThreeLetterFunCardData? CurrentCard
        {
            get { return _currentCard; }
            set
            {
                if (SetProperty(ref _currentCard, value))
                {
                    CalculateVisible();
                }

            }
        }

        private void CalculateVisible()
        {
            if (CurrentCard != null)
            {
                _board.Visible = false;
                return;
            }
            if (PlayerWon != "")
            {
                _board.Visible = false;
                return;
            }
            _board.Visible = true;

        }
    }
}