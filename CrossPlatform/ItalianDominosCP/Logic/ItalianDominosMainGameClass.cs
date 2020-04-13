using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using ItalianDominosCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace ItalianDominosCP.Logic
{
    [SingletonGame]
    public class ItalianDominosMainGameClass : DominosGameClass<SimpleDominoInfo, ItalianDominosPlayerItem, ItalianDominosSaveInfo>
    {
        public ItalianDominosMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            ItalianDominosVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            BasicGameContainer<ItalianDominosPlayerItem, ItalianDominosSaveInfo> gameContainer
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer)
        {
            _model = model;
            DominosToPassOut = 6; //usually 6 but can be changed.

        }

        private readonly ItalianDominosVMData _model;

        public override Task FinishGetSavedAsync()
        {
            LoadUpDominos();
            ProtectedLoadBone();
            AfterPassedDominos(); //i think this too.
            //anything else needed is here.
            return Task.CompletedTask;
        }

        public override async Task SetUpGameAsync(bool isBeginning)
        {
            if (IsLoaded == false)
            {
                LoadUpDominos();
                if (PlayerList.Count() == 3)
                    DominosToPassOut = 7;
                else if (PlayerList.Count() == 4)
                    DominosToPassOut = 5;
                else if (PlayerList.Count() == 5)
                    DominosToPassOut = 4;
                else
                    throw new BasicBlankException("Has to have 3 to 5 players");
            }
            PlayerList!.ForEach(items =>
            {
                items.TotalScore = 0;
                items.DrewYet = false;
            });
            if (Test!.DoubleCheck == false)
                SaveRoot!.UpTo = 0;
            else
                SaveRoot!.UpTo = 101;
            SaveRoot.NextNumber = 30;
            ClearBoneYard();
            PassDominos();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            await FinishUpAsync(isBeginning);
        }

        protected override async Task AfterDrawingDomino()
        {
            SingleInfo!.DrewYet = true;
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            await EndTurnAsync();
        }


        public override async Task StartNewTurnAsync()
        {
            ProtectedStartTurn(); //anything else is below.

            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }

        public override async Task PlayDominoAsync(int deck)
        {
            await SendPlayDominoAsync(deck); //if it can't send, won't.
            SimpleDominoInfo thisDomino = SingleInfo!.MainHandList.GetSpecificItem(deck);
            SingleInfo.MainHandList.RemoveSpecificItem(thisDomino); //i think.
            int Points = PointsObtained(SaveRoot!.UpTo, thisDomino.Points, out int NewNumber);
            if (NewNumber > 0)
                SaveRoot.NextNumber = NewNumber;
            SingleInfo.TotalScore += Points;
            SaveRoot.UpTo += thisDomino.Points;
            if (SaveRoot.UpTo >= 100)
            {
                await GameOverAsync();
                return;
            }
            await EndTurnAsync();
        }
        private int PointsObtained(int firsts, int points, out int newPoint)
        {
            newPoint = 0;
            if ((firsts + points) == 30)
            {
                newPoint = 50;
                return 1;
            }
            if ((firsts + points) == 50)
            {
                newPoint = 70;
                return 2;
            }
            if ((firsts + points) == 70)
            {
                newPoint = 100;
                return 4;
            }
            if ((firsts + points) == 100)
                return 8;
            if ((firsts + points) < 30)
                newPoint = 30;
            else if ((firsts + points) < 50)
                newPoint = 50;
            else if ((firsts + points) < 70)
                newPoint = 70;
            else
                newPoint = 100;
            return 0;
        }
        protected async override Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            if (SingleInfo!.DrewYet == false)
            {
                await DrawDominoAsync(_model!.BoneYard!.DrawDomino());
                return;
            }
            await PlayDominoAsync(ItalianDominosComputerAI.Play(this));
        }
        public override async Task EndTurnAsync() //usually the process for ending turn.
        {
            _model!.PlayerHand1!.EndTurn();
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public override Task PopulateSaveRootAsync() //usually needs this too.
        {
            ProtectedSaveBone();
            return Task.CompletedTask;
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerWon();
            if (SingleInfo == null)
                await ShowTieAsync();
            else
                await ShowWinAsync();
        }
        private ItalianDominosPlayerItem? PlayerWon()
        {
            var FirstList = PlayerList.OrderByDescending(items => items.TotalScore).Take(2).ToCustomBasicList();
            if (FirstList.First().TotalScore == FirstList.Last().TotalScore)
                return null;
            return FirstList.First();
        }

    }
}
