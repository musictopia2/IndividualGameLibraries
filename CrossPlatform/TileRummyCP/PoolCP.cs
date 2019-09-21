using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace TileRummyCP
{
    public class PoolCP : ScatteringPiecesViewModel<TileInfo, TileShuffler>
    {
        private readonly TileRummyViewModel _thisMod;
        public PoolCP(IBasicGameVM thisMod) : base(thisMod)
        {
            _thisMod = (TileRummyViewModel)thisMod;
            ProtectedText = "Pool";
            ObjectList = thisMod.MainContainer!.Resolve<TileShuffler>();
        }
        public bool HasTiles() => HasPieces();
        public void PopulatePool()
        {
            PopulateBoard();
        }
        public TileInfo GetTile(int deck)
        {
            TileInfo thisTile = new TileInfo();
            thisTile.Populate(deck);
            return thisTile;
        }
        public TileInfo DrawTile()
        {
            int decks = DrawPiece();
            return PrivateGetTile(decks);
        }
        public void RemoveTile(TileInfo thisTile)
        {
            RemoveSinglePiece(thisTile.Deck);
        }
        public DeckRegularDict<TileInfo> FirstDraw()
        {
            GetFirstPieces(14, out DeckObservableDict<TileInfo> newList); //28 was for testing.  should have been 14.
            return newList.ToRegularDeckDict();
        }
        private TileInfo PrivateGetTile(int deck)
        {
            TileInfo output = GetTile(deck);
            output.IsUnknown = false;
            output.Drew = true;
            output.WhatDraw = EnumDrawType.FromPool;
            return output;
        }
        protected override async Task ClickedBoardAsync()
        {
            var thisTile = DrawTile();
            await _thisMod.DrewTileAsync(thisTile);
        }
        protected override async Task ClickedPieceAsync(int Deck)
        {
            await _thisMod.DrewTileAsync(GetTile(Deck));
        }
        protected override void PrivateEnableAlways() { }
    }
}