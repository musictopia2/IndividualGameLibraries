using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.ViewModelInterfaces;
namespace TileRummyCP
{
    public class TileHand : HandViewModel<TileInfo>
    {
        public override void EndTurn()
        {
            HandList.ForEach(thisTile =>
            {
                if (thisTile.WhatDraw != EnumDrawType.FromSet)
                    thisTile.WhatDraw = EnumDrawType.IsNone;
                thisTile.IsSelected = false;
            });
        }
        public TileHand(IBasicGameVM thisMod) : base(thisMod)
        {
            Text = "Your Tiles";
            AutoSelect = HandViewModel<TileInfo>.EnumAutoType.SelectAsMany;
        }
    }
}