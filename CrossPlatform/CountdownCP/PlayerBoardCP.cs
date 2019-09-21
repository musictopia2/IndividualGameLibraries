using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BasicGameBoards;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
using SkiaSharpGeneralLibrary.GeneralGraphics;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CountdownCP
{
    public class PlayerBoardCP : BaseGameBoardCP<NumberCP>
    {
        private CountdownPlayerItem _thisPlayer;
        private readonly CustomBasicList<NumberCP> _thisList = new CustomBasicList<NumberCP>();
        private FrameGraphics? _thisFrame;
        private readonly CountdownViewModel _thisMod;
        private readonly CountdownMainGameClass _mainGame;
        public PlayerBoardCP(IGamePackageResolver mainContainer, ISkiaSharpGameBoard thisElement, CountdownPlayerItem thisPlayer) : base(mainContainer, thisElement)
        {
            _thisMod = Resolve<CountdownViewModel>();
            _mainGame = Resolve<CountdownMainGameClass>();
            _thisPlayer = thisPlayer; //i think here this time.  if i am wrong, rethink
            if (thisPlayer.NumberList.Count == 0)
                thisPlayer.NumberList = _mainGame.GetNewNumbers(); //i think
        }

        public override string TagUsed => ""; //maybe we don't need a tag.  if i am wrong, rethink.

        protected override SKSize OriginalSize { get; set; } = new SKSize(320, 100); // can experiment as well.

        protected override bool CanStartPaint()
        {
            return true;
        }
        public void UpdatePlayer(CountdownPlayerItem thisPlayer)
        {
            _thisPlayer = thisPlayer;
            _thisList.Clear();
            CreateSpaces(); //try to create spaces again.
        }
        private bool CanClickOnPlayer()
        {
            if (_thisMod.CommandContainer!.IsExecuting == true)
                return false;
            var YourID = _thisPlayer.Id;
            if (YourID == _mainGame.SaveRoot!.PlayOrder.WhoTurn) //has to do it this way this time.
                return true;
            return false;
        }

        protected override void ClickProcess(SKPoint thisPoint)
        {
            if (CanClickOnPlayer() == false)
                return;// i think the dice should roll automatically.
            foreach (var thisNumber in _thisList)
            {
                var thisRect = thisNumber.GetMainRect();
                if (MiscHelpers.DidClickRectangle(thisRect, thisPoint) == true)
                {
                    if (_mainGame.CanChooseNumber(thisNumber.ThisNumber!) == false)
                        return;// no need for messagebox, just do nothing
                    _thisMod.SpaceCommand!.Execute(thisNumber.ThisNumber!);
                    return;
                }
            }
        }

        protected override void CreateSpaces()
        {
            if (_thisPlayer.NumberList.Count != 10)
                throw new Exception("Must have 10 numbers so it can create the processes");
            _thisFrame = new FrameGraphics(null); //could require rethinking since it can be used with other graphics.
            _thisFrame.Text = _thisPlayer.NickName;
            var bounds = GetBounds();
            _thisFrame.ActualHeight = bounds.Height;
            _thisFrame.ActualWidth = bounds.Width;
            _thisFrame.Visible = true;
            int x;
            int y;
            int z = default;
            float tops;
            tops = 13;
            var ThisSize = GetActualSize(60, 40);
            for (x = 1; x <= 2; x++)
            {
                float lefts = 8;
                for (y = 1; y <= 5; y++)
                {
                    NumberCP newNumber = new NumberCP();
                    var tempNumber = _thisPlayer.NumberList[z];
                    newNumber.ThisNumber = tempNumber;
                    newNumber.NeedsToClear = false; // not this time.
                    newNumber.ActualHeight = ThisSize.Height;
                    newNumber.ActualWidth = ThisSize.Width;
                    newNumber.Location = new SKPoint((float)lefts, (float)tops);
                    _thisList.Add(newNumber);
                    lefts += ThisSize.Width + 3;
                    z += 1;
                }
                tops += ThisSize.Height + 3;
            }
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            if (CanClickOnPlayer() == false)
                _thisFrame!.IsEnabled = false;
            else
                _thisFrame!.IsEnabled = true;
            _thisFrame.DrawFrame(canvas);
            foreach (var ThisNumber in _thisList)
            {
                ThisNumber.IsEnabled = _thisFrame.IsEnabled;
                ThisNumber.DrawImage(canvas);
            }
        }
        protected override void SetUpPaints() { }
    }
}