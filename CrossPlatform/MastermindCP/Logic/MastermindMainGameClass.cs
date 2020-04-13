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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommonInterfaces;
using MastermindCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using MastermindCP.ViewModels;

namespace MastermindCP.Logic
{
    [SingletonGame]
    public class MastermindMainGameClass : IAggregatorContainer, IFinishGuess
    {
        private readonly GlobalClass _global;
        private readonly LevelClass _level;
        public MastermindMainGameClass(IEventAggregator aggregator,
            GlobalClass global,
            LevelClass level
            )
        {
            Aggregator = aggregator;
            _global = global;
            _level = level;
        }


        public IEventAggregator Aggregator { get; }

        public async Task NewGameAsync(GameBoardViewModel guess)
        {
            //hopefully no need to hide solution anymore since another class is responsible for it now.
            bool canRepeat = _level.LevelChosen == 2 || _level.LevelChosen == 4 || _level.LevelChosen == 6;
            int level = _level.LevelChosen;
            CustomBasicList<Bead> possibleList = new CustomBasicList<Bead>();
            if (level == 5 || level == 6)
            {
                possibleList.Add(new Bead(EnumColorPossibilities.Aqua));
                possibleList.Add(new Bead(EnumColorPossibilities.Black));
            }
            possibleList.Add(new Bead(EnumColorPossibilities.Blue));
            possibleList.Add(new Bead(EnumColorPossibilities.Green));
            if (level > 2)
                possibleList.Add(new Bead(EnumColorPossibilities.Purple));
            possibleList.Add(new Bead(EnumColorPossibilities.Red));
            possibleList.Add(new Bead(EnumColorPossibilities.White));
            if (level > 2)
                possibleList.Add(new Bead(EnumColorPossibilities.Yellow));
            ICustomBasicList<Bead> tempList;
            if (canRepeat == false)
                tempList = possibleList.GetRandomList(false, 4);
            else
            {
                int x;
                tempList = new CustomBasicList<Bead>();
                for (x = 1; x <= 4; x++)
                {
                    var ThisBead = possibleList.GetRandomItem();
                    tempList.Add(ThisBead); // can have repeat
                }
            }
            _global.Solution = tempList.ToCustomBasicList();
            await guess.NewGameAsync();
            await guess.StartNewGuessAsync();
            _global.ColorList = possibleList.Select(items => items.ColorChosen).ToCustomBasicList();
        }

        public async Task GiveUpAsync()
        {
            await UIPlatform.ShowMessageAsync("Sorry you are giving up");
            await this.SendGameOverAsync(); //hopefully this works.
            //Aggregator.ShowSolution(); //does not care who responds to showing solution.
            //i propose a new view model for the solution part.

        }

        async Task IFinishGuess.FinishGuessAsync(int howManyCorrect, GameBoardViewModel board)
        {
            bool handled = false;
            if (howManyCorrect == 4)
            {
                await UIPlatform.ShowMessageAsync("Congratuations, you won");
                handled = true;
            }
            if (board.GuessList.Last().IsCompleted)
            {
                await UIPlatform.ShowMessageAsync("You ran out of guesses.");
                handled = true;
            }
            if (handled)
            {
                await this.SendGameOverAsync();
                return;
            }
            await board.StartNewGuessAsync();
        }
    }
}