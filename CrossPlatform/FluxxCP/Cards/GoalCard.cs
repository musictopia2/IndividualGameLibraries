using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Containers;
using FluxxCP.Data;
using System;
using System.Linq;

namespace FluxxCP.Cards
{
    public class GoalCard : FluxxCardInformation
    {
        public new EnumGoalRegular Deck
        {
            get
            {
                return (EnumGoalRegular)base.Deck;
            }
            set
            {
                base.Deck = (int)value;
            }
        }
        public GoalCard()
        {
            CardType = EnumCardType.Goal;
        }
        public int HowMany()
        {
            if (Deck == EnumGoalRegular.Keepers)
                return 5;
            if (Deck == EnumGoalRegular.CardsInHand)
                return 10;
            return 0;
        }
        public override bool IncreaseOne()
        {
            if (HowMany() > 0)
                return true;
            return base.IncreaseOne();
        }
        private EnumSpecialGoalSpecial SpecialGoal()
        {
            int count = HowMany();
            if (count == 5)
                return EnumSpecialGoalSpecial.Keepers;
            if (count == 10)
                return EnumSpecialGoalSpecial.Hand;
            return EnumSpecialGoalSpecial.None;
        }
        private CustomBasicList<Tuple<EnumKeeper, bool>> GetRegularGoals(EnumKeeper Keeper1, EnumKeeper Keeper2)
        {
            CustomBasicList<Tuple<EnumKeeper, bool>> ThisList = new CustomBasicList<Tuple<EnumKeeper, bool>>
            {
                new Tuple<EnumKeeper, bool>(Keeper1, false),
                new Tuple<EnumKeeper, bool>(Keeper2, false)
            };
            return ThisList;
        }
        private Tuple<EnumSpecialGoalSpecial, CustomBasicList<Tuple<EnumKeeper, bool>>>? _goal;
        public override void Populate(int Chosen)
        {
            Deck = (EnumGoalRegular)Chosen;
            PopulateDescription();
            PopulateGoal();
        }
        public void PopulateGoal()
        {
            _goal = PrivateGetGoalInfo();
        }
        private Tuple<EnumSpecialGoalSpecial, CustomBasicList<Tuple<EnumKeeper, bool>>> PrivateGetGoalInfo()
        {
            CustomBasicList<Tuple<EnumKeeper, bool>> ThisList = new CustomBasicList<Tuple<EnumKeeper, bool>>();
            EnumSpecialGoalSpecial ThisSpecial;
            ThisSpecial = SpecialGoal();
            switch (Deck)
            {
                case EnumGoalRegular.AllYouNeedIsLove:
                    {
                        ThisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.Love, false));
                        break;
                    }

                case EnumGoalRegular.BakedGoods:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Bread, EnumKeeper.Cookies);
                        break;
                    }

                case EnumGoalRegular.BedTime:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Sleep, EnumKeeper.Time);
                        break;
                    }

                case EnumGoalRegular.ChocolateCookies:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Chocolate, EnumKeeper.Cookies);
                        break;
                    }

                case EnumGoalRegular.ChocolateMilk:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Chocolate, EnumKeeper.Milk);
                        break;
                    }

                case EnumGoalRegular.DeathByChocolate:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Death, EnumKeeper.Chocolate);
                        break;
                    }

                case EnumGoalRegular.DreamLand:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Sleep, EnumKeeper.Dreams);
                        break;
                    }

                case EnumGoalRegular.HeartsAndMinds:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Love, EnumKeeper.TheBrain);
                        break;
                    }

                case EnumGoalRegular.Hippyism:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Peace, EnumKeeper.Love);
                        break;
                    }

                case EnumGoalRegular.MilkAndCookies:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Milk, EnumKeeper.Cookies);
                        break;
                    }

                case EnumGoalRegular.NightAndDay:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.TheMoon, EnumKeeper.TheSun);
                        break;
                    }

                case EnumGoalRegular.PeaceNoWar:
                    {
                        ThisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.Peace, false));
                        ThisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.War, true));
                        break;
                    }

                case EnumGoalRegular.RocketScience:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.TheRocket, EnumKeeper.TheBrain);
                        break;
                    }

                case EnumGoalRegular.RocketToTheMoon:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.TheRocket, EnumKeeper.TheMoon);
                        break;
                    }

                case EnumGoalRegular.SquishyChocolate:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Chocolate, EnumKeeper.TheSun);
                        break;
                    }

                case EnumGoalRegular.TheAppliances:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Television, EnumKeeper.TheToaster);
                        break;
                    }

                case EnumGoalRegular.TheBrainNoTV:
                    {
                        ThisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.TheBrain, false));
                        ThisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.Television, true));
                        break;
                    }

                case EnumGoalRegular.TimeIsMoney:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Time, EnumKeeper.Money);
                        break;
                    }

                case EnumGoalRegular.Toast:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Bread, EnumKeeper.TheToaster);
                        break;
                    }

                case EnumGoalRegular.WinningTheLottery:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.Dreams, EnumKeeper.Money);
                        break;
                    }

                case EnumGoalRegular.WarDeath:
                    {
                        ThisList = GetRegularGoals(EnumKeeper.War, EnumKeeper.Death);
                        break;
                    }
            }
            return new Tuple<EnumSpecialGoalSpecial, CustomBasicList<Tuple<EnumKeeper, bool>>>(ThisSpecial, ThisList);
        }
        public int WhoWon(int HowManyExtra, FluxxGameContainer gameContainer)
        {
            if (_goal == null)
                PopulateGoal(); //so it works with autoresume too.
            int count;
            CustomBasicList<FluxxPlayerItem> tempList;
            if (_goal!.Item1 != EnumSpecialGoalSpecial.None)
            {
                count = HowMany();
                count += HowManyExtra;
                if (_goal.Item1 == EnumSpecialGoalSpecial.Hand)
                    tempList = gameContainer.PlayerList.Where(items => items.MainHandList.Count >= count).OrderByDescending(items => items.MainHandList.Count()).Take(2).ToCustomBasicList();
                else
                    tempList = gameContainer.PlayerList.Where(items => items.KeeperList.Count >= count).OrderByDescending(items => items.KeeperList.Count).Take(2).ToCustomBasicList();
                if (tempList.Count == 0)
                    return 0;
                if (tempList.Count == 1)
                    return tempList.Single().Id;
                if (_goal.Item1 == EnumSpecialGoalSpecial.Hand && tempList.First().MainHandList.Count == tempList.Last().MainHandList.Count)
                    return 0;
                if (_goal.Item1 == EnumSpecialGoalSpecial.Keepers && tempList.First().KeeperList.Count == tempList.Last().KeeperList.Count)
                    return 0;
                return tempList.First().Id;
            }
            if (_goal.Item2.Count == 0)
                throw new BasicBlankException("Must have at least one goal since its not keepers or in hand");
            if (_goal.Item2.Count > 2)
                throw new BasicBlankException("Can have a maximum of 2 keepers to reach a goal");
            FluxxPlayerItem thisPlayer;
            if (_goal.Item2.Count == 1)
            {
                if (Deck != EnumGoalRegular.AllYouNeedIsLove)
                    throw new BasicBlankException("Only all you need is love can have only one goal to win");
                thisPlayer = gameContainer.PlayerList.Where(items => items.KeeperList.Count == 1 && items.KeeperList.Single().Deck == EnumKeeper.Love).SingleOrDefault();
            }
            else
            {
                if (_goal.Item2.Last().Item2 == true)
                {
                    if (gameContainer.PlayerList.Any(tempPlayer => tempPlayer.KeeperList.Any(thisKeeper => thisKeeper.Deck == _goal.Item2.Last().Item1)))
                        return 0;
                    thisPlayer = gameContainer.PlayerList.Where(tempPlayer => tempPlayer.KeeperList.Any(thisKeeper => thisKeeper.Deck == _goal.Item2.First().Item1)).SingleOrDefault();
                }
                else
                {
                    thisPlayer = gameContainer.PlayerList.Where(tempPlayer => tempPlayer.KeeperList.Any
                        (thisKeeper => thisKeeper.Deck == _goal.Item2.First().Item1) && tempPlayer.KeeperList.Any
                        (thisKeeper => thisKeeper.Deck == _goal.Item2.Last().Item1)).SingleOrDefault();
                }
            }
            if (thisPlayer == null)
                return 0;
            return thisPlayer.Id;
        }
    }
}
