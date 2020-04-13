using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using BattleshipCP.Data;
using System.Windows.Controls;
using System.Windows.Data;
namespace BattleshipWPF
{
    public class GameBoardWPF : BasicGameBoard<FieldInfoCP>
    {
        protected override Control GetControl(FieldInfoCP thisItem, int index)
        {
            SpaceWPF thisGraphics = new SpaceWPF(thisItem);
            thisGraphics.SetBinding(SpaceWPF.FillColorProperty, new Binding(nameof(FieldInfoCP.FillColor)));
            thisGraphics.SetBinding(SpaceWPF.WhatHitProperty, new Binding(nameof(FieldInfoCP.Hit)));
            return thisGraphics;
        }
    }
}