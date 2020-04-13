using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using TileRummyCP.Data;

namespace TileRummyCP.Logic
{
    public class TileHand : HandObservable<TileInfo>
    {
        public TileHand(CommandContainer command) : base(command)
        {
            Text = "Your Tiles";
            AutoSelect = EnumAutoType.SelectAsMany;
        }
        public override void EndTurn()
        {
            HandList.ForEach(thisTile =>
            {
                if (thisTile.WhatDraw != EnumDrawType.FromSet)
                    thisTile.WhatDraw = EnumDrawType.IsNone;
                thisTile.IsSelected = false;
            });
        }
    }
}