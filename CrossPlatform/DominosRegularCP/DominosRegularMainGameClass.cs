using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Dominos;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace DominosRegularCP
{
    [SingletonGame]
    public class DominosRegularMainGameClass : DominosGameClass<SimpleDominoInfo, DominosRegularPlayerItem, DominosRegularSaveInfo>, IMiscDataNM
    {
        private bool _didPlay;
        private bool _wentOut;
        internal readonly DominosRegularViewModel ThisMod;
        public DominosRegularMainGameClass(IGamePackageResolver container) : base(container)
        {
            ThisMod = MainContainer.Resolve<DominosRegularViewModel>();
            DominosToPassOut = 6;
        }
        internal async Task DominoPileClicked(int whichOne)
        {
            int decks = ThisMod.PlayerHand1!.ObjectSelected();
            if (decks == 0)
            {
                await ThisMod.ShowGameMessageAsync($"Sorry, you have to select a domino to play for {whichOne}");
                return;
            }
            var thisDomino = SingleInfo!.MainHandList.GetSpecificItem(decks);
            if (ThisMod.GameBoard1!.IsValidMove(whichOne, thisDomino) == false)
            {
                await ThisMod.ShowGameMessageAsync("Illegal Move");
                return;
            }
            await PlayDominoAsync(decks, whichOne);
        }
        public override Task FinishGetSavedAsync()
        {
            if (IsLoaded == false)
                LoadControls();
            ProtectedLoadBone();
            ThisMod.GameBoard1!.LoadSavedGame();
            AfterPassedDominos(); //i did need this too.
            return Task.CompletedTask;
        }
        public override Task PopulateSaveRootAsync()
        {
            ProtectedSaveBone();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            LoadUpDominos();
            IsLoaded = true; //you do have to set everytime though.
        }
        public override async Task StartNewTurnAsync()
        {
            SaveRoot!.Beginnings = false; //forgot this part.
            ProtectedStartTurn();
            _didPlay = false;
            await ContinueTurnAsync(); //i think.
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.Beginnings)
            {
                await StartNewTurnAsync();
                return;
            }
            await base.ContinueTurnAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            int decks = ComputerAI.DominoToPlay(out int whichOne, this);
            if (decks > 0)
            {
                await PlayDominoAsync(decks, whichOne);
                return;
            }
            if (ThisMod.BoneYard!.HasBone() == false || ThisMod.BoneYard.HasDrawn())
            {
                _wentOut = false;
                if (SingleInfo!.CanSendMessage(ThisData!))
                    await ThisNet!.SendEndTurnAsync();
                await EndTurnAsync();
                return;
            }
            await DrawDominoAsync(ThisMod.BoneYard.DrawDomino());
        }
        public override async Task SetUpGameAsync(bool IsBeginning)
        {
            if (IsBeginning == true)
                LoadControls();
            SaveRoot!.Beginnings = true;
            ClearBoneYard();
            PassDominos();
            ThisMod.GameBoard1!.ClearBoard();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.NoPlay = false;
            });
            var (turn, dominoused) = PrivateWhoStartsFirst();
            WhoTurn = turn;
            SingleInfo = PlayerList.GetWhoPlayer();
            SingleInfo.MainHandList.RemoveObjectByDeck(dominoused.Deck);
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            ThisMod.GameBoard1.PopulateCenter(dominoused);
            await ThisLoader!.FinishUpAsync(IsBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                //put in cases here.
                case "play":
                    PlayInfo thisPlay = await js.DeserializeObjectAsync<PlayInfo>(content);
                    await PlayDominoAsync(thisPlay.Deck, thisPlay.WhichOne);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public async Task PlayDominoAsync(int deck, int whichOne)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
            {
                PlayInfo thisPlay = new PlayInfo();
                thisPlay.Deck = deck;
                thisPlay.WhichOne = whichOne;
                await ThisNet!.SendAllAsync("play", thisPlay);
            }
            SimpleDominoInfo thisDomino = SingleInfo!.MainHandList.GetSpecificItem(deck);
            await PlayDominoAsync(thisDomino, whichOne);
        }
        public override async Task EndTurnAsync()
        {
            if (_didPlay == false && ThisMod.BoneYard!.HasDrawn() == false)
                SingleInfo!.NoPlay = true;
            else
                SingleInfo!.NoPlay = false;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                ThisMod.PlayerHand1!.EndTurn();
            if (PlayerList.All(items => items.NoPlay))
            {
                _wentOut = false;
                await GameOverAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        private async Task GameOverAsync()
        {
            Scoring();
            if (_wentOut == false)
                SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
            await ShowWinAsync();
        }
        public async Task PlayDominoAsync(SimpleDominoInfo thisDomino, int whichOne)
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(thisDomino.Deck);
            _didPlay = true;
            ThisMod.GameBoard1!.MakeMove(whichOne, thisDomino);
            if (SingleInfo.MainHandList.Count == 0)
            {
                _wentOut = true;
                await GameOverAsync();
                return;
            }
            await EndTurnAsync();
        }
        public override Task PlayDominoAsync(int deck)
        {
            throw new BasicBlankException("This game has an exception.  Must use the one with 2 parameters unfortunately");
        }
        private void Scoring()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore = thisPlayer.MainHandList.Sum(items => items.Points));
        }
        private (int turn, SimpleDominoInfo dominoused) PrivateWhoStartsFirst()
        {
            var thisInfo = GetHighestDouble();
            if (thisInfo.turn > 0)
                return thisInfo;
            return GetMostPoints();
        }

        private (int turn, SimpleDominoInfo dominoused) GetHighestDouble()
        {
            int highs = -1;
            SimpleDominoInfo? highDomino = null;
            int x = 0;
            int whichs = 0;
            int currents;
            PlayerList!.ForEach(thisPlayer =>
            {
                x++;
                thisPlayer.MainHandList.ForConditionalItems(items => items.FirstNum == items.SecondNum, thisDomino =>
                {
                    currents = thisDomino.FirstNum;
                    if (currents > highs)
                    {
                        highs = currents;
                        whichs = x;
                        highDomino = thisDomino;
                    }
                });
            });
            return (whichs!, highDomino!);
        }
        private (int turn, SimpleDominoInfo dominoused) GetMostPoints()
        {
            int highs = -1;
            SimpleDominoInfo? highDomino = null;
            int x = 0;
            int whichs = 0;
            int currents;
            PlayerList!.ForEach(thisPlayer =>
            {
                x++;
                thisPlayer.MainHandList.ForEach(thisDomino =>
                {
                    currents = thisDomino.FirstNum + thisDomino.SecondNum;
                    if (currents > highs)
                    {
                        highs = currents;
                        whichs = x;
                        highDomino = thisDomino;
                    }
                });
            });
            return (whichs!, highDomino!);
        }
    }
}