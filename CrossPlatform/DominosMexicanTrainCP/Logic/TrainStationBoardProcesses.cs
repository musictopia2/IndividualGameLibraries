using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using DominosMexicanTrainCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static SkiaSharpGeneralLibrary.SKExtensions.RotateExtensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
namespace DominosMexicanTrainCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class TrainStationBoardProcesses
    {
        private int NewestTrain
        {
            get
            {
                return _saveRoot!.NewestTrain;
            }
            set
            {
                _saveRoot!.NewestTrain = value;
            }
        }
        public int UpTo
        {
            get
            {
                return _saveRoot!.UpTo;
            }
            set
            {
                _saveRoot!.UpTo = value;
            }
        }
        private readonly IEventAggregator _thisE;
        private readonly GlobalClass _thisGlobal;
        private readonly BasicData _basicData;
        private readonly IAsyncDelayer _delayer;
        private readonly TestOptions _test;
        private readonly CommandContainer _command;
        private readonly TrainStationGraphicsCP _imageBoard; //if this causes overflow, then rethink.
        private DominosMexicanTrainSaveInfo? _saveRoot;
        public TrainStationBoardProcesses(IEventAggregator thisE,
            GlobalClass thisGlobal,
            BasicData basicData,
            IAsyncDelayer delayer,
            TestOptions test,
            CommandContainer command,
            TrainStationGraphicsCP imageBoard //hopefully this simple.
            )
        {
            _thisE = thisE;
            _thisGlobal = thisGlobal;
            _basicData = basicData;
            _delayer = delayer;
            _test = test;
            _command = command;
            _imageBoard = imageBoard;
            thisGlobal.TrainStation1 = this; //hopefully this simple.
            GlobalClass.GetTrainClicked = TrainClicked;
        }

        public bool CanEndEarly()
        {
            if (_thisGlobal.BoneYard!.HasBone())
                return false;
            return !_thisGlobal.BoneYard.HasDrawn();
        }

        public void SavedData()
        {
            if (_saveRoot == null)
            {
                throw new BasicBlankException("Save Root was not set when getting saved data.  Rethink");
            }
            SavedTrain output = new SavedTrain();
            output.Satisfy = Satisfy;
            output.CenterDomino = CenterDomino;
            output.TrainList = TrainList;
            _saveRoot.TrainData = output;
        }
        public async Task LoadSavedDataAsync(DominosMexicanTrainSaveInfo saveRoot)
        {
            _saveRoot = saveRoot;
            if (_basicData.IsXamarinForms)
            {
                RepaintBoard();
                await _delayer.DelaySeconds(1);
            }
            SavedTrain output = _saveRoot.TrainData!;
            Satisfy = output.Satisfy;
            CenterDomino = output.CenterDomino!;
            TrainList = output.TrainList;
            int x = 0;
            //SetImage();
            foreach (var thisTrain in TrainList.Values)
            {
                x++;
                var tempList = thisTrain.DominoList.ToRegularDeckDict();
                thisTrain.DominoList.Clear();
                tempList.ForEach(thisDomino =>
                {
                    thisTrain.DominoList.Add(thisDomino);
                });
            }
            RepaintBoard();
        }
        public void LoadPlayers(int highestDomino, DominosMexicanTrainMainGameClass mainGame)
        {
            //SetImage();
            _imageBoard!.FirstLoad();
            _saveRoot = mainGame.SaveRoot; //hopefully this simple.
            LoadBoard(mainGame.PlayerList.Count());
            if (Self == 0)
                throw new BasicBlankException("Needs to know self");
            if (UpTo == -1 && _test.DoubleCheck == false)
                UpTo = highestDomino;
            else if (_test.DoubleCheck == true)
                UpTo = 0;
            NewestTrain = 0;
        }
        private void LoadBoard(int players)
        {
            if (_imageBoard!.PrivateList.Count == 0)
                throw new BasicBlankException("Sorry; there are no items on the private list.  Run FirstLoad first before loading the board");
            if (TrainList.Count > 0)
                throw new BasicBlankException("There are already items on the train list");
            if (Self > 7)
                throw new BasicBlankException("The self player has to be between 1 and 7");
            CustomBasicList<int> newList;
            if (players == 2)
            {
                newList = new CustomBasicList<int>() { 1, 3, 6 };
            }
            else if (players == 3)
            {
                newList = new CustomBasicList<int>() { 1, 3, 6, 8 };
            }
            else if (players == 4)
            {
                newList = new CustomBasicList<int>() { 1, 2, 3, 6, 8 };
            }
            else if (players == 5)
            {
                newList = new CustomBasicList<int>();
                for (var x = 1; x <= 5; x++)
                    newList.Add(x);
                newList.Add(8);
            }
            else if (players == 6)
            {
                newList = new CustomBasicList<int>();
                for (var x = 1; x <= 6; x++)
                    newList.Add(x);
                newList.Add(8);
            }
            else if (players == 7)
            {
                newList = GetIntegerList(1, 8);
            }
            else
            {
                throw new BasicBlankException("Sorry; the new list does not match.  Find out what happened");
            }

            for (var x = 1; x <= players + 1; x++)
            {
                TrainInfo thisTrain = new TrainInfo();
                thisTrain.Index = newList[x - 1];
                if (x == players + 1)
                {
                    thisTrain.TrainUp = true;
                    thisTrain.IsPublic = true;
                }
                TrainList.Add(thisTrain);
            }
            RepaintBoard();
        }
        public void StartRound()
        {
            CenterDomino = _thisGlobal.BoneYard!.FindDoubleDomino(UpTo); //i think
            RepaintBoard();
        }
        public async Task EndRoundAsync(DominosMexicanTrainMainGameClass mainGame)
        {
            UpTo--;
            if (UpTo < 0)
            {
                await mainGame.GameOverAsync();
                return;
            }
            await mainGame.RoundOverNextAsync();
        }
        private void ClearBoard()
        {
            if (TrainList.Count == 0)
                throw new BasicBlankException("There is no trains shown");
            if (TrainList.Count < 3)
                throw new BasicBlankException("There has to be at least 3 trains because 2 players plus public");
            if (TrainList.Count > 8)
                throw new BasicBlankException("The most trains can be 8 because 7 players max plus public");
            Satisfy = 0;
            int x = 0;
            foreach (var thisTrain in TrainList.Values)
            {
                x++;
                thisTrain.DominoList = new DeckRegularDict<MexicanDomino>();
                thisTrain.TrainUp = x == TrainList.Count;
            }
        }
        public void NewRound()
        {
            NewestTrain = 0;
            ClearBoard();
            StartRound();
        }
        public TrainInfo GetTrain(int index)
        {
            return TrainList[index];
        }
        public void StartTurn()
        {
            RepaintBoard();
        }

        //private void SetImage()
        //{
        //    if (_imageBoard != null)
        //        return; //has to be this way to stop the overflow errors
        //    _imageBoard = _mainGame.MainContainer.Resolve<TrainStationGraphicsCP>();
        //}

        public void RepaintBoard()
        {
            _thisE.RepaintBoard();
        }
        public Dictionary<int, TrainInfo> TrainList = new Dictionary<int, TrainInfo>();
        public int Self { get; set; }
        public MexicanDomino CenterDomino { get; set; } = new MexicanDomino();
        public int Satisfy { get; set; }
        public int TrainClicked(SKPoint point)
        {
            if (_command.IsExecuting)
            {
                return 0;
            }

            TrainStationGraphicsCP.PrivateTrain thisPrivate;
            if (Satisfy > 0)
            {
                var tempTrain = TrainList[Satisfy];
                thisPrivate = _imageBoard!.PrivateList[tempTrain.Index];
                var tempList = _imageBoard.GetClickableRectangle(thisPrivate);
                foreach (var thisRect in tempList)
                {
                    if (MiscHelpers.DidClickRectangle(thisRect, point))
                        return Satisfy;
                }
                return 0;
            }
            int x = 0;
            foreach (var thisTrain in TrainList.Values)
            {
                x++;
                thisPrivate = _imageBoard!.PrivateList[thisTrain.Index];
                if (thisTrain.TrainUp == true || x == Self)
                {
                    var tempList = _imageBoard.GetClickableRectangle(thisPrivate);
                    foreach (var thisRect in tempList)
                    {
                        if (MiscHelpers.DidClickRectangle(thisRect, point))
                            return x;
                    }
                }
            }
            return 0;
        }
        public CustomBasicList<int> FindAvailablePlays(int turn)
        {
            CustomBasicList<int> output = new CustomBasicList<int>();
            if (Satisfy > 0)
            {
                output.Add(Satisfy);
                return output;
            }
            int x = 0;
            foreach (var thisTrain in TrainList.Values)
            {
                x++;
                if (x == turn || thisTrain.TrainUp)
                    output.Add(x);
            }
            return output;
        }
        public int MiddleDominoDeck => CenterDomino.Deck;
        public bool CanFillPrevious(PlayerCollection<DominosMexicanTrainPlayerItem> players, int turn) //try this way.
        {
            if (CanEndEarly() == false)
                return false;
            if (Satisfy > 0)
                return true;
            var thisCol = FindAvailablePlays(turn);
            bool rets = thisCol.Count < players.Count() + 1;
            return !rets;
        }
        public void RemoveTrain(int player, PlayerCollection<DominosMexicanTrainPlayerItem> players)
        {
            if (player == players.Count() + 1)
                return;
            var thisTrain = GetTrain(player);
            thisTrain.TrainUp = false;
            if (NewestTrain == player)
                NewestTrain = 0;
            RepaintBoard();
        }
        public bool NeedDouble(out int numberNeeded)
        {
            numberNeeded = -1;
            if (Satisfy == 0)
                return false;
            numberNeeded = DominoNeeded(Satisfy);
            return true;
        }
        private MexicanDomino GetLastDomino(TrainInfo thisTrain)
        {
            if (thisTrain.DominoList.Count == 0)
                return CenterDomino;
            return thisTrain.DominoList.Last();
        }
        public int DominoNeeded(int index)
        {
            var thisTrain = TrainList[index];
            var thisDomino = GetLastDomino(thisTrain);
            if (thisDomino.FirstNum == thisDomino.SecondNum)
                return thisDomino.FirstNum;
            var thisPrivate = _imageBoard!.PrivateList[thisTrain.Index];
            if (thisPrivate.IsOpposite)
                return thisDomino.CurrentFirst;
            return thisDomino.CurrentSecond;
        }
        public void PutTrain(int player, PlayerCollection<DominosMexicanTrainPlayerItem> players)
        {
            if (player == players.Count() + 1)
                return;
            var thisTrain = GetTrain(player);
            thisTrain.TrainUp = true;
            NewestTrain = player;
            RepaintBoard();
        }
        public bool CanPlacePiece(MexicanDomino thisDomino, int player)
        {
            if (_test.AllowAnyMove)
            {
                return true;
            }
            int newNumber = DominoNeeded(player);
            return thisDomino.FirstNum == newNumber || thisDomino.SecondNum == newNumber;
        }
        private void AddDomino(int index, MexicanDomino thisDomino)
        {
            var thisTrain = TrainList[index];
            bool doubles = thisDomino.FirstNum == thisDomino.SecondNum;
            if (thisTrain.DominoList.Count == 2)
            {
                if (thisDomino.FirstNum != thisDomino.SecondNum)
                    thisTrain.DominoList.RemoveAt(1); //because 1 based.
            }
            else if (thisTrain.DominoList.Count == 3)
            {
                thisTrain.DominoList.RemoveAt(2);
                thisTrain.DominoList.RemoveAt(1);
            }
            else if (thisTrain.DominoList.Count > 3)
            {
                throw new BasicBlankException("Cannot have more than 3 items on the list");
            }
            if (thisTrain.DominoList.Count == 0 && doubles)
                throw new BasicBlankException("The first domino played cannot be doubles");
            thisTrain.DominoList.Add(thisDomino);
            if (doubles)
                Satisfy = index;
            else
                Satisfy = 0;
        }
        private void RotateDomino(int index, MexicanDomino newDomino)
        {
            var thisTrain = TrainList[index];
            var thisPrivate = _imageBoard!.PrivateList[thisTrain.Index];
            if (thisPrivate.IsRotated && newDomino.FirstNum != newDomino.SecondNum)
                newDomino.Angle = EnumRotateCategory.RotateOnly90;
            else if (newDomino.FirstNum != newDomino.SecondNum)
                newDomino.Angle = EnumRotateCategory.None;
            else if (thisPrivate.IsRotated)
                newDomino.Angle = EnumRotateCategory.None; //i think
            else
                newDomino.Angle = EnumRotateCategory.RotateOnly90;
            if (newDomino.FirstNum == newDomino.SecondNum)
                return;
            var oldDomino = GetLastDomino(thisTrain);
            if (thisPrivate.IsOpposite)
            {
                if (newDomino.SecondNum == oldDomino.CurrentFirst)
                {
                    newDomino.CurrentFirst = newDomino.FirstNum;
                    newDomino.CurrentSecond = newDomino.SecondNum;
                    return;
                }
                newDomino.CurrentFirst = newDomino.SecondNum;
                newDomino.CurrentSecond = newDomino.FirstNum;
                return;
            }
            if (newDomino.FirstNum == oldDomino.CurrentSecond)
            {
                newDomino.CurrentFirst = newDomino.FirstNum;
                newDomino.CurrentSecond = newDomino.SecondNum;
                return;
            }
            newDomino.CurrentFirst = newDomino.SecondNum;
            newDomino.CurrentSecond = newDomino.FirstNum;
        }
        public async Task AnimateShowSelectedDominoAsync(int player, MexicanDomino thisDomino, DominosMexicanTrainMainGameClass mainGame)
        {
            thisDomino.IsSelected = false;
            thisDomino.Drew = false;
            thisDomino.IsUnknown = false;
            RotateDomino(player, thisDomino);
            var imageDomino = _thisGlobal.GetDominoPiece(thisDomino);
            imageDomino.MainGraphics!.Location = new SKPoint(5, 5); //try here.
            _thisGlobal.MovingDomino = imageDomino;
            _thisGlobal.Animates!.LocationFrom = imageDomino.MainGraphics.Location;
            _thisGlobal.Animates.LocationTo = _imageBoard!.DominoLocationNeeded(player, thisDomino.CurrentFirst, thisDomino.CurrentSecond);
            await _thisGlobal.Animates.DoAnimateAsync();
            thisDomino.Location = _thisGlobal.Animates.LocationTo;
            AddDomino(player, thisDomino);
            if (thisDomino.FirstNum == thisDomino.SecondNum)
            {
                mainGame.SaveRoot!.CurrentPlayerDouble = true;
                if (mainGame.SingleInfo!.ObjectCount == 0)
                {
                    await mainGame.EndTurnAsync(true);
                    return;
                }
                _thisGlobal.BoneYard!.NewTurn();
                await mainGame.ContinueTurnAsync();
                return;
            }
            RemoveTrain(mainGame.WhoTurn, mainGame.PlayerList);
            await mainGame.EndTurnAsync(true);
        }
    }
}