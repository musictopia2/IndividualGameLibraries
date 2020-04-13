using MastermindCP.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MastermindCP.Logic
{
    public interface IFinishGuess
    {
        Task FinishGuessAsync(int howManyCorrect, GameBoardViewModel board);
    }
}
