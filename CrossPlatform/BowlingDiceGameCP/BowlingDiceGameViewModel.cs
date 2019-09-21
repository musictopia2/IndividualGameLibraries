using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace BowlingDiceGameCP
{
    public class BowlingDiceGameViewModel : BasicMultiplayerVM<BowlingDiceGamePlayerItem, BowlingDiceGameMainGameClass>
    {
        private bool _IsExtended;
        public bool IsExtended
        {
            get { return _IsExtended; }
            set
            {
                if (SetProperty(ref _IsExtended, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _WhichPart;
        public int WhichPart
        {
            get { return _WhichPart; }
            set
            {
                if (SetProperty(ref _WhichPart, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _WhatFrame;
        public int WhatFrame
        {
            get { return _WhatFrame; }
            set
            {
                if (SetProperty(ref _WhatFrame, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool LastTurn;
        public override bool CanEndTurn()
        {
            if (base.CanEndTurn() == false)
                return false;
            if (WhichPart < 3)
                return false;
            if (LastTurn == true)
                return false;
            return !IsExtended;
        }
        public BasicGameCommand? ContinueTurnCommand { get; set; }
        public BasicGameCommand? RollCommand { get; set; }
        public BowlingDiceGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            ContinueTurnCommand = new BasicGameCommand(this, items =>
            {
                MainGame!.SaveRoot!.IsExtended = false;
                LastTurn = true; //this is it
            }, items =>
            {
                return IsExtended;
            }, this, CommandContainer!);
            RollCommand = new BasicGameCommand(this, async items =>
            {
                LastTurn = false;
                await MainGame!.RollDiceAsync();
            }, items =>
            {
                if (WhichPart < 3)
                    return true;
                return LastTurn;
            }, this, CommandContainer!);
        }
    }
}