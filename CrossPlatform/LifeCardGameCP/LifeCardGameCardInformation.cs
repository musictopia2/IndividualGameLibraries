using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
namespace LifeCardGameCP
{
    public class LifeCardGameCardInformation : SimpleDeckObject, IDeckObject, IComparable<LifeCardGameCardInformation>
    {

        private int _Points;
        public int Points
        {
            get { return _Points; }
            set
            {
                if (SetProperty(ref _Points, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public bool TimeFlying => Points == 0;
        private EnumAction _Action;
        public EnumAction Action
        {
            get { return _Action; }
            set
            {
                if (SetProperty(ref _Action, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumSpecialCardCategory _Requirement;
        public EnumSpecialCardCategory Requirement
        {
            get { return _Requirement; }
            set
            {
                if (SetProperty(ref _Requirement, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumSpecialCardCategory _SpecialCategory;
        public EnumSpecialCardCategory SpecialCategory
        {
            get { return _SpecialCategory; }
            set
            {
                if (SetProperty(ref _SpecialCategory, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumFirstCardCategory _Category;
        public EnumFirstCardCategory Category
        {
            get { return _Category; }
            set
            {
                if (SetProperty(ref _Category, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _Description = "";
        public string Description
        {
            get { return _Description; }
            set
            {
                if (SetProperty(ref _Description, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _OpponentKeepsCard;
        public bool OpponentKeepsCard
        {
            get { return _OpponentKeepsCard; }
            set
            {
                if (SetProperty(ref _OpponentKeepsCard, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumSwitchCategory _SwitchCategory;
        public EnumSwitchCategory SwitchCategory
        {
            get { return _SwitchCategory; }
            set
            {
                if (SetProperty(ref _SwitchCategory, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public EnumSwitchCategory CardToSwitch { get; set; }
        public bool CanBeInPlayerHandToBeginWith { get; set; } //hopefully can find a way to work around this one.
        public bool OnlyOneAllowed()
        {
            return SpecialCategory == EnumSpecialCardCategory.Marriage || SpecialCategory == (EnumSpecialCardCategory.Passport);
        }
        public bool IsPayday()
        {
            return Points == 20 && Category == EnumFirstCardCategory.Career;
        }
        public LifeCardGameCardInformation()
        {
            DefaultSize = new SKSize(100, 115);
        }
        public void Populate(int chosen)
        {
            int x;
            int y;
            int z = 0;
            Points = 0; //until proven otherwise.
            Deck = chosen;
            CanBeInPlayerHandToBeginWith = true; //has to be proven false.
            for (x = 1; x <= 4; x++)
            {
                for (y = 1; y <= 27; y++)
                {
                    z += 1;
                    if (z == Deck)
                    {
                        Category = (EnumFirstCardCategory)x;
                        if (y <= 2)
                        {
                            CanBeInPlayerHandToBeginWith = false;
                            return;
                        }
                        if (y == 3)
                        {
                            Action = EnumAction.Lawsuit;
                            SpecialCategory = EnumSpecialCardCategory.Switch;
                            OpponentKeepsCard = true;
                            Description = "Sue another player.|They must give you|a card with 30+|from their life store.";
                            Points = 5;
                            return;
                        }
                        switch (x)
                        {
                            case 1:
                                {
                                    switch (y)
                                    {
                                        case 4:
                                            {
                                                Action = EnumAction.IMTheBoss;
                                                Description = "Take a payday from|another player";
                                                Points = 5;
                                                OpponentKeepsCard = true;
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                break;
                                            }

                                        case 5:
                                            {
                                                Action = EnumAction.YoureFired;
                                                Description = "Discard another|player's Payday|card.";
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                Points = 5;
                                                OpponentKeepsCard = true;
                                                break;
                                            }

                                        case 6:
                                            {
                                                Action = EnumAction.TurnBackTime;
                                                Description = "Return a +10 card|randomly to the|deck and reshuffle"; // iffy
                                                Points = 5;
                                                break;
                                            }

                                        case 7:
                                            {
                                                Action = EnumAction.CareerSwap;
                                                Description = "Swap a played|career card with|another player.";
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                Points = 5;
                                                OpponentKeepsCard = true;
                                                break;
                                            }

                                        case object _ when y <= 11:
                                            {
                                                SpecialCategory = EnumSpecialCardCategory.Degree;
                                                Description = "Degree";
                                                Points = 10;
                                                break;
                                            }

                                        case object _ when y <= 20:
                                            {
                                                Points = 20;
                                                Description = "Payday|(3 paydays per|career)";
                                                break;
                                            }

                                        case 21:
                                            {
                                                Points = 30;
                                                Description = "Stunt Double";
                                                SwitchCategory = EnumSwitchCategory.Career;
                                                break;
                                            }

                                        case 22:
                                            {
                                                Points = 40;
                                                Description = "Pop Star";
                                                SwitchCategory = EnumSwitchCategory.Career;
                                                break;
                                            }

                                        case 23:
                                            {
                                                Points = 50;
                                                Description = "Jet Pilot";
                                                SwitchCategory = EnumSwitchCategory.Career;
                                                break;
                                            }

                                        case 24:
                                            {
                                                Points = 60;
                                                Description = "Teacher";
                                                Requirement = EnumSpecialCardCategory.Degree;
                                                SwitchCategory = EnumSwitchCategory.Career;
                                                break;
                                            }

                                        case 25:
                                            {
                                                Points = 70;
                                                Description = "Exotic Pet Vet";
                                                Requirement = EnumSpecialCardCategory.Degree;
                                                SwitchCategory = EnumSwitchCategory.Career;
                                                break;
                                            }

                                        case 26:
                                            {
                                                Points = 80;
                                                Description = "Rocket Scientist";
                                                Requirement = EnumSpecialCardCategory.Degree;
                                                SwitchCategory = EnumSwitchCategory.Career;
                                                break;
                                            }

                                        case 27:
                                            {
                                                Points = 90;
                                                Description = "Politician";
                                                Requirement = EnumSpecialCardCategory.Degree;
                                                SwitchCategory = EnumSwitchCategory.Career;
                                                break;
                                            }

                                        default:
                                            {
                                                throw new BasicBlankException("Don't know what to do about this career card");
                                            }
                                    }

                                    break;
                                }

                            case 2:
                                {
                                    switch (y)
                                    {
                                        case 4:
                                            {
                                                Points = 5;
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                Description = "Take a Baby card|from another|player's life story."; // iffy
                                                OpponentKeepsCard = true;
                                                Action = EnumAction.AdoptBaby;
                                                CardToSwitch = EnumSwitchCategory.Baby;
                                                break;
                                            }

                                        case 5:
                                            {
                                                Points = 5;
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                Description = "Take a Family card|from another|player's life story.";
                                                OpponentKeepsCard = true;
                                                Action = EnumAction.LongLostRelative;
                                                break;
                                            }

                                        case 6:
                                            {
                                                Points = 5;
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                Description = "Swap your whole|hand with another|player.";
                                                OpponentKeepsCard = true;
                                                Action = EnumAction.MidlifeCrisis;
                                                break;
                                            }

                                        case 7:
                                            {
                                                Points = 5;
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                Description = "Swap a played Pet|card with another|player.";
                                                OpponentKeepsCard = true;
                                                Action = EnumAction.MixUpAtVets;
                                                CardToSwitch = EnumSwitchCategory.Pet;
                                                break;
                                            }

                                        case 8:
                                            {
                                                Points = 10;
                                                Description = "Monkey";
                                                SwitchCategory = EnumSwitchCategory.Pet;
                                                break;
                                            }

                                        case 9:
                                            {
                                                Points = 10;
                                                Description = "Shark";
                                                SwitchCategory = EnumSwitchCategory.Pet;
                                                break;
                                            }

                                        case 10:
                                            {
                                                Points = 20;
                                                Description = "Giraffe";
                                                SwitchCategory = EnumSwitchCategory.Pet;
                                                break;
                                            }

                                        case 11:
                                            {
                                                Points = 20;
                                                Description = "Baby polar bear";
                                                SwitchCategory = EnumSwitchCategory.Pet;
                                                break;
                                            }

                                        case 12:
                                            {
                                                Points = 30;
                                                Description = "Lion";
                                                SwitchCategory = EnumSwitchCategory.Pet;
                                                break;
                                            }

                                        case 13:
                                            {
                                                Points = 50;
                                                Description = "Vegas wedding";
                                                SpecialCategory = EnumSpecialCardCategory.Marriage;
                                                break;
                                            }

                                        case 14:
                                            {
                                                Points = 50;
                                                Description = "Celebrity wedding";
                                                SpecialCategory = EnumSpecialCardCategory.Marriage;
                                                break;
                                            }

                                        case 15:
                                            {
                                                Points = 50;
                                                Description = "Underwater wedding";
                                                SpecialCategory = EnumSpecialCardCategory.Marriage;
                                                break;
                                            }

                                        case 16:
                                            {
                                                Points = 50;
                                                Description = "Parachute wedding";
                                                SpecialCategory = EnumSpecialCardCategory.Marriage;
                                                break;
                                            }

                                        case 17:
                                            {
                                                Points = 50;
                                                Description = "Beach wedding";
                                                SpecialCategory = EnumSpecialCardCategory.Marriage;
                                                break;
                                            }

                                        case 18:
                                            {
                                                Points = 50;
                                                Description = "Fairytale wedding";
                                                SpecialCategory = EnumSpecialCardCategory.Marriage;
                                                break;
                                            }

                                        case 19:
                                            {
                                                Points = 20;
                                                Description = "Golden|anniversary";
                                                Requirement = EnumSpecialCardCategory.Marriage;
                                                break;
                                            }

                                        case 20:
                                            {
                                                Points = 30;
                                                Description = "Diamond|anniversary";
                                                Requirement = EnumSpecialCardCategory.Marriage;
                                                break;
                                            }

                                        case 21:
                                            {
                                                Points = 20;
                                                Description = "Baby girl";
                                                Requirement = EnumSpecialCardCategory.Marriage;
                                                SwitchCategory = EnumSwitchCategory.Baby;
                                                break;
                                            }

                                        case 22:
                                            {
                                                Points = 60;
                                                Description = "Baby triplets";
                                                Requirement = EnumSpecialCardCategory.Marriage;
                                                SwitchCategory = EnumSwitchCategory.Baby;
                                                break;
                                            }

                                        case 23:
                                            {
                                                Points = 20;
                                                Description = "Baby boy";
                                                Requirement = EnumSpecialCardCategory.Marriage;
                                                SwitchCategory = EnumSwitchCategory.Baby;
                                                break;
                                            }

                                        case 24:
                                            {
                                                Points = 20;
                                                Description = "Baby boy";
                                                Requirement = EnumSpecialCardCategory.Marriage;
                                                SwitchCategory = EnumSwitchCategory.Baby;
                                                break;
                                            }

                                        case 25:
                                            {
                                                Points = 20;
                                                Description = "Baby girl";
                                                Requirement = EnumSpecialCardCategory.Marriage;
                                                SwitchCategory = EnumSwitchCategory.Baby;
                                                break;
                                            }

                                        case 26:
                                            {
                                                Points = 40;
                                                Description = "Baby girl twins";
                                                Requirement = EnumSpecialCardCategory.Marriage;
                                                SwitchCategory = EnumSwitchCategory.Baby;
                                                break;
                                            }

                                        case 27:
                                            {
                                                Points = 40;
                                                Description = "Baby boy twins";
                                                Requirement = EnumSpecialCardCategory.Marriage;
                                                SwitchCategory = EnumSwitchCategory.Baby;
                                                break;
                                            }

                                        default:
                                            {
                                                throw new BasicBlankException("Don't know what to do about this family card");
                                            }
                                    }

                                    break;
                                }

                            case 3:
                                {
                                    switch (y)
                                    {
                                        case 4:
                                            {
                                                Points = 5;
                                                OpponentKeepsCard = true;
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                Description = "Discard another|player's passwort.";
                                                Action = EnumAction.LostPassport;
                                                break;
                                            }

                                        case 5:
                                            {
                                                Points = 5;
                                                OpponentKeepsCard = true;
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                Description = "Take an Adventure|card from another|player's life story.";
                                                Action = EnumAction.YourStory;
                                                break;
                                            }

                                        case 6:
                                            {
                                                Points = 5;
                                                Action = EnumAction.LifeSwap;
                                                Description = "Choose another|player.  Switch 2|cards at random.";
                                                break;
                                            }

                                        case 7:
                                            {
                                                Points = 5;
                                                Action = EnumAction.SecondChance;
                                                Description = "Take any card from|another player's life|story worth 10 to|30 points.";
                                                OpponentKeepsCard = true;
                                                break;
                                            }

                                        case 8:
                                            {
                                                Points = 10;
                                                Description = "Learn to|play the bonjos";
                                                break;
                                            }

                                        case 9:
                                            {
                                                Points = 10;
                                                Description = "See a solar eclipse";
                                                break;
                                            }

                                        case 10:
                                            {
                                                Points = 20;
                                                Description = "Go diving in|Niagara Falls";
                                                break;
                                            }

                                        case 11:
                                            {
                                                Points = 20;
                                                Description = "Headline at|a rock concert";
                                                break;
                                            }

                                        case 12:
                                            {
                                                Points = 20;
                                                Description = "Find a message|in a bottle";
                                                break;
                                            }

                                        case 13:
                                            {
                                                Points = 30;
                                                Description = "Go Skydiving";
                                                break;
                                            }

                                        case 14:
                                            {
                                                Points = 30;
                                                Description = "Ride the tallest|rollercoaster in|the world";
                                                break;
                                            }

                                        case 15:
                                            {
                                                Points = 40;
                                                Description = "Swim with dolphins";
                                                break;
                                            }

                                        case 16:
                                            {
                                                Points = 40;
                                                Description = "Win a charity|skateboard contest";
                                                break;
                                            }

                                        case 17:
                                            {
                                                Points = 50;
                                                Description = "Win the jackpot";
                                                break;
                                            }

                                        case 18:
                                            {
                                                Points = 60;
                                                Description = "Fly high in a|hot-air balloon";
                                                break;
                                            }

                                        case 19:
                                            {
                                                Points = 20;
                                                Description = "Dance at the Rio|Carnival";
                                                Requirement = EnumSpecialCardCategory.Passport;
                                                break;
                                            }

                                        case 20:
                                            {
                                                Points = 20;
                                                Description = "Trek to the North|Pole";
                                                Requirement = EnumSpecialCardCategory.Passport;
                                                break;
                                            }

                                        case 21:
                                            {
                                                Points = 30;
                                                Description = "Explore a|live volcano";
                                                Requirement = EnumSpecialCardCategory.Passport;
                                                break;
                                            }

                                        case 22:
                                            {
                                                Points = 30;
                                                Description = "Dig up dinosaur|fossils";
                                                Requirement = EnumSpecialCardCategory.Passport;
                                                break;
                                            }

                                        case 23:
                                            {
                                                Points = 40;
                                                Description = "Travel to|the Moon|in a rocket";
                                                Requirement = EnumSpecialCardCategory.Passport;
                                                break;
                                            }

                                        case 24:
                                            {
                                                Points = 50;
                                                Description = "Find Big Foot";
                                                Requirement = EnumSpecialCardCategory.Passport;
                                                break;
                                            }

                                        case 25:
                                            {
                                                Points = 20;
                                                Description = "Win the Jungle|Safari Rally";
                                                Requirement = EnumSpecialCardCategory.Car;
                                                break;
                                            }

                                        case 26:
                                            {
                                                Points = 30;
                                                Description = "Sail solo around|the world";
                                                Requirement = EnumSpecialCardCategory.Boat;
                                                break;
                                            }

                                        case 27:
                                            {
                                                Points = 40;
                                                Description = "Learn to|loop-the-loop";
                                                Requirement = EnumSpecialCardCategory.Airplane;
                                                break;
                                            }

                                        default:
                                            {
                                                throw new BasicBlankException("Don't know what to do about this adventure card");
                                            }
                                    }

                                    break;
                                }

                            case 4:
                                {
                                    switch (y)
                                    {
                                        case 4:
                                            {
                                                SpecialCategory = EnumSpecialCardCategory.Switch; // this will have otherturn
                                                Points = 5;
                                                OpponentKeepsCard = true;
                                                Description = "Choose another|player.  They must|discard one Wealth|card.";
                                                Action = EnumAction.DonateToCharity;
                                                break;
                                            }

                                        case object _ when y <= 7:
                                            {
                                                SpecialCategory = EnumSpecialCardCategory.Switch;
                                                Points = 5;
                                                OpponentKeepsCard = true;
                                                Description = "Swap a played|House card with|another player.";
                                                Requirement = EnumSpecialCardCategory.House;
                                                Action = EnumAction.MovingHouse;
                                                break;
                                            }

                                        case object _ when y <= 12:
                                            {
                                                Points = 10;
                                                Description = "Passport";
                                                SpecialCategory = EnumSpecialCardCategory.Passport;
                                                break;
                                            }

                                        case 13:
                                            {
                                                Points = 10;
                                                SpecialCategory = EnumSpecialCardCategory.Car;
                                                Description = "Pink Cadillac";
                                                break;
                                            }

                                        case 14:
                                            {
                                                Points = 10;
                                                SpecialCategory = EnumSpecialCardCategory.Car;
                                                Description = "Eco-bubble car";
                                                break;
                                            }

                                        case 15:
                                            {
                                                Points = 20;
                                                SpecialCategory = EnumSpecialCardCategory.Boat;
                                                Description = "Racing yacht";
                                                break;
                                            }

                                        case 16:
                                            {
                                                Points = 20;
                                                SpecialCategory = EnumSpecialCardCategory.Boat;
                                                Description = "Bathtub boat";
                                                break;
                                            }

                                        case 17:
                                            {
                                                Points = 30;
                                                SpecialCategory = EnumSpecialCardCategory.Airplane;
                                                Description = "Private jet";
                                                break;
                                            }

                                        case 18:
                                            {
                                                Points = 30;
                                                SpecialCategory = EnumSpecialCardCategory.House;
                                                Description = "Treehouse";
                                                break;
                                            }

                                        case 19:
                                            {
                                                Points = 40;
                                                SpecialCategory = EnumSpecialCardCategory.House;
                                                Description = "Igloo";
                                                break;
                                            }

                                        case 20:
                                            {
                                                Points = 50;
                                                SpecialCategory = EnumSpecialCardCategory.House;
                                                Description = "Lighthouse";
                                                break;
                                            }

                                        case 21:
                                            {
                                                Points = 60;
                                                SpecialCategory = EnumSpecialCardCategory.House;
                                                Description = "Beach house";
                                                break;
                                            }

                                        case 22:
                                            {
                                                Points = 70;
                                                SpecialCategory = EnumSpecialCardCategory.House;
                                                Description = "Ranch";
                                                break;
                                            }

                                        case 23:
                                            {
                                                Points = 80;
                                                SpecialCategory = EnumSpecialCardCategory.House;
                                                Description = "Eco house";
                                                break;
                                            }

                                        case 24:
                                            {
                                                Points = 90;
                                                SpecialCategory = EnumSpecialCardCategory.House;
                                                Description = "Castle";
                                                break;
                                            }

                                        case 25:
                                            {
                                                Points = 20;
                                                Requirement = EnumSpecialCardCategory.House;
                                                Description = "Build a|swimming pool";
                                                break;
                                            }

                                        case 26:
                                            {
                                                Points = 30;
                                                Requirement = EnumSpecialCardCategory.House;
                                                Description = "Switch to|natural power";
                                                break;
                                            }

                                        case 27:
                                            {
                                                Points = 40;
                                                Requirement = EnumSpecialCardCategory.House;
                                                Description = "Build a multi-screen|cinema";
                                                break;
                                            }

                                        default:
                                            {
                                                throw new Exception("Don't know what to do about this wealth card");
                                            }
                                    }
                                    break;
                                }
                        }
                        return;
                    }
                }
            }
            throw new BasicBlankException("Can't find the deck " + Deck);
        }
        public void Reset() { }
        int IComparable<LifeCardGameCardInformation>.CompareTo(LifeCardGameCardInformation other)
        {
            if (Category != other.Category)
                return Category.CompareTo(other.Category);
            if (SpecialCategory != other.SpecialCategory)
                return SpecialCategory.CompareTo(other.SpecialCategory);
            if (SwitchCategory != other.SwitchCategory)
                return SwitchCategory.CompareTo(other.SwitchCategory);
            if (Requirement != other.Requirement)
                return Requirement.CompareTo(other.Requirement);
            return Points.CompareTo(other.Points);
        }
    }
}