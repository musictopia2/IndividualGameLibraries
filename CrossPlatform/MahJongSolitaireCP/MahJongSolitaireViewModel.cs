using BaseMahjongTilesCP;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MahJongSolitaireCP
{
    public class MahJongSolitaireViewModel : SimpleGameVM
    {

        public MahJongSolitaireMainGameClass? MainGame; //i think
        public PlainCommand? UndoMoveCommand { get; set; }
        public PlainCommand<MahjongSolitaireTileInfo>? TileSelectedCommand { get; set; }
        public PlainCommand? HintCommand { get; set; }
        private RandomGenerator? _rs;

        private int _TilesGone;

        public int TilesGone
        {
            get { return _TilesGone; }
            set
            {
                if (SetProperty(ref _TilesGone, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public MahJongSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }

        public override async Task StartNewGameAsync()
        {
            TilesGone = 0; //to start with.
            await MainGame!.NewGameAsync(Items => TilesGone = Items);
            NewGameVisible = true;
            CommandContainer!.IsExecuting = false; //has to be done manually just this time.
        }

        private async Task UndoMoveAsync()
        {
            MainGame!.SaveRoot!.FirstSelected = 0;
            MainGame.CustomGlobal.SecondSelected = 0;
            TilesGone -= 2;
            MainGame.GameBoard1!.PopulateBoardFromUndo();
            MainGame.SaveRoot.PreviousList.Clear();
            MainGame.Publish();
            await MainGame.MainSave.SaveSimpleSinglePlayerGameAsync(MainGame.SaveRoot);
        }
        private async Task ProcessHintAsync()
        {
            if (MainGame!.GameBoard1!.ValidList.Count == 0)
                MainGame.GameBoard1.CheckForValidTiles();
            MainGame.ThisGlobal!.CanShowTiles = false; //because animations.
            MahjongSolitaireTileInfo? tileSelected = null;
            bool hadHint = false;
            foreach (var tile1 in MainGame.GameBoard1.ValidList)
            {
                foreach (var tile2 in MainGame.GameBoard1.ValidList)
                {
                    if (!(tile1 == tile2))
                    {
                        MainGame.SaveRoot!.FirstSelected = tile1.Deck;
                        MainGame.CustomGlobal.SecondSelected = tile2.Deck;
                        if (MainGame.IsValidMove() == true)
                        {

                            int TempFirst = MainGame.SaveRoot.FirstSelected;
                            int TempSecond = MainGame.CustomGlobal.SecondSelected;
                            int ask1 = _rs!.GetRandomNumber(2);
                            if (ask1 == 1)
                            {
                                MainGame.SaveRoot.FirstSelected = tile1.Deck;
                                MainGame.CustomGlobal.SecondSelected = 0;
                                tileSelected = tile1;
                            }
                            else
                            {
                                MainGame.SaveRoot.FirstSelected = tile2.Deck;
                                MainGame.CustomGlobal.SecondSelected = 0;
                                tileSelected = tile2;
                            }
                            hadHint = true;
                            break;

                        }
                    }

                }
                if (hadHint == true)
                    break;
            }
            MainGame.ThisGlobal.CanShowTiles = true;
            if (hadHint == false)
            {
                MainGame.SaveRoot!.FirstSelected = 0;
                MainGame.CustomGlobal.SecondSelected = 0;
                return;
            }
            tileSelected!.IsSelected = true;
            MainGame.ShowSelectedItem(tileSelected.Deck);
            await MainGame.MainSave.SaveSimpleSinglePlayerGameAsync(MainGame.SaveRoot!);
        }
        private async Task ProcessTileAsync(MahjongSolitaireTileInfo thisTile)
        {
            var tempList = MainGame!.CustomGlobal.TileList.GetSelectedItems();
            if (thisTile.IsSelected == true)
            {
                MainGame.CustomGlobal.TileList.UnselectAllObjects();
                MainGame.SaveRoot!.FirstSelected = 0;
                MainGame.CustomGlobal.SecondSelected = 0; //try that as well.
                await MainGame.MainSave.SaveSimpleSinglePlayerGameAsync(MainGame.SaveRoot);
                MainGame.ShowSelectedItem(0);
                return;
            }
            if (MainGame.SaveRoot!.FirstSelected == 0)
                MainGame.SaveRoot.FirstSelected = thisTile.Deck;
            else if (MainGame.CustomGlobal.SecondSelected == 0)
                MainGame.CustomGlobal.SecondSelected = thisTile.Deck;
            if (MainGame.SaveRoot.FirstSelected == 0 || MainGame.CustomGlobal.SecondSelected == 0)
            {
                thisTile.IsSelected = true;
                var NextTile = MainGame.CustomGlobal.TileList.GetSpecificItem(thisTile.Deck);
                if (NextTile.IsSelected == false)
                    throw new BasicBlankException("Did not commit properly");
                MainGame.ShowSelectedItem(thisTile.Deck);
                await MainGame.MainSave.SaveSimpleSinglePlayerGameAsync(MainGame.SaveRoot);
                return;
            }
            await MainGame.PairSelectedManuallyAsync(() => TilesGone += 2);
        }

        public override void Init()
        {
            MainGame = MainContainer!.Resolve<MahJongSolitaireMainGameClass>();
            _rs = MainContainer.Resolve<RandomGenerator>(); //needed random for the hints
            UndoMoveCommand = new PlainCommand(async Items =>
            {
                await UndoMoveAsync();
            }, Items =>
            {
                return MainGame.SaveRoot!.PreviousList.Count > 0;
            }, this, CommandContainer!);

            HintCommand = new PlainCommand(async Items =>
            {
                await ProcessHintAsync();
            }, Items =>
            {
                return MainGame.SaveRoot!.FirstSelected == 0;
            }, this, CommandContainer!);
            TileSelectedCommand = new PlainCommand<MahjongSolitaireTileInfo>
            (async Items => await ProcessTileAsync(Items), Items => Items.IsEnabled, this, CommandContainer!);
        }
    }
}