using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using BattleshipCP;
using Xamarin.Forms;
namespace BattleshipXF
{
    public class GameBoardWPF : BasicGameBoardXF<FieldInfoCP>
    {
        protected override View GetControl(FieldInfoCP thisItem, int index)
        {
            SpaceXF thisGraphics = new SpaceXF(thisItem);
            thisGraphics.SetBinding(SpaceXF.FillColorProperty, new Binding(nameof(FieldInfoCP.FillColor)));
            thisGraphics.SetBinding(SpaceXF.WhatHitProperty, new Binding(nameof(FieldInfoCP.Hit)));
            return thisGraphics;
        }
    }
}