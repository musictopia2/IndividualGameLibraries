using BaseMahjongTilesCP;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using MahJongSolitaireCP.Data;
using MahJongSolitaireCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MahJongSolitaireCP.ViewModels
{
    [InstanceGame]
    public class MahJongSolitaireMainViewModel : Screen, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
    {
        private readonly IEventAggregator _aggregator;
        private readonly RandomGenerator _rs;
        private readonly MahJongSolitaireMainGameClass _mainGame;

        private int _tilesGone;

        public int TilesGone
        {
            get { return _tilesGone; }
            set
            {
                if (SetProperty(ref _tilesGone, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public bool CanUndoMove => _saveRoot!.PreviousList.Count > 0;
        [Command(EnumCommandCategory.Plain)]
        public void UndoMove()
        {
            _saveRoot.FirstSelected = 0;
            _customGlobal.SecondSelected = 0;
            TilesGone -= 2;
            _mainGame.GameBoard1!.PopulateBoardFromUndo();
            _saveRoot.PreviousList.Clear();
            _mainGame.Publish();
        }
        public bool CanGetHint => _saveRoot.FirstSelected == 0;
        [Command(EnumCommandCategory.Plain)]
        public void GetHint()
        {
            if (_mainGame!.GameBoard1!.ValidList.Count == 0)
            {
                _mainGame.GameBoard1.CheckForValidTiles();
            }
            _mainGlobal.CanShowTiles = false; //because animations.
            MahjongSolitaireTileInfo? tileSelected = null;
            bool hadHint = false;
            foreach (var tile1 in _mainGame.GameBoard1.ValidList)
            {
                foreach (var tile2 in _mainGame.GameBoard1.ValidList)
                {
                    if (!(tile1 == tile2))
                    {
                        _saveRoot.FirstSelected = tile1.Deck;
                        _customGlobal.SecondSelected = tile2.Deck;
                        if (_mainGame.IsValidMove() == true)
                        {

                            int TempFirst = _saveRoot.FirstSelected;
                            int TempSecond = _customGlobal.SecondSelected;
                            int ask1 = _rs!.GetRandomNumber(2);
                            if (ask1 == 1)
                            {
                                _saveRoot.FirstSelected = tile1.Deck;
                                _customGlobal.SecondSelected = 0;
                                tileSelected = tile1;
                            }
                            else
                            {
                                _saveRoot.FirstSelected = tile2.Deck;
                                _customGlobal.SecondSelected = 0;
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
            _mainGlobal.CanShowTiles = true;
            if (hadHint == false)
            {
                _saveRoot.FirstSelected = 0;
                _customGlobal.SecondSelected = 0;
                return;
            }
            tileSelected!.IsSelected = true;
            _mainGame.ShowSelectedItem(tileSelected.Deck);
        }

        public bool CanSelectTile(MahjongSolitaireTileInfo tile)
        {
            return tile.IsEnabled;
        }
        public async Task SelectTileAsync(MahjongSolitaireTileInfo tile)
        {
            var tempList = _customGlobal.TileList.GetSelectedItems();
            if (tile.IsSelected == true)
            {
                _customGlobal.TileList.UnselectAllObjects();
                _saveRoot.FirstSelected = 0;
                _customGlobal.SecondSelected = 0; //try that as well.
                _mainGame.ShowSelectedItem(0);
                return;
            }
            if (_saveRoot.FirstSelected == 0)
                _saveRoot.FirstSelected = tile.Deck;
            else if (_customGlobal.SecondSelected == 0)
                _customGlobal.SecondSelected = tile.Deck;
            if (_saveRoot.FirstSelected == 0 || _customGlobal.SecondSelected == 0)
            {
                tile.IsSelected = true;
                var nextTile = _customGlobal.TileList.GetSpecificItem(tile.Deck);
                if (nextTile.IsSelected == false)
                    throw new BasicBlankException("Did not commit properly");
                _mainGame.ShowSelectedItem(tile.Deck);
                return;
            }
            await _mainGame.PairSelectedManuallyAsync(() => TilesGone += 2);
        }

        private readonly MahJongSolitaireSaveInfo _saveRoot;
        private readonly MahJongSolitaireModGlobal _customGlobal;
        private readonly BaseMahjongGlobals _mainGlobal;
        public MahJongSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer commandContainer,
            IGamePackageResolver resolver,
            RandomGenerator rs
            )
        {
            _aggregator = aggregator;
            CommandContainer = commandContainer;
            _rs = rs;
            //there are lots of things that has to be replaced here because of new game.
            _saveRoot = resolver.ReplaceObject<MahJongSolitaireSaveInfo>();
            _mainGlobal = resolver.ReplaceObject<BaseMahjongGlobals>();
            _customGlobal = resolver.ReplaceObject<MahJongSolitaireModGlobal>();
            _ = resolver.ReplaceObject<MahJongSolitaireGameBoardCP>(); //somehow i have to replace the gameboard as well.
            _mainGame = resolver.ReplaceObject<MahJongSolitaireMainGameClass>(); //hopefully this works.  means you have to really rethink.
        }

        public CommandContainer CommandContainer { get; set; }

        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        public bool CanEnableBasics()
        {
            return true; //because maybe you can't enable it.
        }


        protected override async Task ActivateAsync()
        {
            TilesGone = 0;
            await base.ActivateAsync();
            await _mainGame.NewGameAsync(x => TilesGone = x);
        }
    }
}