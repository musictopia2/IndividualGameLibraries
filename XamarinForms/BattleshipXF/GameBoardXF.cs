using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BattleshipCP.Data;
using Xamarin.Forms;
namespace BattleshipXF
{
    public class GameBoardXF : BasicGameBoardXF<FieldInfoCP>
    {
        protected override View GetControl(FieldInfoCP thisItem, int index)
        {

            SpaceXF thisGraphics = new SpaceXF(thisItem);
            thisGraphics.SetBinding(SpaceXF.FillColorProperty, new Binding(nameof(FieldInfoCP.FillColor)));
            thisGraphics.SetBinding(SpaceXF.WhatHitProperty, new Binding(nameof(FieldInfoCP.Hit)));
            return thisGraphics;
            //try to use old space.
            //even though no clicking.
        }
    }
}