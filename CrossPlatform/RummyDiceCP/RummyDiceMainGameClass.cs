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
using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.DIContainers;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;

namespace RummyDiceCP
{
    [SingletonGame]
    public class RummyDiceMainGameClass : BasicGameClass<RummyDicePlayerItem, RummyDiceSaveInfo>, IMiscDataNM
    {
        public RummyDiceMainGameClass(IGamePackageResolver container) : base(container) { }

        private RummyDiceViewModel? _thisMod;
        public RummyProcesses<EnumColorType, EnumColorType, RummyDiceInfo>? Rummys;

        public CustomBasicList<PhaseList>? PhaseInfo;
        public CustomBasicList<RummyDiceHandVM>? TempSets; //maybe this will work.
        public RummyBoardCP? MainBoard1;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
			base.Init();
            _thisMod = MainContainer.Resolve<RummyDiceViewModel>();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            SaveRoot!.LoadMod(_thisMod!);
            if (SaveRoot.TempSets.Count != 2)
                throw new BasicBlankException("You always have to have data for 2 temp sets even if you never put data there.");
            int x = 0;
            SaveRoot.TempSets.ForEach(firsts =>
            {
                var thisItem = TempSets![x];
                thisItem.HandList = firsts.ToCustomBasicCollection();
                x++;
            });
            PrepStartTurn(); //i think this is fine too.
            return Task.CompletedTask;
        }
        protected override void PrepStartTurn()
        {
            base.PrepStartTurn();
            _thisMod!.CurrentPhase = PhaseInfo![SingleInfo!.Phase - 1].Description;
        }
        public override Task PopulateSaveRootAsync()
        {
            int x = 0;
            SaveRoot!.TempSets.ForEach(firsts =>
            {
                firsts.Clear();
                var thisItem = TempSets![x];
                firsts.AddRange(thisItem.HandList);
                x++;
            });
            return base.PopulateSaveRootAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            MainBoard1 = MainContainer.Resolve<RummyBoardCP>();
            TempSets = new CustomBasicList<RummyDiceHandVM>();
            bool needsTemps;
            if (SaveRoot!.TempSets.Count == 0)
                needsTemps = true;
            else
                needsTemps = false;
            2.Times(x =>
            {
                RummyDiceHandVM thisTemp = new RummyDiceHandVM(_thisMod!);
                thisTemp.Index = x; //try one based this time.
                TempSets.Add(thisTemp); //forgot to add to the list.
                if (needsTemps == true)
                    SaveRoot.TempSets.Add(new CustomBasicList<RummyDiceInfo>()); //i think.
            });
            CreateSets(); //i think
            IsLoaded = true; //i think needs to be here.
        }

        protected override async Task ComputerTurnAsync()
        {
            //if there is nothing, then just won't do anything.
            await Task.CompletedTask;
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            SaveRoot!.LoadMod(_thisMod!);
            SaveRoot.SomeoneFinished = false;
            SaveRoot.RollNumber = 1; //i think
            TempSets!.ForEach(thisSet =>
            {
                thisSet.HandList.Clear();
            });
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.HowManyRepeats = 0;
                thisPlayer.Phase = 1;
                thisPlayer.ScoreGame = 0;
                thisPlayer.ScoreRound = 0;
            });
            PrepStartTurn();
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        private async Task GameOverAsync()
        {
            _thisMod!.Score = 0;
            _thisMod.RollNumber = 0;
            if (PlayerList.Any(Items => Items.Phase == 11) == false)
                throw new BasicBlankException("Somebody had to complete the 10th phase in order to figure out who won");
            SingleInfo = PlayerList.Where(Items => Items.Phase == 11).OrderByDescending(Items => Items.ScoreGame).Take(1).Single();
            await ShowWinAsync();
        }

        private void CreateSets()
        {
            Rummys = new RummyProcesses<EnumColorType, EnumColorType, RummyDiceInfo>();
            Rummys.HasSecond = false;
            Rummys.HasWild = true;
            Rummys.LowNumber = 1;
            Rummys.HighNumber = 12;
            Rummys.NeedMatch = false;
            PhaseList thisPhase = new PhaseList();
            thisPhase.Description = "2 Sets of 3";
            SetInfo newSets;
            PhaseInfo = new CustomBasicList<PhaseList>();
            2.Times(x =>
            {
                newSets = new SetInfo();
                newSets.HowMany = 3;
                newSets.SetType = EnumWhatSets.Kinds;
                thisPhase.PhaseSets.Add(newSets);
            });
            PhaseInfo.Add(thisPhase);
            thisPhase = new PhaseList();
            thisPhase.Description = "1 Set of 3, 1 Run of 4";
            2.Times(x =>
            {
                newSets = new SetInfo();
                if (x == 2)
                {
                    newSets.HowMany = 3;
                    newSets.SetType = EnumWhatSets.Kinds;
                }
                else
                {
                    newSets.HowMany = 4;
                    newSets.SetType = EnumWhatSets.Runs;
                }
                thisPhase.PhaseSets.Add(newSets);
            });
            PhaseInfo.Add(thisPhase);
            thisPhase = new PhaseList();
            thisPhase.Description = "1 Set of 4, 1 Run of 4";
            2.Times(x =>
            {
                newSets = new SetInfo();
                newSets.HowMany = 4;
                if (x == 2)
                    newSets.SetType = EnumWhatSets.Kinds;
                else
                    newSets.SetType = EnumWhatSets.Runs;
                thisPhase.PhaseSets.Add(newSets);
            });
            PhaseInfo.Add(thisPhase);
            for (int x = 7; x <= 9; x++)
            {
                thisPhase = new PhaseList();
                newSets = new SetInfo();
                thisPhase.Description = "1 Run of " + x;
                newSets.SetType = EnumWhatSets.Runs;
                newSets.HowMany = x;
                thisPhase.PhaseSets.Add(newSets);
                PhaseInfo.Add(thisPhase);
            }
            thisPhase = new PhaseList();
            thisPhase.Description = "2 Sets Of 4";
            for (int x = 1; x <= 2; x++)
            {
                newSets = new SetInfo();
                newSets.HowMany = 4;
                newSets.SetType = EnumWhatSets.Kinds;
                thisPhase.PhaseSets.Add(newSets);
            }
            PhaseInfo.Add(thisPhase);
            thisPhase = new PhaseList();
            thisPhase.Description = "7 Cards Of 1 Color";
            newSets = new SetInfo();
            newSets.HowMany = 7;
            newSets.SetType = EnumWhatSets.Colors;
            thisPhase.PhaseSets.Add(newSets);
            PhaseInfo.Add(thisPhase);
            int Y;
            for (int x = 2; x <= 3; x++)
            {
                thisPhase = new PhaseList();
                thisPhase.Description = "1 Set Of 5, 1 Set Of " + x;
                for (Y = 1; Y <= 2; Y++)
                {
                    newSets = new SetInfo();
                    newSets.SetType = EnumWhatSets.Kinds;
                    if (Y == 2)
                        newSets.HowMany = x;
                    else
                        newSets.HowMany = 5;
                    thisPhase.PhaseSets.Add(newSets);
                }
                PhaseInfo.Add(thisPhase);
            }
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                //put in cases here.
                case "boardclicked":
                    await BoardProcessAsync();
                    return;
                case "calculate":
                    await DoCalculateAsync();
                    return;
                case "continueroll":
                    await ContinueRollAsync();
                    return;
                case "rolldice":
                    CustomBasicList<CustomBasicList<RummyDiceInfo>> thisList = await js.DeserializeObjectAsync<CustomBasicList<CustomBasicList<RummyDiceInfo>>>(content);
                    await ShowRollingAsync(thisList);
                    return;
                case "diceclicked":
                    await SelectOneMainAsync(int.Parse(content));
                    return;
                case "setchosen":
                    await SetProcessAsync(int.Parse(content));
                    return;
                case "diceset":
                    SendSet thisSend = await js.DeserializeObjectAsync<SendSet>(content);
                    RummyDiceHandVM thisTemp = TempSets.Single(Items => Items.Index == thisSend.WhichSet);
                    await thisTemp.SelectUnselectDiceAsync(thisSend.Dice);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }

        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            SaveRoot!.RollNumber = 1;
            _thisMod!.Score = 0;
            TempSets!.ForEach(thisSet =>
            {
                thisSet.HandList.Clear();
            });
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            int nums = CalculateScore();
            _thisMod!.Score = nums;
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            MainBoard1!.EndTurn();
            UpdateInfo(nums, out bool Overs);
            if (Overs == true)
            {
                await GameOverAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            await StartNewTurnAsync();
        }
        private void UpdateInfo(int score, out bool gameOver)
        {
            int previousScore;
            previousScore = SingleInfo!.ScoreGame;
            SingleInfo.ScoreRound = score;
            int newScore;
            newScore = previousScore + score;
            int phase;
            phase = SingleInfo.Phase;
            if (score == 0)
            {
                SingleInfo.HowManyRepeats++;
                if (SaveRoot!.SomeoneFinished == true)
                    SingleInfo.InGame = false;
                gameOver = PlayerList.All(Items => Items.InGame == false); //if everybody is out of the game, then its game over.
                return;
            }
            if (phase == 5 && newScore > 240)
                newScore += 40;
            SingleInfo.Phase++;
            if (phase == 10)
            {
                SaveRoot!.SomeoneFinished = true;
                SingleInfo.InGame = false;
                if (LeastRepeats() == true)
                    newScore += 40;
            }
            SingleInfo.ScoreGame = newScore;
            gameOver = PlayerList.All(Items => Items.InGame == false); //if everybody is out of the game, then its game over.
        }
        public async Task DoCalculateAsync()
        {
            int nums = CalculateScore();
            _thisMod!.Score = nums;
            await ContinueTurnAsync();
        }
        private int CalculateScore()
        {
            int phase = SingleInfo!.Phase;
            PhaseList ThisPhase = PhaseInfo![phase - 1]; //because 0 based.
            int output;
            CustomBasicList<RummyDiceInfo> thisCollection;
            CustomBasicCollection<RummyDiceInfo> tempCollection;
            output = 0;
            for (int x = 1; x <= 2; x++)
            {
                thisCollection = new CustomBasicList<RummyDiceInfo>();
                tempCollection = TempSets![x - 1].HandList;
                if (tempCollection.Count > 0)
                    thisCollection.AddRange(tempCollection);
                if (thisCollection.Count > 0)
                {
                    foreach (var newSet in ThisPhase.PhaseSets)
                    {
                        if (newSet.DidSucceed == false)
                        {
                            newSet.DidSucceed = newSet.SetType switch
                            {
                                EnumWhatSets.Colors => Rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumColorType, EnumColorType, RummyDiceInfo>.EnumRummyType.Colors),
                                EnumWhatSets.Kinds => Rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumColorType, EnumColorType, RummyDiceInfo>.EnumRummyType.Sets),
                                EnumWhatSets.Runs => Rummys!.IsNewRummy(thisCollection, newSet.HowMany, RummyProcesses<EnumColorType, EnumColorType, RummyDiceInfo>.EnumRummyType.Runs),
                                _ => throw new BasicBlankException("Not Supported"),
                            };
                            if (newSet.DidSucceed == true)
                            {
                                output += thisCollection.Sum(Items => Items.Value);
                                break;
                            }
                        }
                    }
                }
            }
            if (ThisPhase.PhaseSets.Any(Items => Items.DidSucceed == false))
            {
                ResetSuccess();
                return 0;
            }
            ResetSuccess();
            return output;
        }
        private void ResetSuccess()
        {
            PhaseInfo!.ForEach(thisPhase =>
            {
                thisPhase.PhaseSets.ForEach(thisSet => thisSet.DidSucceed = false);
            });
        }
        private bool LeastRepeats()
        {
            int repeats = SingleInfo!.HowManyRepeats;
            return !PlayerList.Any(items => items.HowManyRepeats < repeats);
        }
        public async Task RollDiceAsync()
        {
            var thisCol = MainBoard1!.RollDice();
            if (ThisData!.MultiPlayer == true)
            {
                if (thisCol.Count > 0)
                    await ThisNet!.SendAllAsync("rolldice", thisCol);
                else
                    await ThisNet!.SendAllAsync("continueroll");
            }
            await ShowRollingAsync(thisCol); //forgot this too.
        }
        private async Task ShowRollingAsync(CustomBasicList<CustomBasicList<RummyDiceInfo>> thisList)
        {
            await MainBoard1!.ShowRollingAsync(thisList);
            await ContinueRollAsync();
        }
        private async Task ContinueRollAsync()
        {
            SaveRoot!.RollNumber++;
            await ContinueTurnAsync();
        }
        public async Task BoardProcessAsync()
        {
            CustomBasicList<RummyDiceInfo> thisCol = new CustomBasicList<RummyDiceInfo>();
            TempSets!.ForEach(items =>
            {
                thisCol.AddRange(items.GetSelectedDiceAndRemove());
            });
            MainBoard1!.AddBack(thisCol);
            await ContinueTurnAsync();
        }
        public async Task SetProcessAsync(int whichOne)
        {
            CustomBasicList<RummyDiceInfo> thisCol = new CustomBasicList<RummyDiceInfo>();
            thisCol.AddRange(MainBoard1!.GetSelectedList());
            TempSets!.ForConditionalItems(items => items.Index != whichOne, Items =>
            {
                thisCol.AddRange(Items.GetSelectedDiceAndRemove());
            });
            RummyDiceHandVM fins = TempSets.Single(items => items.Index == whichOne);
            fins.TransferTiles(thisCol);
            await ContinueTurnAsync();
        }
        public async Task SelectOneMainAsync(int whichOne)
        {
            MainBoard1!.SelectDice(whichOne);
            await ContinueTurnAsync();
        }
    }
}