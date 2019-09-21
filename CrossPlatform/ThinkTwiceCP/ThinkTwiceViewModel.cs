using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFramework.MainViewModels;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.Dice;
namespace ThinkTwiceCP
{
    public class ThinkTwiceViewModel : DiceGamesVM<SimpleDice, ThinkTwicePlayerItem, ThinkTwiceMainGameClass>
    {
        private int _Score;

        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _CategoryChosen = "None";

        public string CategoryChosen
        {
            get { return _CategoryChosen; }
            set
            {
                if (SetProperty(ref _CategoryChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        protected override bool CanRollDice()
        {
            return RollNumber <= 2;
        }
        public override bool CanEndTurn()
        {
            return MainGame!.Scores!.ItemSelected > -1; //because you have to choose a score.
        }
        public ThinkTwiceViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand? RollMultCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            RollMultCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.RollMultsAsync();
            }, items =>
            {
                if (RollNumber <= 1)
                    return false;
                return MainGame!.SaveRoot!.WhichMulti == -1;
            }, this, CommandContainer!);
        }
        protected override bool CanEnableDice()
        {
            return CanRollDice();
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 6;
            ThisCup.ShowHold = true;
            ThisCup.Visible = true;
        }
    }
}