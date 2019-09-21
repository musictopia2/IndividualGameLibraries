﻿using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BasicGameBoards;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using System.Linq;
namespace MancalaCP
{
    [SingletonGame]
    public class GameBoardGraphicsCP : BaseGameBoardCP<CheckerPiecesCP>
    {
        public GameBoardGraphicsCP(IGamePackageResolver MainContainer) : base(MainContainer)
        {
            _mainGame = MainContainer.Resolve<MancalaMainGameClass>();
        }
        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(74, 310);
        internal static void CreateSpaceList(MancalaMainGameClass mainGame)
        {
            mainGame.SpaceList = new Dictionary<int, SpaceInfo>();
            14.Times(x =>
            {
                SpaceInfo ThisSpace = new SpaceInfo();
                ThisSpace.Pieces = 0;
                mainGame.SpaceList.Add(x, ThisSpace); //has to be done this way.
            });
        }
        protected override bool CanStartPaint()
        {
            return true;
        }
        protected override void ClickProcess(SKPoint thisPoint)
        {
            if (_mainGame!.ThisMod!.SpaceCommand!.CanExecute(0) == false)
                return;
            var thisList = _mainGame.SpaceList!.Values.Take(6).ToCustomBasicList();
            foreach (var thisNumber in thisList)
            {
                if (MiscHelpers.DidClickRectangle(thisNumber.Bounds, thisPoint) == true)
                {
                    _mainGame.ThisMod.SpaceCommand.Execute(_mainGame.SpaceList.GetKey(thisNumber));
                    return;
                }
            }
        }
        private readonly MancalaMainGameClass _mainGame;
        protected override void CreateSpaces()
        {
            14.Times(x =>
            {
                SpaceInfo thisSpace = _mainGame.SpaceList![x];
                if (x == 1)
                    thisSpace.Bounds = GetActualRectangle(40, 40, 33, 33);
                else if (x == 2)
                    thisSpace.Bounds = GetActualRectangle(40, 79, 33, 33);
                else if (x == 3)
                    thisSpace.Bounds = GetActualRectangle(40, 118, 33, 33);
                else if (x == 4)
                    thisSpace.Bounds = GetActualRectangle(40, 157, 33, 33);
                else if (x == 5)
                    thisSpace.Bounds = GetActualRectangle(40, 196, 33, 33);
                else if (x == 6)
                    thisSpace.Bounds = GetActualRectangle(40, 235, 33, 33);
                else if (x == 7)
                    thisSpace.Bounds = GetActualRectangle(1, 274, 73, 33);
                else if (x == 8)
                    thisSpace.Bounds = GetActualRectangle(1, 235, 33, 33);
                else if (x == 9)
                    thisSpace.Bounds = GetActualRectangle(1, 196, 33, 33);
                else if (x == 10)
                    thisSpace.Bounds = GetActualRectangle(1, 157, 33, 33);
                else if (x == 11)
                    thisSpace.Bounds = GetActualRectangle(1, 118, 33, 33);
                else if (x == 12)
                    thisSpace.Bounds = GetActualRectangle(1, 79, 33, 33);
                else if (x == 13)
                    thisSpace.Bounds = GetActualRectangle(1, 40, 33, 33);
                else if (x == 14)
                    thisSpace.Bounds = GetActualRectangle(1, 1, 73, 33);
            });
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            int x = 0;
            foreach (var thisSpace in _mainGame.SpaceList!.Values)
            {
                canvas.DrawOval(thisSpace.Bounds, _whitePaint);
                SKPaint thisPen;
                x += 1;
                if (x == _mainGame.SpaceSelected)
                    thisPen = _redPen!;
                else if (x == _mainGame.SpaceStarted)
                    thisPen = _greenPen!;
                else
                    thisPen = _blackPen!;
                canvas.DrawOval(thisSpace.Bounds, thisPen);
                float fontSize = thisSpace.Bounds.Height * 0.8f; // can change
                var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
                canvas.DrawCustomText(thisSpace.Pieces.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thisSpace.Bounds, out _);
            }
        }
        private SKPaint? _whitePaint;
        private SKPaint? _blackPen;
        private SKPaint? _greenPen;
        private SKPaint? _redPen;
        protected override void SetUpPaints()
        {
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _blackPen = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _greenPen = MiscHelpers.GetStrokePaint(SKColors.Green, 3);
            _redPen = MiscHelpers.GetStrokePaint(SKColors.Red, 3);
        }
    }
}