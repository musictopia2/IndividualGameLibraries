using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Threading.Tasks;
using TileRummyCP.Data;

namespace TileRummyCP.Logic
{
    public class PoolCP : ScatteringPiecesObservable<TileInfo, TileShuffler>
    {
        //private readonly TileRummyViewModel _thisMod;
        public PoolCP(CommandContainer command, IGamePackageResolver resolver, TileShuffler objectList) : base(command, resolver)
        {
            //_thisMod = (TileRummyViewModel)thisMod;
            ProtectedText = "Pool";
            ObjectList = objectList;
        }

        public Func<TileInfo, Task>? DrewTileAsync { get; set; }

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
            if (DrewTileAsync == null)
            {
                throw new BasicBlankException("Drew tile function was not set");
            }
            await DrewTileAsync.Invoke(thisTile);
        }
        protected override async Task ClickedPieceAsync(int Deck)
        {
            if (DrewTileAsync == null)
            {
                throw new BasicBlankException("Drew tile function was not set");
            }
            await DrewTileAsync.Invoke(GetTile(Deck));
        }
        protected override void PrivateEnableAlways() { }
    }
}
