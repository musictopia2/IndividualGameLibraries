using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace BlackjackCP
{
    [SingletonGame]
    public class BlackjackGameClass : RegularDeckOfCardsGameClass<BlackjackCardInfo>
    {
        public BlackjackSaveInfo SaveRoot;

        private readonly ISaveSinglePlayerClass _thisState;

        internal readonly BlackjackViewModel ThisMod;

        internal bool GameGoing;

        private int _oneNeeded;
        private bool _computerStartChoice; //should have been boolean

        private readonly IAsyncDelayer _thisDelay;

        public BlackjackGameClass(ISoloCardGameVM<BlackjackCardInfo> thisMod,
           IAsyncDelayer ThisDelay) : base(thisMod)
        {
            _thisState = thisMod.MainContainer!.Resolve<ISaveSinglePlayerClass>();
            ThisMod = (BlackjackViewModel)thisMod;
            SaveRoot = new BlackjackSaveInfo();
            _thisDelay = ThisDelay;
        }
        public override Task NewGameAsync()
        {
            GameGoing = true;
            return base.NewGameAsync();
        }
        public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
        {
            return await _thisState.CanOpenSavedSinglePlayerGameAsync();
        }
        public override async Task OpenSavedGameAsync()
        {
            DeckList.OrderedObjects(); //i think
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<BlackjackSaveInfo>();
            if (SaveRoot.DeckList.Count > 0)
            {
                var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                ThisMod.DeckPile!.OriginalList(newList);
            }
        }
        public async Task SaveStateAsync()
        {
            SaveRoot.DeckList = ThisMod.DeckPile!.GetCardIntegers();
            await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think
        }
        protected override void AfterShuffle()
        {
            DeckRegularDict<BlackjackCardInfo> newCol;
            int x;
            int y;
            bool choicess = false;
            _oneNeeded = 0;
            ThisMod.NeedsAceChoice = false; // defaults to false.  must be proven true.
            ThisMod.CanHitOrStay = false; // has to start that it can't hit or stay.
            ThisMod.DeckPile!.Visible = true;
            for (x = 1; x <= 2; x++)
            {
                newCol = new DeckRegularDict<BlackjackCardInfo>();
                for (y = 1; y <= 2; y++)
                    newCol.Add(ThisMod.DeckPile.DrawCard());
                if (x == 2)
                    ThisMod.ComputerStack!.ClearBoard(newCol, ref _computerStartChoice);
                else
                    ThisMod.HumanStack!.ClearBoard(newCol, ref choicess);
            }
            if (choicess == true)
            {
                _oneNeeded = 2;
                ThisMod.NeedsAceChoice = true;
            }
            else
                ThisMod.HumanPoints = ThisMod.HumanStack!.CalculateScore();
        }

        public async Task HumanHitAsync()
        {
            bool choicess = false;
            ThisMod.HumanStack!.HitMe(ThisMod.DeckPile!.DrawCard(), ref choicess);
            int points;
            if (choicess == false)
            {
                points = ThisMod.HumanStack.CalculateScore();
                ThisMod.HumanPoints = points;
                if (points > 21)
                {
                    await GameOverAsync();
                    return;
                }
                if (points == 21)
                {
                    await GameOverAsync();
                    return;
                }
            }
            if (choicess == true)
            {
                _oneNeeded = 1;
                ThisMod.NeedsAceChoice = true;
            }
        }

        private async Task GameOverAsync()
        {
            string messageToShow;
            if (ThisMod.HumanPoints == 21)
            {
                messageToShow = "Congratuations, you won because you got 21 points exactly";
                ThisMod.Wins += 1;
            }
            else if (ThisMod.ComputerPoints == 21)
            {
                messageToShow = "Sorry, you lost because the dealer got 21 points exactly";
                ThisMod.Losses += 1;
            }
            else if (ThisMod.HumanPoints > 21)
            {
                messageToShow = "Sorry, you lost because you busted for going over 21 points";
                ThisMod.Losses += 1;
            }
            else if (ThisMod.HumanPoints == ThisMod.ComputerPoints)
            {
                messageToShow = "Its a draw";
                ThisMod.Draws += 1;
            }
            else if (ThisMod.ComputerPoints > 21)
            {
                messageToShow = "Congratulations, you won because the dealer went over 21 points";
                ThisMod.Wins += 1;
            }
            else if (ThisMod.HumanPoints > ThisMod.ComputerPoints)
            {
                messageToShow = "Congratulations, you won because you had more points than the dealer had";
                ThisMod.Wins += 1;
            }
            else
            {
                messageToShow = "Sorry, you lost because the dealer got more points than you had";
                ThisMod.Losses += 1;
            }

            await ThisMod.ShowGameMessageAsync(messageToShow);
            ThisMod.DeckPile!.Visible = false; // to support mobile.
            ThisMod.NewGameVisible = true; // i think
        }
        public async Task HumanSelectAsync(bool choicess)
        {
            ThisMod.SelectedYet = true;
            ThisMod.HumanStack!.Reveal(1);
            if (choicess == true)
            {
                _oneNeeded = 1;
                ThisMod.NeedsAceChoice = true;
                return;
            }
            ThisMod.HumanPoints = ThisMod.HumanStack.CalculateScore();
            if (ThisMod.HumanPoints == 21)
            {
                await GameOverAsync();
                return;
            }
            if (ThisMod.HumanPoints > 21)
            {
                await GameOverAsync();
                return;
            }
        }

        public async Task HumanAceAsync(BlackjackViewModel.EnumAceChoice whichOne)
        {
            ThisMod.NeedsAceChoice = false; // not anymore because chose it now.
            if (whichOne == BlackjackViewModel.EnumAceChoice.Low)
                ThisMod.HumanStack!.AceChose(_oneNeeded, true);
            else
                ThisMod.HumanStack!.AceChose(_oneNeeded, false);
            ThisMod.HumanPoints = ThisMod.HumanStack.CalculateScore();
            if (ThisMod.HumanPoints == 21)
            {
                await GameOverAsync();
                return;
            }
            if (ThisMod.HumanPoints > 21)
            {
                await GameOverAsync();
                return;
            }
        }

        public async Task HumanStayAsync()
        {
            await ComputerTurnAsync();
        }

        private async Task ComputerTurnAsync()
        {
            ThisMod.SelectedYet = false;
            ThisMod.ComputerStack!.Reveal(2);
            if (_computerStartChoice == false)
                ThisMod.ComputerPoints = ThisMod.ComputerStack.CalculateScore();
            await _thisDelay.DelaySeconds(0.55);
            if (_computerStartChoice == true)
                ThisMod.ComputerStack.AceChose(2, true);
            ThisMod.ComputerPoints = ThisMod.ComputerStack.CalculateScore();
            bool Choicess = false;
            ThisMod.ComputerStack.ComputerSelectFirst(ref Choicess);
            ThisMod.SelectedYet = true;
            ThisMod.ComputerStack.Reveal(1);
            if (Choicess == true)
                ThisMod.ComputerStack.AceChose(1, WillComputerChoose1());
            do
            {
                ThisMod.ComputerPoints = ThisMod.ComputerStack.CalculateScore();
                await _thisDelay.DelaySeconds(0.5);
                if (ThisMod.ComputerPoints > ThisMod.HumanPoints)
                {
                    await GameOverAsync();
                    return;
                }

                if (ThisMod.ComputerPoints > 21)
                {
                    await GameOverAsync();
                    return;
                }

                if (ThisMod.ComputerPoints == ThisMod.HumanPoints && ThisMod.ComputerPoints > 15)
                {
                    await GameOverAsync();
                    return;
                }
                Choicess = false;
                ThisMod.ComputerStack.HitMe(ThisMod.DeckPile!.DrawCard(), ref Choicess);
                if (Choicess == true)
                    ThisMod.ComputerStack.AceChose(1, WillComputerChoose1());
                await _thisDelay.DelaySeconds(0.5);
            }
            while (true);
        }
        private bool WillComputerChoose1()
        {
            int Points1;
            int Points2;
            // 1 is with the 1
            // 2 is with the 11
            Points1 = ThisMod.ComputerPoints + 1;
            Points2 = ThisMod.ComputerPoints + 11;
            if (Points1 == 21)
                return true;
            if (Points2 == 21)
                return false;// because that will get to 21
            if (Points2 > 21)
                return true;
            if (Points2 > ThisMod.HumanPoints)
                return false;
            if (Points1 > ThisMod.HumanPoints)
                return true;
            if (Points1 == ThisMod.HumanPoints && Points1 > 15)
                return true;
            if (Points2 == ThisMod.HumanPoints)
                return false; //would rather tie than risk the human winning;
            return false;
        }
    }
}