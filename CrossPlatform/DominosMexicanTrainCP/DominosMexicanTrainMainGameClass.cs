using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Animations;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace DominosMexicanTrainCP
{
    [SingletonGame]
    public class DominosMexicanTrainMainGameClass : DominosGameClass<MexicanDomino, DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>, IMiscDataNM, IStartNewGame
    {
        internal readonly DominosMexicanTrainViewModel ThisMod;
        private GlobalClass? _thisGlobal;
        public DominosMexicanTrainMainGameClass(IGamePackageResolver container) : base(container)
        {
            ThisMod = MainContainer.Resolve<DominosMexicanTrainViewModel>();
            DominosToPassOut = 12;
        }
        internal async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
            await ShowWinAsync();
        }
        public override async Task FinishGetSavedAsync()
        {
            if (IsLoaded == false)
                LoadControls();
            ProtectedLoadBone();
            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.LongestTrainList.Count > 0)
                {
                    DeckObservableDict<MexicanDomino> tempList = new DeckObservableDict<MexicanDomino>();
                    ThisMod.BoneYard!.LoadPlayerPieces(thisPlayer.LongestTrainList, ref tempList, false);
                    thisPlayer.MainHandList.AddRange(tempList);
                    thisPlayer.LongestTrainList.Clear();
                }
                thisPlayer.MainHandList.ForEach(thisDomino =>
                {
                    thisDomino.Drew = false;
                    thisDomino.IsSelected = false;
                });
            });
            SingleInfo = PlayerList.GetSelf();
            ThisMod.TrainStation1!.Self = SingleInfo.Id;
            ThisMod.PrivateTrain1!.ClearHand();
            ThisMod.UpdateCount();
            SingleInfo.MainHandList.Sort();
            ThisE.Subscribe(SingleInfo); //i think
            SaveRoot!.LoadMod(ThisMod);
            await ThisMod.TrainStation1.LoadSavedDataAsync();
            SingleInfo = PlayerList.GetWhoPlayer();
            AfterPassedDominos(); //i did need this too.
        }
        private void FirstLoad()
        {
            int highest;
            if (PlayerList.Count() < 5)
                highest = 12;
            else
                highest = 15;
            ThisMod.TrainStation1!.Self = PlayerList!.GetSelf().Id;
            ThisMod.TrainStation1.LoadPlayers(highest);
        }
        private void LoadControls()
        {
            _thisGlobal = MainContainer.Resolve<GlobalClass>();
            _thisGlobal.Init(this);
            LoadUpDominos();
            FirstLoad();
            _thisGlobal.Animates = new AnimateSkiaSharpGameBoard();
            _thisGlobal.Animates.LongestTravelTime = 150;
            ThisMod.AfterLoadControls();
            IsLoaded = true; //you do have to set everytime though.
        }
        public override Task PopulateSaveRootAsync()
        {
            ProtectedSaveBone();
            ThisMod.TrainStation1!.SavedData();
            return Task.CompletedTask;
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            if (isBeginning == true)
                LoadControls();
            SaveRoot!.LoadMod(ThisMod);
            ThisMod.PrivateTrain1!.HandList.Clear(); //i think
            SaveRoot.FirstPlayerPlayed = 0;
            SaveRoot.ImmediatelyStartTurn = true;
            ClearBoneYard();
            if (isBeginning == false)
                ThisMod.TrainStation1!.NewRound();
            else
                ThisMod.TrainStation1!.StartRound();
            PassDominos();
            if (isBeginning)
            {
                SingleInfo = PlayerList!.GetSelf();
                SingleInfo.MainHandList.Sort();
                ThisE.Subscribe(SingleInfo); //i think
            }
            PlayerList!.ForEach(thisPlayer => thisPlayer.LongestTrainList.Clear()); //just in case.
            SingleInfo = PlayerList.GetSelf();
            ThisMod.UpdateCount(); //try this too.
            SingleInfo = PlayerList.GetWhoPlayer();
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "dominoplayed":
                    SendPlay output = await js.DeserializeObjectAsync<SendPlay>(content);
                    MexicanDomino thisDomino = SingleInfo!.MainHandList.GetSpecificItem(output.Deck);
                    SingleInfo.MainHandList.RemoveObjectByDeck(output.Deck);
                    await ThisMod.TrainStation1!.AnimateShowSelectedDominoAsync(output.Section, thisDomino);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public async Task EndTurnAsync(bool didPlay)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                ThisMod.PlayerHand1!.EndTurn();
                ThisMod.PrivateTrain1!.EndTurn();
            }
            if (didPlay == false)
                ThisMod.TrainStation1!.PutTrain(WhoTurn);
            if (await CanEndRoundAsync(didPlay))
            {
                await EndRoundAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            SaveRoot!.ImmediatelyStartTurn = false; //otherwise, will give overflow error.
            ProtectedStartTurn();
            SingleInfo = PlayerList!.GetWhoPlayer();
            SaveRoot.CurrentPlayerDouble = false;
            ThisE.RepaintBoard();
            await ContinueTurnAsync();
        }
        private async Task<bool> CanEndRoundAsync(bool didPlay)
        {
            if (ThisTest!.DoubleCheck)
                return true;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.ObjectCount == 0)
                return true;
            if (didPlay)
            {
                SaveRoot!.FirstPlayerPlayed = 0;
                return false;
            }
            if (SaveRoot!.FirstPlayerPlayed == 0)
            {
                if (ThisMod.TrainStation1!.CanFillPrevious())
                    SaveRoot.FirstPlayerPlayed = WhoTurn;
                return false;
            }
            if (ThisMod.TrainStation1!.CanEndEarly() == false)
            {
                SaveRoot.FirstPlayerPlayed = 0;
                return false;
            }
            int newTurn = await PlayerList.CalculateWhoTurnAsync();
            return newTurn == SaveRoot.FirstPlayerPlayed;
        }
        private int CalculatePoints(DominosMexicanTrainPlayerItem thisPlayer)
        {
            DeckRegularDict<MexicanDomino> tempList = thisPlayer.MainHandList.ToRegularDeckDict();
            tempList.AddRange(thisPlayer.LongestTrainList);
            return tempList.Sum(items => items.Points);
        }
        private async Task EndRoundAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PreviousLeft = thisPlayer.ObjectCount;
                thisPlayer.PreviousScore = CalculatePoints(thisPlayer);
                thisPlayer.TotalScore += thisPlayer.PreviousScore;
            });
            await ThisMod.TrainStation1!.EndRoundAsync();
        }
        public override async Task EndTurnAsync()
        {
            await EndTurnAsync(false);
        }
        public override Task PlayDominoAsync(int Deck)
        {
            throw new BasicBlankException("I don't think we can run it this time.");
        }
        public bool ForceSatisfy()
        {
            if (ThisMod.TrainStation1!.NeedDouble(out int numberNeeded) == false)
                return false;
            if (SaveRoot!.CurrentPlayerDouble == true)
                return false;
            if (numberNeeded < 0)
                throw new BasicBlankException("The number neeed has to at least be 0");
            foreach (var thisPlayer in PlayerList!)
            {
                if (thisPlayer.ObjectCount < 5 && WhoTurn != thisPlayer.Id)
                    return false;
            }
            DeckRegularDict<MexicanDomino> tempList = SingleInfo!.MainHandList.ToRegularDeckDict();
            tempList.AddRange(SingleInfo.LongestTrainList);
            return tempList.Any(items => items.FirstNum == numberNeeded || items.SecondNum == numberNeeded);
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0;
                thisPlayer.PreviousScore = 0;
                thisPlayer.PreviousLeft = 0;
            });
            SaveRoot!.UpTo = 12;
            return Task.CompletedTask;
        }
    }
}