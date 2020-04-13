using MastermindCP.Data;

namespace MastermindCP.EventModels
{
    public class ScrollToGuessEventModel
    {
        public ScrollToGuessEventModel(Guess guess)
        {
            Guess = guess;
        }

        public Guess Guess { get; }
    }
}