using BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers;
using ChessCP.Data;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessCP.SpaceProcessClasses
{
    public interface IChessMan
    {
        //can just do interface.
        int Row { get; set; }
        int Column { get; set; }
        int Player { get; set; }
        bool IsReversed { get; set; }
        EnumPieceType PieceCategory { get; set; }
        CustomBasicList<CheckersChessVector> GetValidMoves();
    }
}