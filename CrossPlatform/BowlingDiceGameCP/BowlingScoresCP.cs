using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
namespace BowlingDiceGameCP
{
    [SingletonGame]
    public class BowlingScoresCP
    {
        public readonly CustomBasicList<string> TextFrames;
        private readonly BowlingDiceGameMainGameClass ThisGame;
        public BowlingScoresCP(BowlingDiceGameMainGameClass ThisGame)
        {
            TextFrames = new CustomBasicList<string>();
            10.Times(Items =>
            {
                TextFrames.Add($"Frame {Items}");
            });
            this.ThisGame = ThisGame;
        }
        private FrameInfoCP FindMainFrame()
        {
            return ThisGame!.SingleInfo!.FrameList[ThisGame.SaveRoot!.WhatFrame];
        }
        public bool CanExtend()
        {
            FrameInfoCP thisFrame;
            thisFrame = FindMainFrame();
            int x;
            for (x = 1; x <= 2; x++)
            {
                if (thisFrame.SectionList[x].Score == "X" || thisFrame.SectionList[x].Score == "/")
                    return true;
            }
            return false;
        }
        public void UpdateForSection(int howMany)
        {
            if (howMany > 10)
                throw new BasicBlankException("The most amount of pins that can be hit is 10, not " + howMany);
            if (howMany < 0)
                throw new BasicBlankException("The number of pins cannot be less than 0");
            FrameInfoCP mainFrame;
            mainFrame = FindMainFrame();
            if (howMany == 10 && ThisGame.SaveRoot!.WhichPart == 1)
            {
                mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = "X";
                return;
            }
            if (ThisGame.SaveRoot!.WhichPart == 1)
            {
                mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = howMany.ToString();
                return;
            }
            int CurrentScore;
            if (ThisGame.SaveRoot.WhichPart == 2)
            {
                if (mainFrame.SectionList[1].Score == "X" && howMany == 10)
                {
                    mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = "X";
                    return;
                }
                if (mainFrame.SectionList[1].Score == "X" && howMany < 10)
                {
                    mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = howMany.ToString();
                    return;
                }
                CurrentScore = int.Parse(mainFrame.SectionList[1].Score) + howMany;
                if (CurrentScore > 10)
                    throw new Exception("There must be only 10 at a most, not " + CurrentScore + " for part 2");
                if (CurrentScore == 10)
                {
                    mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = "/";
                    return;
                }
                mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = howMany.ToString();
                return;
            }
            if (ThisGame.SaveRoot.WhichPart == 3)
            {
                var previousScore = mainFrame.SectionList[2];
                if (previousScore.Score == "X" && howMany == 10)
                {
                    mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = "X";
                    return;
                }
                if (previousScore.Score == "/" && howMany == 10)
                {
                    mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = "X";
                    return;
                }
                if (previousScore.Score == "X" && howMany < 10)
                {
                    mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = howMany.ToString();
                    return;
                }
                if (previousScore.Score == "/" && howMany < 10)
                {
                    mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = howMany.ToString();
                    return;
                }
                CurrentScore = int.Parse(previousScore.Score) + howMany;
                if (CurrentScore == 10)
                {
                    mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = "/";
                    return;
                }
                mainFrame.SectionList[ThisGame.SaveRoot.WhichPart].Score = howMany.ToString();
                return;
            }
        }
        private int CalculateScoreForFrame()
        {
            FrameInfoCP thisFrame;
            thisFrame = FindMainFrame();
            string value1;
            string value2;
            string value3;
            value1 = "";
            value2 = "";
            value3 = "";
            int x = 0;
            foreach (var minis in thisFrame.SectionList.Values)
            {
                x += 1;
                if (x == 1)
                    value1 = minis.Score;
                else if (x == 2)
                    value2 = minis.Score;
                else if (x == 3)
                    value3 = minis.Score;
                else
                    throw new Exception("Nothing for " + x);
            }
            if (value1 == "X" && value2 == "X" && value3 == "X")
                return 30;
            if (value3 == "")
                return System.Convert.ToInt32(value1) + System.Convert.ToInt32(value2);
            if (value2 == "/" && value3 == "X")
                return 20;
            if (value1 == "X" && value2 == "X")
                return 20 + System.Convert.ToInt32(value3);
            if (value1 == "X" && value3 == "/")
                return 20;
            if (value2 == "/")
                return 10 + System.Convert.ToInt32(value3);
            if (value1 == "X")
                return 10 + System.Convert.ToInt32(value2) + System.Convert.ToInt32(value3);
            throw new BasicBlankException("Cannot figure out based on " + value1 + ", " + value2 + ", " + value3);
        }
        public void UpdateScore()
        {
            int currentScore;
            currentScore = CalculateScoreForFrame();
            FrameInfoCP mainFrame;
            mainFrame = FindMainFrame();
            mainFrame.Score = currentScore + ThisGame.SingleInfo!.TotalScore;
            ThisGame.SingleInfo.TotalScore = mainFrame.Score;
        }
        public void ClearBoard()
        {
            foreach (var tempPlayer in ThisGame.PlayerList!)
            {
                foreach (var thisFrame in tempPlayer.FrameList.Values)
                {
                    thisFrame.Score = -1; // has to be -1 so it won't display 0s everywhere.
                    int x;
                    var loopTo = thisFrame.SectionList.Count;
                    for (x = 1; x <= loopTo; x++)
                        thisFrame.SectionList[x].Score = "";// has to be this way.
                }
                tempPlayer.TotalScore = 0;
            }
        }
        public bool NeedToClear()
        {
            if (ThisGame.SaveRoot!.WhichPart == 1)
                return false;// because its the first roll
            string previousMini;
            FrameInfoCP mainFrame;
            mainFrame = FindMainFrame();
            previousMini = mainFrame.SectionList[ThisGame.SaveRoot.WhichPart - 1].Score;
            if (previousMini == "X" || previousMini == "/")
                return true;
            return false;
        }
        public int PreviousHit()
        {
            if (ThisGame.SaveRoot!.WhichPart == 1)
                return 0;// because its the first one
            string previousMini;
            FrameInfoCP mainFrame;
            mainFrame = FindMainFrame();
            previousMini = mainFrame.SectionList[ThisGame.SaveRoot.WhichPart - 1].Score;
            if (previousMini == "X" || previousMini == "/")
                return 0;
            if (previousMini == "")
                throw new Exception("Cannot be blank. If none was hit, should show 0");
            return int.Parse(previousMini);
        }
    }
}