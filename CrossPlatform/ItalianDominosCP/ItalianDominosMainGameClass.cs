using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Dominos;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ItalianDominosCP
{
    [SingletonGame]
    public class ItalianDominosMainGameClass : DominosGameClass<SimpleDominoInfo, ItalianDominosPlayerItem, ItalianDominosSaveInfo>
    {
        public ItalianDominosMainGameClass(IGamePackageResolver _Container) : base(_Container) { }
        private ItalianDominosViewModel? _thisMod;
        public override void Init()
        {
            base.Init();
            _thisMod = MainContainer!.Resolve<ItalianDominosViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            if (IsLoaded == false)
                LoadUp();
            ProtectedLoadBone();
            AfterPassedDominos(); //i think this too.
            return Task.CompletedTask;
        }
        private void LoadUp()
        {
            LoadUpDominos();
            SaveRoot!.LoadMod(_thisMod!);
        }
        protected override async Task AfterDrawingDomino()
        {
            SingleInfo!.DrewYet = true;
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            await EndTurnAsync();
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
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            if (IsLoaded == false)
            {
                LoadUp();
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
            if (ThisTest!.DoubleCheck == false)
                SaveRoot!.UpTo = 0;
            else
                SaveRoot!.UpTo = 101;
            SaveRoot.NextNumber = 30;
            ClearBoneYard();
            PassDominos();
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public override async Task StartNewTurnAsync()
        {
            ProtectedStartTurn();
            await ContinueTurnAsync(); //i think.
        }
        protected async override Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            if (SingleInfo!.DrewYet == false)
            {
                await DrawDominoAsync(_thisMod!.BoneYard!.DrawDomino());
                return;
            }
            await PlayDominoAsync(ItalianDominosComputerAI.Play(this));
        }
        public override async Task EndTurnAsync()
        {
            _thisMod!.PlayerHand1!.EndTurn();
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public override Task PopulateSaveRootAsync()
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