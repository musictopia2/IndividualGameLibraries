using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static LifeBoardGameCP.CardsModule;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace LifeBoardGameCP
{
    [SingletonGame]
    public class LifeBoardGameMainGameClass : SimpleBoardGameClass<EnumColorChoice, CarPieceCP,
        LifeBoardGamePlayerItem, LifeBoardGameSaveInfo, int>, IMiscDataNM
    {
        internal EnumWhatStatus GameStatus
        {
            get
            {
                return SaveRoot!.GameStatus;
            }
            set
            {
                SaveRoot!.GameStatus = value;
            }
        }
        public LifeBoardGameMainGameClass(IGamePackageResolver container) : base(container) { }

        internal LifeBoardGameViewModel? ThisMod;
        internal RandomGenerator? Rs;
        private GlobalVariableClass? _thisGlobal;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<LifeBoardGameViewModel>();
            Rs = MainContainer.Resolve<RandomGenerator>();
            _thisGlobal = MainContainer.Resolve<GlobalVariableClass>(); //risk here.
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            SaveRoot!.LoadMod(ThisMod!);
            if (DidChooseColors)
            {
                ThisMod!.GameBoard1!.LoadSavedGame();
                if (GameStatus == EnumWhatStatus.NeedChooseGender)
                    ThisMod.RepaintGenders();
                PlayerList!.ForEach(thisPlayer =>
                {
                    var thisList = thisPlayer.Hand.ToRegularDeckDict();
                    thisPlayer.Hand.Clear();
                    thisList.ForEach(tempCard =>
                    {
                        if (tempCard.Deck <= 9)
                            thisPlayer.Hand.Add(GetCareerCard(tempCard.Deck));
                        else if (tempCard.Deck <= 18)
                            thisPlayer.Hand.Add(GetHouseCard(tempCard.Deck));
                        else if (tempCard.Deck <= 27)
                            thisPlayer.Hand.Add(GetSalaryCard(tempCard.Deck));
                        else if (tempCard.Deck <= 36)
                            thisPlayer.Hand.Add(GetStockCard(tempCard.Deck));
                        else
                            throw new BasicBlankException("Must be between 1 and 36");
                    });
                });
                if (SaveRoot.ChangePosition > 0)
                {
                    _thisGlobal!.SpinnerCanvas!.Position = SaveRoot.ChangePosition;
                    _thisGlobal.SpinnerCanvas.Repaint();
                }
                if (GameStatus == EnumWhatStatus.NeedToEndTurn)
                {
                    SingleInfo = PlayerList.GetWhoPlayer();
                    if (SingleInfo.Position > 0 && SingleInfo.LastMove == EnumFinal.None)
                        ThisMod.GameDetails = SpaceDescription(SingleInfo.Position);
                }
            }
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            PopulateSpaces();
            IsLoaded = true; //i think needs to be here.
        }
        private void PopulateSpaces()
        {
            _thisGlobal!.SpaceList = new CustomBasicList<SpaceInfo>();
            SpaceInfo thisspace;
            for (var x = 1; x <= 147; x++)
            {
                thisspace = new SpaceInfo();
                switch (x)
                {
                    case 1:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 20000;
                            thisspace.Description = "Scholarship!";
                            break;
                        }

                    case 2:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Buy books and supplies.";
                            break;
                        }

                    case 3:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Make new friends.";
                            break;
                        }

                    case 4:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 5000;
                            thisspace.Description = "Part time job.";
                            break;
                        }

                    case 5:
                        {
                            thisspace.ActionInfo = EnumActionType.WillMissTurn;
                            thisspace.Description = "Study for exams.";
                            break;
                        }

                    case 6:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Study abroad.";
                            break;
                        }

                    case 7:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Spring Break!";
                            break;
                        }

                    case 8:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Dean's List!";
                            break;
                        }

                    case 9:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Your buddies crash your car.";
                            thisspace.WhatInsurance = EnumInsuranceType.NeedCar;
                            break;
                        }

                    case 10:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Graduation day!";
                            break;
                        }

                    case 11:
                        {
                            thisspace.ActionInfo = EnumActionType.StartCareer;
                            thisspace.Description = "CAREER CHOICE!";
                            break;
                        }

                    case 12:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 13:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Rent apartment.";
                            break;
                        }

                    case 14:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 10000;
                            thisspace.Description = "Inheritance.";
                            break;
                        }

                    case 15:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 16:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Adopt a pet.";
                            break;
                        }

                    case 17:
                        {
                            thisspace.ActionInfo = EnumActionType.WillMissTurn;
                            thisspace.Description = "Lost!";
                            break;
                        }

                    case 18:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Birthday Party!";
                            break;
                        }

                    case 19:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.CareerSpace = EnumCareerType.Doctor;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Ski accident";
                            break;
                        }

                    case 20:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 10000;
                            thisspace.Description = "Win marathan";
                            break;
                        }

                    case 21:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Visit a muesum.";
                            break;
                        }

                    case 22:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Cycle to work.";
                            break;
                        }

                    case 23:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 24:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -15000;
                            thisspace.Description = "Car rolls away.";
                            thisspace.WhatInsurance = EnumInsuranceType.NeedCar;
                            break;
                        }

                    case 25:
                        {
                            thisspace.ActionInfo = EnumActionType.GetMarried;
                            break;
                        }

                    case 26:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -10000;
                            thisspace.Description = "Wedding reception.";
                            break;
                        }

                    case 27:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Happy honeymoon!";
                            break;
                        }

                    case 28:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -10000;
                            thisspace.CareerSpace = EnumCareerType.SalesPerson;
                            thisspace.Description = "Upgrade computer.";
                            break;
                        }

                    case 29:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -10000;
                            thisspace.Description = "Car accident";
                            thisspace.WhatInsurance = EnumInsuranceType.NeedCar;
                            break;
                        }

                    case 30:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -10000;
                            thisspace.Description = "Attend high-tech seminar.";
                            thisspace.CareerSpace = EnumCareerType.ComputerConsultant;
                            break;
                        }

                    case 31:
                        {
                            thisspace.ActionInfo = EnumActionType.AttendNightSchool;
                            break;
                        }

                    case 32:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 33:
                        {
                            thisspace.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 34:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 50000;
                            thisspace.Description = "Win lottery!";
                            break;
                        }

                    case 35:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Visit in-laws.";
                            break;
                        }

                    case 36:
                        {
                            thisspace.ActionInfo = EnumActionType.MayBuyHouse;
                            break;
                        }

                    case 37:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 38:
                        {
                            thisspace.ActionInfo = EnumActionType.FindNewJob;
                            thisspace.Description = "Lose your job." + Constants.vbCrLf + "Start new career.";
                            break;
                        }

                    case 39:
                        {
                            thisspace.ActionInfo = EnumActionType.GotBabyBoy;
                            break;
                        }

                    case 40:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Furnish baby room.";
                            thisspace.CareerSpace = EnumCareerType.SalesPerson;
                            break;
                        }

                    case 41:
                        {
                            thisspace.ActionInfo = EnumActionType.GotBabyGirl;
                            break;
                        }

                    case 42:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 10000;
                            thisspace.Description = "Win talent show.";
                            break;
                        }

                    case 43:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 44:
                        {
                            thisspace.ActionInfo = EnumActionType.HadTwins;
                            thisspace.Description = "Twins!";
                            break;
                        }

                    case 45:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -20000;
                            thisspace.Description = "50-yard line seats at the big game.";
                            thisspace.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 46:
                        {
                            thisspace.ActionInfo = EnumActionType.GotBabyGirl;
                            break;
                        }

                    case 47:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Attend Hollywood movie premier.";
                            thisspace.CareerSpace = EnumCareerType.Entertainer;
                            break;
                        }

                    case 48:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -40000;
                            thisspace.Description = "House flooded!";
                            thisspace.WhatInsurance = EnumInsuranceType.NeedHouse;
                            break;
                        }

                    case 49:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Family physicals.";
                            thisspace.CareerSpace = EnumCareerType.Doctor;
                            break;
                        }

                    case 50:
                        {
                            thisspace.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 51:
                        {
                            thisspace.ActionInfo = EnumActionType.GotBabyBoy;
                            break;
                        }

                    case 52:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 53:
                        {
                            thisspace.ActionInfo = EnumActionType.GotBabyGirl;
                            break;
                        }

                    case 54:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -1500;
                            thisspace.Description = "Tree falls on house.";
                            thisspace.WhatInsurance = EnumInsuranceType.NeedHouse;
                            break;
                        }

                    case 55:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Return lost wallet.";
                            break;
                        }

                    case 56:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Buy high-definition TV.";
                            thisspace.CareerSpace = EnumCareerType.SalesPerson;
                            break;
                        }

                    case 57:
                        {
                            thisspace.ActionInfo = EnumActionType.StockBoomed;
                            break;
                        }

                    case 58:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Family picnic.";
                            break;
                        }

                    case 59:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Visit Mount Rushmore.";
                            break;
                        }

                    case 60:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 61:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -15000;
                            thisspace.Description = "Car stolen!";
                            thisspace.WhatInsurance = EnumInsuranceType.NeedCar;
                            break;
                        }

                    case 62:
                        {
                            thisspace.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 63:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Run for mayor.";
                            break;
                        }

                    case 64:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Vote!";
                            break;
                        }

                    case 65:
                        {
                            thisspace.ActionInfo = EnumActionType.GotBabyBoy;
                            break;
                        }

                    case 66:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 67:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -25000;
                            thisspace.Description = "Buy luxury cruise online.";
                            thisspace.CareerSpace = EnumCareerType.ComputerConsultant;
                            break;
                        }

                    case 68:
                        {
                            thisspace.ActionInfo = EnumActionType.AttendNightSchool;
                            break;
                        }

                    case 69:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Learn CPR.";
                            break;
                        }

                    case 70:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -20000;
                            thisspace.Description = "Art auction.";
                            thisspace.CareerSpace = EnumCareerType.Artist;
                            break;
                        }

                    case 71:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 72:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Volunteer at charity sports event.";
                            break;
                        }

                    case 73:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 10000;
                            thisspace.Description = "Win photography contest.";
                            break;
                        }

                    case 74:
                        {
                            thisspace.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 75:
                        {
                            thisspace.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 76:
                        {
                            thisspace.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 77:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -25000;
                            thisspace.Description = "Tennis camp.";
                            thisspace.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 78:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -25000;
                            thisspace.Description = "Donate computer network.";
                            thisspace.CareerSpace = EnumCareerType.ComputerConsultant;
                            break;
                        }

                    case 79:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 80:
                        {
                            thisspace.ActionInfo = EnumActionType.StockCrashed;
                            break;
                        }

                    case 81:
                        {
                            thisspace.ActionInfo = EnumActionType.MaySellHouse;
                            break;
                        }

                    case 82:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.PerKid = true;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Day care.";
                            thisspace.CareerSpace = EnumCareerType.Teacher;
                            break;
                        }

                    case 83:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 80000;
                            thisspace.Description = "Write best-seller.";
                            break;
                        }

                    case 84:
                        {
                            thisspace.ActionInfo = EnumActionType.HadTwins;
                            thisspace.Description = "Adopt twins!";
                            break;
                        }

                    case 85:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -15000;
                            thisspace.Description = "Invest in broadway play.";
                            thisspace.CareerSpace = EnumCareerType.Entertainer;
                            break;
                        }

                    case 86:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 87:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Join health club.";
                            break;
                        }

                    case 88:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -35000;
                            thisspace.Description = "Family portrait.";
                            thisspace.CareerSpace = EnumCareerType.Artist;
                            break;
                        }

                    case 89:
                        {
                            thisspace.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 90:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -25000;
                            thisspace.Description = "Buy sport utility vehicle.";
                            thisspace.CareerSpace = EnumCareerType.SalesPerson;
                            break;
                        }

                    case 91:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 75000;
                            thisspace.Description = "Tax refund.";
                            break;
                        }

                    case 92:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -15000;
                            thisspace.Description = "Host Police Charity Ball!";
                            thisspace.CareerSpace = EnumCareerType.PoliceOfficer;
                            break;
                        }

                    case 93:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 94:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 80000;
                            thisspace.Description = "Find buried treasure!";
                            break;
                        }

                    case 95:
                        {
                            thisspace.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 96:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 97:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.CareerSpace = EnumCareerType.Artist;
                            thisspace.AmountReceived = -25000;
                            thisspace.Description = "Donate to Art Institute.";
                            break;
                        }

                    case 98:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Recycle.";
                            break;
                        }

                    case 99:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 95000;
                            thisspace.Description = "TV Game show winner!";
                            break;
                        }

                    case 100:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -5000;
                            thisspace.Description = "Summer School.";
                            thisspace.PerKid = true;
                            thisspace.CareerSpace = EnumCareerType.Teacher;
                            break;
                        }

                    case 101:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Have a Family Game Night.";
                            break;
                        }

                    case 102:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Learn sign language.";
                            break;
                        }

                    case 103:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -90000;
                            thisspace.Description = "Buy lakeside cabin.";
                            break;
                        }

                    case 104:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 105:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -50000;
                            thisspace.Description = "Burglar!";
                            thisspace.WhatInsurance = EnumInsuranceType.NeedHouse;
                            break;
                        }

                    case 106:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 10000;
                            thisspace.Description = "Win Nobel Peace Prize.";
                            break;
                        }

                    case 107:
                        {
                            thisspace.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 108:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -30000;
                            thisspace.Description = "Buy home gym.";
                            thisspace.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 109:
                        {
                            thisspace.ActionInfo = EnumActionType.StockCrashed;
                            break;
                        }

                    case 110:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -125000;
                            thisspace.Description = "Tornado hits house!";
                            thisspace.WhatInsurance = EnumInsuranceType.NeedHouse;
                            break;
                        }

                    case 111:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 112:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -25000;
                            thisspace.Description = "Life-saving operation.";
                            thisspace.CareerSpace = EnumCareerType.Doctor;
                            break;
                        }

                    case 113:
                        {
                            thisspace.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 114:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -30000;
                            thisspace.Description = "Buy sailboat.";
                            thisspace.CareerSpace = EnumCareerType.SalesPerson;
                            break;
                        }

                    case 115:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -35000;
                            thisspace.Description = "Sponsor golf tournament.";
                            thisspace.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 116:
                        {
                            thisspace.ActionInfo = EnumActionType.FindNewJob;
                            thisspace.Description = "Mid-life crisis." + Constants.vbCrLf + "Start new career.";
                            break;
                        }

                    case 117:
                        {
                            thisspace.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 118:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 119:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -100000;
                            thisspace.CareerSpace = EnumCareerType.Entertainer;
                            thisspace.Description = "Host on-line concert.";
                            break;
                        }

                    case 120:
                        {
                            thisspace.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 121:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Help the homeless.";
                            break;
                        }

                    case 122:
                        {
                            thisspace.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 123:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -100000;
                            thisspace.Description = "Have cosmetic surgery.";
                            thisspace.CareerSpace = EnumCareerType.Doctor;
                            break;
                        }

                    case 124:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -50000;
                            thisspace.Description = "College.";
                            thisspace.CareerSpace = EnumCareerType.Teacher;
                            break;
                        }

                    case 125:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 126:
                        {
                            thisspace.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 127:
                        {
                            thisspace.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 128:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Visit memorial.";
                            break;
                        }

                    case 129:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -125000;
                            thisspace.CareerSpace = EnumCareerType.Artist;
                            thisspace.Description = "Sponsor art exhihit.";
                            break;
                        }

                    case 130:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Grand Canyon vacation.";
                            break;
                        }

                    case 131:
                        {
                            thisspace.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 132:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 133:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Go fishing.";
                            break;
                        }

                    case 134:
                        {
                            thisspace.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 135:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -65000;
                            thisspace.Description = "Hire jockey for your racehorse.";
                            thisspace.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 136:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Go hiking.";
                            break;
                        }

                    case 137:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 138:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Plant a tree.";
                            break;
                        }

                    case 139:
                        {
                            thisspace.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 140:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "Support wildlife fund.";
                            break;
                        }

                    case 141:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -45000;
                            thisspace.CareerSpace = EnumCareerType.ComputerConsultant;
                            thisspace.Description = "Have a web site designed.";
                            break;
                        }

                    case 142:
                        {
                            thisspace.ActionInfo = EnumActionType.ObtainLifeTile;
                            thisspace.Description = "You're a grandparent!";
                            break;
                        }

                    case 143:
                        {
                            thisspace.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 144:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -35000;
                            thisspace.Description = "Throw party for entertainment award winners.";
                            thisspace.CareerSpace = EnumCareerType.Entertainer;
                            break;
                        }

                    case 145:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = -45000;
                            thisspace.Description = "Invest in E-commerce company.";
                            thisspace.CareerSpace = EnumCareerType.ComputerConsultant;
                            break;
                        }

                    case 146:
                        {
                            thisspace.ActionInfo = EnumActionType.CollectPayMoney;
                            thisspace.AmountReceived = 20000;
                            thisspace.Description = "Pension";
                            break;
                        }

                    case 147:
                        {
                            thisspace.ActionInfo = EnumActionType.WillRetire;
                            break;
                        }
                }
                if (thisspace.ActionInfo == EnumActionType.ObtainLifeTile)
                    thisspace.GetLifeTile = true;
                if (thisspace.ActionInfo == EnumActionType.GotBabyBoy)
                    thisspace.GetLifeTile = true;
                if (thisspace.ActionInfo == EnumActionType.GotBabyGirl)
                    thisspace.GetLifeTile = true;
                if (thisspace.ActionInfo == EnumActionType.HadTwins)
                    thisspace.GetLifeTile = true;
                if (thisspace.ActionInfo == EnumActionType.GetMarried)
                    thisspace.GetLifeTile = true;
                if (thisspace.ActionInfo == EnumActionType.TradeSalary)
                    thisspace.IsOptional = true;
                if (thisspace.ActionInfo == EnumActionType.MayBuyHouse)
                    thisspace.IsOptional = true;
                if (thisspace.ActionInfo == EnumActionType.MaySellHouse)
                    thisspace.IsOptional = true;
                if (thisspace.ActionInfo == EnumActionType.AttendNightSchool)
                {
                    thisspace.IsOptional = true;
                    thisspace.AmountReceived = -20000;
                    thisspace.CareerSpace = EnumCareerType.Teacher;
                }
                if (thisspace.ActionInfo == EnumActionType.PayTaxes)
                    thisspace.CareerSpace = EnumCareerType.Accountant;
                _thisGlobal.SpaceList.Add(thisspace);
            }
        }
        public string SpaceDescription(int index)
        {
            var thisSpace = _thisGlobal!.SpaceList![index - 1];
            decimal newAmount;
            string output;
            switch (thisSpace.ActionInfo)
            {
                case EnumActionType.CollectPayMoney:
                    if (thisSpace.AmountReceived < 0)
                    {
                        newAmount = Math.Abs(thisSpace.AmountReceived);
                        output = $"{thisSpace.Description}{Constants.vbCrLf}Pay {newAmount.ToCurrency(0)}";
                        if (thisSpace.WhatInsurance != EnumInsuranceType.NoInsurance)
                            output += Constants.vbCrLf + " if not insured";
                        return output;
                    }
                    return thisSpace.Description + Constants.vbCrLf + thisSpace.AmountReceived.ToCurrency(0);
                case EnumActionType.AttendNightSchool:
                    return $"Night School.{Constants.vbCrLf} Pay $20,000";
                case EnumActionType.FindNewJob:
                    return thisSpace.Description;
                case EnumActionType.GetMarried:
                    return "Get Married";
                case EnumActionType.GetPaid:
                    return "Pay!" + Constants.vbCrLf + "Day";
                case EnumActionType.GotBabyBoy:
                    return "Baby boy!" + Constants.vbCrLf + "Life";
                case EnumActionType.GotBabyGirl:
                    return "Baby girl!" + Constants.vbCrLf + "Life";
                case EnumActionType.HadTwins:
                    return thisSpace.Description + Constants.vbCrLf + "Life";
                case EnumActionType.MayBuyHouse:
                    return "You may BUY A HOUSE" + Constants.vbCrLf + "Draw Deed";
                case EnumActionType.MaySellHouse:
                    return "You may sell your house and buy a new one.";
                case EnumActionType.ObtainLifeTile:
                    return thisSpace.Description + Constants.vbCrLf + "Life";
                case EnumActionType.PayTaxes:
                    return "Taxes due.";
                case EnumActionType.SpinAgainIfBehind:
                    return "Spin again if you are not in the lead.";
                case EnumActionType.StartCareer:
                    return "CAREER CHOICE";
                case EnumActionType.StockBoomed:
                    return "Stock market soars!" + Constants.vbCrLf + "Collect 1 stock.";
                case EnumActionType.StockCrashed:
                    return "Stock market crash." + Constants.vbCrLf + "Return 1 stock.";
                case EnumActionType.TradeSalary:
                    return "Trade salary card with any player.";
                case EnumActionType.WillMissTurn:
                    return thisSpace.Description + Constants.vbCrLf + "Miss next turn.";
                case EnumActionType.WillRetire:
                    return "RETIRE" + Constants.vbCrLf + "Go to Countryside Acres" + Constants.vbCrLf + "or Millionaire Estates.";
                default:
                    throw new BasicBlankException("No description for " + thisSpace.ActionInfo.ToString());
            }
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            EraseColors();
            SaveRoot!.LoadMod(ThisMod!);
            SaveRoot.ImmediatelyStartTurn = true; //try this too.
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            WhoTurn = WhoStarts;
            ThisMod!.GameBoard1!.ResetBoard();
            GameStatus = EnumWhatStatus.NeedChooseGender;
            if (ThisData!.MultiPlayer && ThisData.Client)
            {
                ThisCheck!.IsEnabled = true;
                return; //wait for state data again
            }
            var tempList = Rs!.GenerateRandomList(25);
            SaveRoot!.TileList.Clear();
            tempList.ForEach(index => SaveRoot.TileList.Add(GetTileInfo(index)));
            SaveRoot.ImmediatelyStartTurn = true;
            if (ThisData.MultiPlayer)
                await ThisNet!.SendRestoreGameAsync(SaveRoot);
            await StartNewTurnAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "spin":
                    SpinnerPositionData thisSpin = await js.DeserializeObjectAsync<SpinnerPositionData>(content);
                    await StartSpinningAsync(thisSpin);
                    return;
                case "gender":
                    EnumGender thisGender = await js.DeserializeObjectAsync<EnumGender>(content);
                    await SelectGenderAsync(thisGender);
                    return;
                case "firstoption":
                    EnumStart thisS = await js.DeserializeObjectAsync<EnumStart>(content);
                    await FirstOptionChosenAsync(thisS);
                    return;
                case "chosecareer":
                    await ChoseCareerAsync(int.Parse(content));
                    return;
                case "purchasecarinsurance":
                    await PurchaseCarInsuranceAsync();
                    return;
                case "purchasedstock":
                    await PurchasedStockAsync();
                    return;
                case "tradedlifeforsalary":
                    await TradedLifeForSalaryAsync();
                    return;
                case "tradedsalary":
                    ThisMod!.PlayerChosen = await js.DeserializeObjectAsync<string>(content); //i was forced to deserialize even for strings.
                    await TradedSalaryAsync();
                    return;
                case "stole":
                    ThisMod!.PlayerChosen = await js.DeserializeObjectAsync<string>(content);
                    await TilesStolenAsync();
                    return;
                case "purchasedhouseinsurance":
                    await PurchaseHouseInsuranceAsync();
                    return;
                case "attendednightschool":
                    await AttendNightSchoolAsync();
                    return;
                case "choseretirement":
                    EnumFinal thisF = await js.DeserializeObjectAsync<EnumFinal>(content);
                    await ChoseRetirementAsync(thisF);
                    return;
                case "chosestock":
                    await ChoseStockAsync(int.Parse(content));
                    return;
                case "chosesalary":
                    await ChoseSalaryAsync(int.Parse(content));
                    return;
                case "stockreturned":
                    await StockReturnedAsync(int.Parse(content));
                    return;
                case "chosehouse":
                    await ChoseHouseAsync(int.Parse(content));
                    return;
                case "willsellhouse":
                    await WillSellHouseAsync();
                    return;
                case "twins":
                    CustomBasicList<EnumGender> gList = await js.DeserializeObjectAsync<CustomBasicList<EnumGender>>(content);
                    await GetTwinsAsync(gList);
                    return;
                case "houselist":
                    CustomBasicList<int> tempList = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    SaveRoot!.HouseList.Clear();
                    tempList.ForEach(thisIndex => SaveRoot.HouseList.Add(GetHouseCard(thisIndex)));
                    await ContinueTurnAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            ThisMod!.GenderSelected = EnumGender.None;
            _thisGlobal!.CurrentSelected = 0;
            if (SingleInfo!.Gender == EnumGender.None)
            {
                SaveRoot!.GameStatus = EnumWhatStatus.NeedChooseGender;
            }
            else if (SingleInfo.OptionChosen == EnumStart.None)
            {
                SaveRoot!.GameStatus = EnumWhatStatus.NeedChooseFirstOption;
            }
            else if (TeacherChooseSecondCareer)
            {
                _thisGlobal.MaxChosen = 1;
                SaveRoot!.GameStatus = EnumWhatStatus.NeedNewCareer;
            }
            else
            {
                SaveRoot!.GameStatus = EnumWhatStatus.NeedToSpin;
            }
            if (_thisGlobal.GameStatus != EnumWhatStatus.NeedChooseGender)
            {
                ThisMod.GameBoard1!.NewTurn();
                _thisGlobal.SpinnerCanvas!.Repaint();
            }
            else
            {
                ThisE.Publish(new NewTurnEventModel());
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task MakeMoveAsync(int space)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendMoveAsync(space);
            await AutomateMoveAsync(space);
        }
        private async Task FinishTilesAsync()
        {
            SaveRoot!.Instructions = "None";
            ThisMod!.GameDetails = "None";
            if (SaveRoot.TileList.Count != 25)
                throw new BasicBlankException("Must have 25 tiles");
            PlayerList!.ForEach(thisPlayer =>
            {
                for (int x = 1; x <= thisPlayer.TilesCollected; x++)
                {
                    var thisTile = SaveRoot.TileList.First();
                    thisPlayer.MoneyEarned += thisTile.AmountReceived;
                    SaveRoot.TileList.RemoveFirstItem();
                }
                FillInfo(thisPlayer);
            });
            SingleInfo = PlayerList.OrderByDescending(items => items.MoneyEarned).First();
            await ShowWinAsync();
        }
        private int WonSoFar(out int secondPlayer)
        {
            secondPlayer = 0;
            var tempList = PlayerList.Where(items => items.LastMove == EnumFinal.MillionaireEstates).OrderByDescending(items => items.NetIncome()).Take(3).ToCustomBasicList();
            if (tempList.Count == 0)
                return 0;
            if (tempList.Count == 1)
                return tempList.Single().Id;
            if (tempList.First().NetIncome() > tempList[1].NetIncome())
                return tempList.First().Id;
            if (tempList.Count > 2)
            {
                if (tempList.First().NetIncome() == tempList.Last().NetIncome())
                    return 0; //if 3 way tie, nobody gets.
                tempList.RemoveLastItem();
            }
            if (tempList.First().NetIncome() != tempList.Last().NetIncome())
                throw new BasicBlankException("Does not reconcile");
            secondPlayer = tempList.Last().Id;
            return tempList.First().Id;
        }
        private async Task EndGameProcessAsync()
        {
            int winPlayer = WonSoFar(out int secondPlayer);
            if (winPlayer > 0)
            {
                var thisPlayer = PlayerList![winPlayer];
                if (secondPlayer == 0)
                {
                    thisPlayer.TilesCollected += 4;
                }
                else
                {
                    thisPlayer.TilesCollected += 2;
                    thisPlayer = PlayerList[secondPlayer];
                    thisPlayer.TilesCollected += 2;
                }
            }
            await FinishTilesAsync();
        }
        public override async Task EndTurnAsync()
        {
            SaveRoot!.WasNight = false;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true); //missing turn process is done automatically via interfaces.
            if (WhoTurn > 0)
                await StartNewTurnAsync();
            else
                await EndGameProcessAsync();
        }
        private bool AllHasCareer
        {
            get
            {
                foreach (var thisPlayer in PlayerList!)
                {
                    var tempList = thisPlayer.GetCareerList();
                    if (tempList.Count == 0)
                        return false;
                }
                return true;
            }
        }
        private bool TeacherChooseSecondCareer
        {
            get
            {
                var tempList = SingleInfo!.GetCareerList();
                if (tempList.Count == 2)
                    return false; //already has it.
                if (tempList.Count == 0)
                    return false;
                if (tempList.Single().Career != EnumCareerType.Teacher)
                    return false;
                return AllHasCareer;
            }
        }
        public async Task AfterTileListAsync()
        {
            await StartNewTurnAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (DidChooseColors)
            {
                if (GameStatus == EnumWhatStatus.NeedChooseHouse)
                {
                    if (SaveRoot!.HouseList.Count == 0)
                    {
                        if (ThisData!.MultiPlayer == true && ThisData.Client)
                        {
                            ThisCheck!.IsEnabled = true; //wait for host to send house data.
                            return;
                        }
                        var firstList = Rs!.GenerateRandomList(18, 9, 10);
                        SaveRoot.HouseList = firstList.GetHouseList(PlayerList!);
                        if (ThisData.MultiPlayer)
                            await ThisNet!.SendAllAsync("houselist", SaveRoot.HouseList.GetDeckListFromObjectList());
                    }
                }
                switch (GameStatus)
                {

                    case EnumWhatStatus.NeedToEndTurn:
                        SaveRoot!.Instructions = "View results and end turn";
                        break;
                    case EnumWhatStatus.NeedChooseStock:
                        ThisMod!.LoadStockList();
                        break;
                    case EnumWhatStatus.NeedChooseHouse:
                        ThisMod!.GameBoard1!.NumberRolled = 0;
                        ThisMod.LoadHouseList();
                        break;
                    case EnumWhatStatus.NeedSellBuyHouse:
                        SaveRoot!.Instructions = "Choose to sell house or spin to not sell house";
                        ThisMod!.ShowYourHouse();
                        ThisMod.GameBoard1!.NumberRolled = 0;
                        break;
                    case EnumWhatStatus.NeedTradeSalary:
                        SaveRoot!.Instructions = $"Choose to trade your salary with someone on list or end turn to keep your current salary{Constants.vbCrLf} Your salary is{SingleInfo!.Salary.ToCurrency(0)}";
                        ThisMod!.LoadOtherPlayerSalaries();
                        break;
                    case EnumWhatStatus.NeedFindSellPrice:
                    case EnumWhatStatus.NeedSellHouse:
                        ThisMod!.ShowYourHouse();
                        SaveRoot!.Instructions = "Spin to find out the selling price for the house";
                        break;
                    case EnumWhatStatus.NeedChooseSalary:
                        ThisMod!.LoadSalaryList();
                        break;
                    case EnumWhatStatus.NeedReturnStock:
                        SaveRoot!.Instructions = "Choose a stock to return";
                        ThisMod!.LoadCurrentPlayerStocks();
                        break;
                    case EnumWhatStatus.NeedStealTile:
                        ThisMod!.LoadOtherPlayerTiles();
                        break;
                    case EnumWhatStatus.NeedChooseRetirement:
                        _thisGlobal!.CurrentView = EnumViewCategory.EndGame;
                        SaveRoot!.Instructions = "Choose either Countryside Acres or Millionaire Estates for retirement";
                        break;
                    case EnumWhatStatus.NeedChooseGender:
                        ThisMod!.RepaintGenders();
                        SaveRoot!.Instructions = "Choose Gender";
                        break;
                    case EnumWhatStatus.NeedChooseFirstOption:
                        SaveRoot!.Instructions = "Choose college or career";
                        break;
                    case EnumWhatStatus.NeedToSpin:
                        SaveRoot!.Instructions = "Spin to take your turn";
                        break;
                    case EnumWhatStatus.NeedToChooseSpace:
                        SaveRoot!.Instructions = "Decide Which Space";
                        break;
                    case EnumWhatStatus.NeedChooseFirstCareer:
                    case EnumWhatStatus.NeedNewCareer:
                        ThisMod!.LoadCareerList();
                        if (ThisMod.HandList!.HandList.Count == 0)
                        {
                            GameStatus = EnumWhatStatus.NeedChooseSalary;
                            await ContinueTurnAsync();
                            return;
                        }
                        break;
                    case EnumWhatStatus.NeedNight:
                        SaveRoot!.Instructions = "Decide whether to goto night school or not";
                        break;
                    default:
                        throw new BasicBlankException("Don't know what to do about continue turn. Rethink");
                }
            }
            await base.ContinueTurnAsync();
        }
        public async Task ChoseRetirementAsync(EnumFinal retirementType)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("choseretirement", retirementType);
            SingleInfo!.LastMove = retirementType;
            ThisE.RepaintBoard();
            SingleInfo.InGame = false;
            await EndTurnAsync();
        }
        private void GetBaby(LifeBoardGamePlayerItem thisPlayer, EnumGender gender)
        {
            thisPlayer.ChildrenList.Add(gender);
            ThisE.RepaintBoard(); //i think
        }
        public async Task GetTwinsAsync(CustomBasicList<EnumGender> twinList)
        {
            SingleInfo = PlayerList!.GetWhoPlayer(); //just in case.
            twinList.ForEach(thisTwin => GetBaby(SingleInfo, thisTwin));
            SaveRoot!.WasMarried = false;
            await ContinueTurnAsync();
        }
        private void GetMarriedProcess(LifeBoardGamePlayerItem thisPlayer)
        {
            thisPlayer.Married = true;
            if (GameStatus != EnumWhatStatus.NeedStealTile)
                GameStatus = EnumWhatStatus.NeedToSpin;
            ThisE.RepaintBoard();
            SaveRoot!.WasMarried = true;
        }
        private int WhoHadStock(int whatNum)
        {
            if (whatNum == 0)
                return 0;
            var thisPlayer = PlayerList.SingleOrDefault(items => items.FirstStock == whatNum || items.SecondStock == whatNum);
            if (thisPlayer == null)
                return 0;
            return thisPlayer.Id;
        }
        public async Task StockReturnedAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("stockreturned", deck);
            var thisStock = GetStockCard(deck);
            await ThisMod!.ShowCardAsync(thisStock);
            SingleInfo!.Hand.RemoveObjectByDeck(thisStock.Deck);
            if (thisStock.Value == SingleInfo.FirstStock)
            {
                SingleInfo.FirstStock = SingleInfo.SecondStock;
                SingleInfo.FirstStock = 0;
            }
            else if (thisStock.Value == SingleInfo.SecondStock)
            {
                SingleInfo.SecondStock = 0;
            }
            else
            {
                throw new BasicBlankException("Cannot update the stock");
            }

            GameStatus = EnumWhatStatus.NeedToEndTurn;
            await ContinueTurnAsync();
        }
        public async Task ChoseStockAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("chosestock", deck);
            var thisStock = GetStockCard(deck);
            await ThisMod!.ShowCardAsync(thisStock);
            SingleInfo!.Hand.Add(thisStock);
            if (SingleInfo.FirstStock == 0)
            {
                SingleInfo.FirstStock = thisStock.Value;
            }
            else if (SingleInfo.SecondStock == 0)
            {
                SingleInfo.SecondStock = thisStock.Value;
            }
            else
            {
                throw new BasicBlankException("Can only have 2 stocks at the most");
            }
            if (SaveRoot!.EndAfterStock)
                GameStatus = EnumWhatStatus.NeedToEndTurn;
            else
                GameStatus = EnumWhatStatus.NeedToSpin;
            await ContinueTurnAsync();
        }
        private async Task AutoReturnAsync()
        {
            StockInfo thisStock;
            if (SingleInfo!.FirstStock > 0)
            {
                thisStock = (StockInfo)SingleInfo.GetStockCard(SingleInfo.FirstStock);
                SingleInfo.FirstStock = 0;
            }
            else if (SingleInfo.SecondStock > 0)
            {
                thisStock = (StockInfo)SingleInfo.GetStockCard(SingleInfo.SecondStock);
                SingleInfo.SecondStock = 0;
            }
            else
            {
                throw new BasicBlankException("There is nothing to return");
            }
            SingleInfo.Hand.RemoveSpecificItem(thisStock);
            await ThisMod!.ShowCardAsync(thisStock);
            GameStatus = EnumWhatStatus.NeedToEndTurn;
            await ContinueTurnAsync();
        }
        public bool TilesLeft
        {
            get
            {
                int howMany = PlayerList.Sum(items => items.TilesCollected);
                if (howMany > 21)
                    throw new BasicBlankException("Only 21 tiles at the most");
                return howMany < 21;
            }
        }
        public bool CanStealTile => PlayerList.Any(items => items.TilesCollected > 0 && items.LastMove != EnumFinal.CountrySideAcres);

        public async Task WillSellHouseAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("willsellhouse");
            GameStatus = EnumWhatStatus.NeedFindSellPrice;
            await ContinueTurnAsync();
        }
        public async Task FirstOptionChosenAsync(EnumStart whatOption)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("firstoption", whatOption);
            SingleInfo!.OptionChosen = whatOption;
            ThisE.RepaintBoard(); //i think
            if (whatOption == EnumStart.College)
            {
                SingleInfo.Loans = 100000;
                GameStatus = EnumWhatStatus.NeedToSpin;
            }
            else
            {
                GameStatus = EnumWhatStatus.NeedChooseFirstCareer;
            }

            await ContinueTurnAsync();
        }
        private void FillInfo(LifeBoardGamePlayerItem thisPlayer)
        {
            var career1 = CareerChosen(thisPlayer, out string career2);
            thisPlayer.Career1 = career1;
            thisPlayer.Career2 = career2;
            thisPlayer.HouseName = thisPlayer.GetHouseName();
        }
        private void ObtainLife(LifeBoardGamePlayerItem thisPlayer)
        {
            thisPlayer.TilesCollected++;
        }
        public string CareerChosen(LifeBoardGamePlayerItem thisPlayer, out string secondCareer)
        {
            secondCareer = ""; //until proven.
            var tempList = thisPlayer.GetCareerList();
            string output;
            var thisCareer = tempList.SingleOrDefault(items => items.Career == EnumCareerType.Teacher);
            if (thisCareer != null && thisCareer.Deck > 0)
            {
                output = "Teacher";
                var newCareer = tempList.SingleOrDefault(items => items.Career != EnumCareerType.Teacher);
                if (newCareer != null && newCareer.Deck > 0)
                    secondCareer = newCareer.Career.ToString();
                return output;
            }
            if (tempList.Count == 0)
                return "";
            return tempList.Single().Career.ToString();
        }
        public async Task TradedSalaryAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (GameStatus == EnumWhatStatus.NeedTradeSalary && SingleInfo.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("tradedsalary", ThisMod!.PlayerChosen);
            ThisMod!.PopUpList!.ShowOnlyOneSelectedItem(ThisMod.PlayerChosen);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            var thisPlayer = PlayerList[ThisMod.PlayerChosen];
            SalaryInfo firstSalary;
            SalaryInfo secondSalary;
            firstSalary = thisPlayer.GetSalaryCard();
            secondSalary = SingleInfo.GetSalaryCard();
            SingleInfo.Salary = firstSalary.PayCheck;
            thisPlayer.Salary = secondSalary.PayCheck;
            thisPlayer.Hand.ReplaceItem(firstSalary, secondSalary);
            SingleInfo.Hand.ReplaceItem(secondSalary, firstSalary);
            FillInfo(SingleInfo);
            FillInfo(thisPlayer);
            if (GameStatus == EnumWhatStatus.NeedTradeSalary)
                GameStatus = EnumWhatStatus.NeedToEndTurn;
            else if (GameStatus != EnumWhatStatus.NeedToSpin)
                throw new BasicBlankException("Rethinking is required");
            await ContinueTurnAsync();
        }
        public async Task TilesStolenAsync()
        {
            if (GameStatus == EnumWhatStatus.NeedTradeSalary && SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("stole", ThisMod!.PlayerChosen);
            ThisMod!.PopUpList!.ShowOnlyOneSelectedItem(ThisMod.PlayerChosen);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            var thisPlayer = PlayerList![ThisMod.PlayerChosen];
            thisPlayer.TilesCollected--;
            ObtainLife(thisPlayer);
            if (SaveRoot!.WasMarried)
                GameStatus = EnumWhatStatus.NeedToSpin;
            else
                GameStatus = EnumWhatStatus.NeedToEndTurn;
            await ContinueTurnAsync();
        }
        private void RemoveCareer(int deck)
        {
            var careerList = SingleInfo!.GetCareerList();
            careerList.ForEach(thisCard =>
            {
                if (thisCard.Deck != deck)
                    SingleInfo!.Hand.RemoveObjectByDeck(thisCard.Deck);
            });
        }
        private bool PrivateCanGetSalary(string career1, string career2)
        {
            if (career1 != "Teacher")
                return true;
            return career2 == "";
        }
        public async Task ChoseCareerAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("chosecareer", deck);
            if (SaveRoot!.WasNight)
                RemoveCareer(deck);
            var thisCareer = GetCareerCard(deck);
            SingleInfo!.Hand.Add(thisCareer);
            await ThisMod!.ShowCardAsync(thisCareer);
            string career1 = CareerChosen(SingleInfo, out string career2);
            if (career1 != "Teacher" && SaveRoot.WasNight == false)
                RemoveCareer(deck);
            FillInfo(SingleInfo);
            if (PrivateCanGetSalary(career1, career2))
            {
                SaveRoot.EndAfterSalary = GameStatus == EnumWhatStatus.NeedNewCareer || SaveRoot.WasNight;
                var thisSalary = SingleInfo.GetSalaryCard();
                SingleInfo.Hand.RemoveSpecificItem(thisSalary);
                GameStatus = EnumWhatStatus.NeedChooseSalary;
                await ContinueTurnAsync();
                return;
            }
            if (SaveRoot.WasNight)
            {
                SaveRoot.EndAfterSalary = true;
            }
            else if (TeacherChooseSecondCareer)
            {
                GameStatus = EnumWhatStatus.NeedNewCareer;
                SaveRoot.MaxChosen = 1;
            }
            else
            {
                GameStatus = EnumWhatStatus.NeedToSpin;
            }

            ThisE.RepaintBoard();
            await ContinueTurnAsync();
        }
        public async Task SelectGenderAsync(EnumGender gender)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("gender", gender);
            ThisMod!.GenderSelected = gender;
            ThisE.RepaintMessage(EnumRepaintCategories.frombeginning); //i think
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelayMilli(500);
            ThisMod.GenderSelected = EnumGender.None;
            SingleInfo!.Gender = gender;
            await EndTurnAsync();
        }
        public async Task ChoseSalaryAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("chosesalary", deck);
            if (GameStatus == EnumWhatStatus.NeedTradeSalary)
            {
                throw new BasicBlankException("I think if the salary is being traded; must use TradedSalary method instead");
            }
            else if (GameStatus == EnumWhatStatus.NeedChooseSalary)
            {
                var thisSalary = GetSalaryCard(deck);
                await ThisMod!.ShowCardAsync(thisSalary);
                SingleInfo!.Hand.Add(thisSalary);
                SingleInfo.Salary = thisSalary.PayCheck;
                FillInfo(SingleInfo);
            }
            else
            {
                throw new BasicBlankException("Not sure what to do about this game status.  Rethink");
            }
            if (SaveRoot!.EndAfterSalary)
                GameStatus = EnumWhatStatus.NeedToEndTurn;
            else
                GameStatus = EnumWhatStatus.NeedToSpin;
            await ContinueTurnAsync();
        }
        private int WhoHadCareerName(EnumCareerType thisCareer)
        {
            foreach (var thisPlayer in PlayerList!)
            {
                var tempList = thisPlayer.GetCareerList();
                if (tempList.Any(items => items.Career == thisCareer))
                    return thisPlayer.Id;
            }
            return 0;
        }
        public int PlayerHas100000 => 19.PlayerHasCard(PlayerList!);
        public SalaryInfo Get100000Card => GetSalaryCard(19);
        public bool CanTradeForBig(bool isAthlete)
        {
            int player = PlayerHas100000;
            if (player == WhoTurn)
                return false; //because you already have it.
            int whoHadAthlete = WhoHadCareerName(EnumCareerType.Athlete);
            int whoHadEntertainer = WhoHadCareerName(EnumCareerType.Entertainer);
            if (whoHadAthlete != WhoTurn && isAthlete)
                return false; //because you are not an athlete
            if (whoHadEntertainer != WhoTurn && isAthlete == false)
                return false; //because you are not an entertainer.
            if (isAthlete)
            {
                return SingleInfo!.TilesCollected > 3;
            }
            if (SaveRoot!.SpinList.Count < 2)
                return false;
            var tempList = SaveRoot.SpinList.Skip(SaveRoot.SpinList.Count - 2).ToCustomBasicList();
            if (tempList.Count != 2)
                throw new BasicBlankException("Must have 2 items");
            if (tempList.First() < 8)
                return false;
            return tempList.First() == tempList.Last();
        }
        public void CollectMoney(int player, decimal moneyCollected)
        {
            var thisPlayer = PlayerList![player];
            thisPlayer.MoneyEarned += moneyCollected;
            FillInfo(thisPlayer);
        }
        public void BetweenNumbers()
        {
            int whoHad = WhoHadCareerName(EnumCareerType.ComputerConsultant);
            if (whoHad > 0 && whoHad != WhoTurn)
                CollectMoney(whoHad, 50000);
        }
        public void TakeOutExpense(decimal whatAmount)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            do
            {
                if (SingleInfo.MoneyEarned > whatAmount)
                    break;
                SingleInfo.MoneyEarned += 20000;
                SingleInfo.Loans += 20000;
            } while (true);
            SingleInfo.MoneyEarned -= whatAmount;
        }
        private void ProcessCommission()
        {
            int whoHad = WhoHadCareerName(EnumCareerType.SalesPerson);
            if (whoHad > 0)
                CollectMoney(whoHad, 5000);
        }
        public async Task PurchaseCarInsuranceAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("purchasecarinsurance");
            SingleInfo!.CarIsInsured = true;
            TakeOutExpense(5000);
            ThisMod!.GameDetails = "Paid $5,000 for car insurance.  Now you owe nothing for car damages or car accidents";
            await ContinueTurnAsync();
        }
        public async Task PurchaseHouseInsuranceAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("purchasedhouseinsurance");
            decimal amountToPay = SingleInfo!.InsuranceCost();
            TakeOutExpense(amountToPay);
            SingleInfo!.HouseIsInsured = true;
            ProcessCommission();
            ThisMod!.GameDetails = $"Paid {amountToPay.ToCurrency(0)}.  Now you owe nothing for damages for the house";
            await ContinueTurnAsync();
        }
        public async Task PurchasedStockAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("purchasedstock");
            SaveRoot!.EndAfterStock = false;
            TakeOutExpense(50000);
            ProcessCommission();
            ThisMod!.GameDetails = "Paid $50,000 for stock";
            GameStatus = EnumWhatStatus.NeedChooseStock;
            await ContinueTurnAsync();
        }
        public async Task ChoseHouseAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("chosehouse", deck);
            HouseInfo thisHouse = GetHouseCard(deck);
            await ThisMod!.ShowCardAsync(thisHouse);
            SingleInfo!.Hand.Add(thisHouse);
            SaveRoot!.HouseList.Clear();
            FillInfo(SingleInfo); //i think here too.
            TakeOutExpense(thisHouse.HousePrice);
            GameStatus = EnumWhatStatus.NeedToSpin;
            await ContinueTurnAsync();
        }
        private async Task SoldHouseAsync(int numberRolled)
        {
            var thisHouse = SingleInfo!.GetHouseCard();
            decimal moneyEarned = thisHouse.SellingPrices[numberRolled];
            SingleInfo!.HouseIsInsured = false;
            ThisMod!.GameDetails = $"House sold for {moneyEarned.ToCurrency(0)}";
            CollectMoney(WhoTurn, moneyEarned);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            SingleInfo.Hand.RemoveSpecificItem(thisHouse);
            FillInfo(SingleInfo);
            ThisMod.ChosenPile!.Visible = false;
            if (GameStatus == EnumWhatStatus.NeedFindSellPrice)
                GameStatus = EnumWhatStatus.NeedChooseHouse;
            else
                GameStatus = EnumWhatStatus.NeedChooseRetirement;
        }
        private void PaydayProcessing(LifeBoardGamePlayerItem thisPlayer)
        {
            if (ThisMod!.GameBoard1!.PayDaysPassed == 0)
                return;
            decimal amountPaid = thisPlayer.Salary;
            CollectMoney(thisPlayer.Id, amountPaid * ThisMod.GameBoard1.PayDaysPassed);
            GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        private async Task RetirementProcessAsync()
        {
            var careerList = SingleInfo!.GetCareerList();
            careerList.ForEach(thisCard =>
            {
                SingleInfo!.Hand.RemoveSpecificItem(thisCard);
            });
            var thisSalary = SingleInfo!.GetSalaryCard();
            SingleInfo!.Hand.RemoveSpecificItem(thisSalary);
            SingleInfo.HouseIsInsured = false;
            SingleInfo.CarIsInsured = false;
            SingleInfo.Salary = 0;
            RepayLoans();
            FillInfo(SingleInfo);
            if (SingleInfo.GetHouseName() == "")
                GameStatus = EnumWhatStatus.NeedChooseRetirement;
            else
                GameStatus = EnumWhatStatus.NeedSellHouse;
            await ContinueTurnAsync();
        }
        private void RepayLoans()
        {
            if (SingleInfo!.Loans == 0)
                return;
            var numberLoans = SingleInfo.Loans / 20000;
            SingleInfo.Loans = 0;
            TakeOutExpense(numberLoans * 25000);
        }

        internal bool WasBig;
        private async Task TradeForBigAsync()
        {
            int whichPlayer = PlayerHas100000;
            if (whichPlayer == 0)
            {
                var thisSalary = SingleInfo!.GetSalaryCard();
                SingleInfo!.Hand.RemoveSpecificItem(thisSalary);
                SingleInfo.Hand.Add(Get100000Card);
                SingleInfo.Salary = 100000;
                FillInfo(SingleInfo);
                if (GameStatus == EnumWhatStatus.LastSpin)
                {
                    await PossibleAutomateMoveAsync();
                    return;
                }
                await ContinueTurnAsync(); //iffy.
                return;
            }
            WasBig = true;
            ThisMod!.LoadOtherPlayerSalaries();
            var thisPlayer = PlayerList![whichPlayer];
            ThisMod.PlayerChosen = thisPlayer.NickName;
            WasBig = false;
            await TradedSalaryAsync();
        }
        public async Task TradedLifeForSalaryAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("tradedlifeforsalary");
            SingleInfo!.TilesCollected -= 4;
            await TradeForBigAsync();
        }
        public async Task AttendNightSchoolAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("attendednightschool");
            SaveRoot!.WasNight = true;
            ThisMod!.GameDetails = "Paid $20,000 to attend night school to possibly get a better career";
            TakeOutExpense(20000);
            GameStatus = EnumWhatStatus.NeedNewCareer;
            await ContinueTurnAsync();
        }
        private string ComputerTradeWith
        {
            get
            {
                decimal maxSalary = SingleInfo!.Salary;
                var tempList = ThisMod!.PopUpList!.TextList.Select(items => items.DisplayText).ToCustomBasicList(); //hopefully this simple.
                var nextList = tempList.Select(items => PlayerList![items]).ToCustomBasicList();
                var maxOne = nextList.OrderByDescending(items => items.Salary).First();
                if (maxOne.Salary < maxSalary)
                    return "";
                return maxOne.NickName;
            }
        }
        protected override async Task ComputerTurnAsync()
        {
            if (DidChooseColors == false)
            {
                await ComputerChooseColorsAsync();
                return;
            }
            if (GameStatus == EnumWhatStatus.NeedChooseGender)
            {
                CustomBasicCollection<EnumGender> gList = new CustomBasicCollection<EnumGender> { EnumGender.Boy, EnumGender.Girl };
                await SelectGenderAsync(gList.GetRandomItem());
                return;
            }
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            switch (GameStatus)
            {

                case EnumWhatStatus.NeedChooseFirstOption:
                    await FirstOptionChosenAsync(EnumStart.Career); //computer will always choose career.
                    return;
                case EnumWhatStatus.NeedChooseFirstCareer:
                case EnumWhatStatus.NeedNewCareer:
                    if (ThisMod!.HandList!.HandList.Count > 0)
                    {
                        await ChoseCareerAsync(ThisMod.HandList.HandList.GetRandomItem().Deck);
                        return;
                    }
                    throw new BasicBlankException("Should have changed the status to salary");
                case EnumWhatStatus.NeedChooseStock:
                    await ChoseStockAsync(ThisMod!.HandList!.HandList.GetRandomItem().Deck);
                    return;
                case EnumWhatStatus.NeedChooseSalary:
                    await ChoseSalaryAsync(ThisMod!.HandList!.HandList.GetRandomItem().Deck);
                    return;
                case EnumWhatStatus.NeedToSpin:
                    if (SingleInfo!.CarIsInsured == false)
                    {
                        await PurchaseCarInsuranceAsync();
                        return;
                    }
                    if (SingleInfo.FirstStock == 0 && SingleInfo.SecondStock == 0)
                    {
                        await PurchasedStockAsync();
                        return;
                    }
                    if (SingleInfo.Salary < 80000 && CanTradeForBig(true))
                    {
                        await TradeForBigAsync();
                        return;
                    }
                    await StartSpinningAsync();
                    break;
                case EnumWhatStatus.NeedReturnStock:
                    if (ThisMod!.HandList!.HandList.Count != 2)
                        throw new BasicBlankException("Must have 2 choices to return.  Otherwise; should have returned automatically");
                    await StockReturnedAsync(ThisMod.HandList.HandList.GetRandomItem().Deck);
                    return;
                case EnumWhatStatus.NeedToChooseSpace:
                    int firstNum = ThisMod!.GameBoard1!.FirstPossiblePosition;
                    int secondNum = ThisMod.GameBoard1.SecondPossiblePosition;
                    if (firstNum == 0)
                        throw new BasicBlankException("The first possible position cannot be 0. Check this out");
                    if (secondNum == 0)
                        throw new BasicBlankException("The second possible position cannot be 0.  Otherwise, should have made move automatically");
                    CustomBasicList<int> posList = new CustomBasicList<int> { firstNum, secondNum };
                    await MakeMoveAsync(posList.GetRandomItem());
                    return;
                case EnumWhatStatus.NeedNight:
                case EnumWhatStatus.NeedToEndTurn:
                    if (ThisData!.MultiPlayer)
                        await ThisNet!.SendEndTurnAsync();
                    await EndTurnAsync();
                    return;
                case EnumWhatStatus.NeedStealTile:
                    int firstItem = ThisMod!.PopUpList!.ItemToChoose();
                    ThisMod.PlayerChosen = ThisMod.PopUpList.GetText(firstItem); //i think
                    await TilesStolenAsync();
                    return;
                case EnumWhatStatus.NeedChooseRetirement:
                    await ChoseRetirementAsync(EnumFinal.MillionaireEstates); //the computer will always choose millionaire estates
                    return;
                case EnumWhatStatus.NeedSellBuyHouse:
                case EnumWhatStatus.NeedChooseHouse:
                    await StartSpinningAsync(); //the computer will never choose a house.
                    return;
                case EnumWhatStatus.NeedTradeSalary:
                    string trade = ComputerTradeWith;
                    if (trade == "")
                    {
                        if (ThisData!.MultiPlayer)
                            await ThisNet!.SendEndTurnAsync();
                        await EndTurnAsync();
                        return;
                    }
                    ThisMod!.PlayerChosen = trade;
                    await TradedSalaryAsync();
                    break;
                default:
                    throw new BasicBlankException("Rethink for computer turn");
            }
        }
        #region "Move Processes"
        private async Task PossibleAutomateMoveAsync()
        {
            if (GameStatus != EnumWhatStatus.NeedToSpin && GameStatus != EnumWhatStatus.LastSpin)
            {
                await ContinueTurnAsync();
                return;
            }
            if (ThisMod!.GameBoard1!.FirstPossiblePosition > 0 && ThisMod.GameBoard1.SecondPossiblePosition > 0)
            {
                GameStatus = EnumWhatStatus.NeedToChooseSpace;
                await ContinueTurnAsync();
                return;
            }
            if (ThisMod.GameBoard1.SecondPossiblePosition > 0)
                throw new BasicBlankException("Can't have a second possible option but not the first one");
            await AutomateMoveAsync(ThisMod.GameBoard1.FirstPossiblePosition);
        }
        private async Task AutomateMoveAsync(int space)
        {
            ThisMod!.GameDetails = SpaceDescription(space);
            SaveRoot!.Instructions = "Making Move";
            ThisMod.GameBoard1!.NewPosition = space;
            if (space == ThisMod.GameBoard1.SecondPossiblePosition)
                await ThisMod.GameBoard1.AnimateMoveAsync(true);
            else
                await ThisMod.GameBoard1.AnimateMoveAsync(false);
            await MoveResultsAsync(space, SingleInfo!);
        }
        private async Task MoveResultsAsync(int index, LifeBoardGamePlayerItem thisPlayer)
        {
            var thisSpace = _thisGlobal!.SpaceList![index - 1];
            PaydayProcessing(thisPlayer);
            decimal newAmount;
            int newPlayer;
            if (thisSpace.GetLifeTile)
            {
                if (TilesLeft)
                {
                    ObtainLife(thisPlayer);
                    GameStatus = EnumWhatStatus.NeedToEndTurn;
                }
                else if (CanStealTile)
                {
                    GameStatus = EnumWhatStatus.NeedStealTile;
                }
                else
                {
                    GameStatus = EnumWhatStatus.NeedToEndTurn;
                }
            }
            switch (thisSpace.ActionInfo)
            {
                case EnumActionType.CollectPayMoney:
                    GameStatus = EnumWhatStatus.NeedToEndTurn;
                    if (thisSpace.AmountReceived < 0)
                    {
                        newAmount = Math.Abs(thisSpace.AmountReceived);
                        if (thisSpace.WhatInsurance == EnumInsuranceType.NeedCar)
                        {
                            if (thisPlayer.CarIsInsured == true)
                            {
                                ThisMod!.GameDetails = $"{ThisMod.GameDetails}{Constants.vbCrLf} Pay nothing because the player had car insurance";
                            }
                            else
                            {
                                ThisMod!.GameDetails = $"{thisSpace.Description}{Constants.vbCrLf}Pay {newAmount.ToCurrency(0)} because the player had no car insurance";
                                TakeOutExpense(newAmount);
                            }
                        }
                        else if (thisSpace.WhatInsurance == EnumInsuranceType.NeedHouse)
                        {
                            if (thisPlayer.HouseIsInsured || thisPlayer.GetHouseName() == "")
                            {
                                ThisMod!.GameDetails = $"{ThisMod.GameDetails}{Constants.vbCrLf} Pay nothing because the player had house insurance or had no house";
                            }
                            else
                            {
                                ThisMod!.GameDetails = $"{thisSpace.Description}{Constants.vbCrLf}Pay {newAmount.ToCurrency(0)} because the player had no house insurance";
                                TakeOutExpense(newAmount);
                            }
                        }
                        else
                        {
                            if (thisSpace.CareerSpace != EnumCareerType.None)
                            {
                                newPlayer = WhoHadCareerName(thisSpace.CareerSpace);
                                if (newPlayer == WhoTurn)
                                {
                                    ThisMod!.GameDetails = "Owe nothing because the player owns the career";
                                }
                                else if (newPlayer > 0)
                                {
                                    var tempPlayer = PlayerList![newPlayer];
                                    ThisMod!.GameDetails = $"{ThisMod.GameDetails}{Constants.vbCrLf} Pay {tempPlayer.NickName} since they own the career";
                                    CollectMoney(newPlayer, newAmount);
                                }
                            }
                            TakeOutExpense(newAmount);
                        }
                    }
                    else
                    {
                        CollectMoney(WhoTurn, thisSpace.AmountReceived); //i think
                    }

                    break;
                case EnumActionType.StartCareer:
                    GameStatus = EnumWhatStatus.NeedChooseFirstCareer;
                    break;
                case EnumActionType.AttendNightSchool:
                    GameStatus = EnumWhatStatus.NeedNight;
                    break;
                case EnumActionType.FindNewJob:
                    var tempList = thisPlayer.GetCareerList();
                    tempList.ForEach(thisCard => thisPlayer.Hand.RemoveSpecificItem(thisCard));
                    GameStatus = EnumWhatStatus.NeedNewCareer;
                    break;
                case EnumActionType.GetMarried:
                    GetMarriedProcess(thisPlayer);
                    break;
                case EnumActionType.ObtainLifeTile:
                    SaveRoot!.WasMarried = false;
                    if (GameStatus != EnumWhatStatus.NeedStealTile)
                        GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.GetPaid:
                    GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.GotBabyBoy:
                    GetBaby(thisPlayer, EnumGender.Boy);
                    SaveRoot!.WasMarried = false;
                    if (GameStatus != EnumWhatStatus.NeedStealTile)
                        GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.GotBabyGirl:
                    GetBaby(thisPlayer, EnumGender.Girl);
                    SaveRoot!.WasMarried = false;
                    if (GameStatus != EnumWhatStatus.NeedStealTile)
                        GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.HadTwins:
                    if (ThisData!.MultiPlayer && SingleInfo!.CanSendMessage(ThisData) == false)
                    {
                        ThisCheck!.IsEnabled = true; //wait to see if they got boys or girls.
                        return;
                    }
                    var genderList = new CustomBasicList<EnumGender> { EnumGender.Boy, EnumGender.Girl };
                    CustomBasicList<EnumGender> newList = new CustomBasicList<EnumGender> { genderList.GetRandomItem(), genderList.GetRandomItem() };
                    if (ThisData.MultiPlayer)
                        await ThisNet!.SendAllAsync("twins", newList);
                    await GetTwinsAsync(newList);
                    return;
                case EnumActionType.MayBuyHouse:
                    GameStatus = EnumWhatStatus.NeedChooseHouse;
                    break;
                case EnumActionType.MaySellHouse:
                    if (thisPlayer.HouseName == "")
                        GameStatus = EnumWhatStatus.NeedChooseHouse;
                    else
                        GameStatus = EnumWhatStatus.NeedSellBuyHouse;
                    break;
                case EnumActionType.PayTaxes:
                    if (thisSpace.CareerSpace != EnumCareerType.Accountant)
                        throw new BasicBlankException("Only accountants can show up for paying taxes");
                    newPlayer = WhoHadCareerName(EnumCareerType.Accountant);
                    newAmount = SingleInfo!.TaxesDue();
                    if (newPlayer == WhoTurn)
                    {
                        ThisMod!.GameDetails = "Owe no taxes because the player is the accountant";
                    }
                    else if (newPlayer != 0)
                    {
                        var tempPlayer = PlayerList![newPlayer];
                        ThisMod!.GameDetails = $"{ThisMod.GameDetails}{Constants.vbCrLf} Pay {tempPlayer.NickName} {newAmount.ToCurrency(0)} in taxes since they are the accountant";
                        CollectMoney(newPlayer, newAmount);
                    }
                    else
                    {
                        ThisMod!.GameDetails = $"{ThisMod.GameDetails}{Constants.vbCrLf} Pay {newAmount.ToCurrency(0)} in taxes";
                    }
                    TakeOutExpense(newAmount);
                    GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.SpinAgainIfBehind:
                    if (WhoTurn != ThisMod!.GameBoard1!.PlayerInLead)
                        GameStatus = EnumWhatStatus.NeedToSpin;
                    else
                        GameStatus = EnumWhatStatus.NeedToEndTurn;
                    break;
                case EnumActionType.StockCrashed:
                    if (thisPlayer.FirstStock > 0 && thisPlayer.SecondStock > 0)
                    {
                        GameStatus = EnumWhatStatus.NeedReturnStock;
                    }
                    else if (thisPlayer.FirstStock > 0 || thisPlayer.SecondStock > 0)
                    {
                        ThisMod!.GameDetails = $"{ThisMod.GameDetails}{Constants.vbCrLf} There was only one stock.  That has been returned automatically.";
                        await AutoReturnAsync();
                        return;
                    }
                    else
                    {
                        ThisMod!.GameDetails = $"{ThisMod.GameDetails}{Constants.vbCrLf} There was no stocks to return.";
                        GameStatus = EnumWhatStatus.NeedToEndTurn;
                    }
                    break;
                case EnumActionType.StockBoomed:
                    GameStatus = EnumWhatStatus.NeedChooseStock;
                    SaveRoot!.EndAfterStock = true;
                    break;
                case EnumActionType.TradeSalary:
                    SaveRoot!.EndAfterSalary = true;
                    GameStatus = EnumWhatStatus.NeedTradeSalary;
                    break;
                case EnumActionType.WillMissTurn:
                    GameStatus = EnumWhatStatus.NeedToEndTurn;
                    thisPlayer.MissNextTurn = true;
                    break;
                case EnumActionType.WillRetire:
                    await RetirementProcessAsync();
                    return;
                default:
                    throw new BasicBlankException("Move Results Status Not Found");
            }
            await ContinueTurnAsync();
        }
        #endregion
        #region "After Spinning"

        private async Task FinishSpinProcessAsync()
        {
            if (GameStatus == EnumWhatStatus.LastSpin)
                ThisE.RepaintBoard();
            else if (GameStatus == EnumWhatStatus.NeedFindSellPrice || GameStatus == EnumWhatStatus.NeedSellHouse)
                await SoldHouseAsync(ThisMod!.GameBoard1!.NumberRolled);
            else
                throw new BasicBlankException("When finishing spin process, not sure what to do.  Rethink");
        }
        private void EarnProcessFromRoll(int rolled)
        {
            int whoHad;
            if (rolled == 1 || rolled == 10)
            {
                EnumCareerType careerNeeded;
                if (rolled == 1)
                    careerNeeded = EnumCareerType.Artist;
                else
                    careerNeeded = EnumCareerType.PoliceOfficer;
                whoHad = WhoHadCareerName(careerNeeded);
                if (whoHad > 0 && whoHad != WhoTurn)
                {
                    TakeOutExpense(10000);
                    CollectMoney(whoHad, 10000);
                }
                if (rolled == 10)
                    return;
            }
            whoHad = WhoHadStock(rolled);
            if (whoHad > 0)
                CollectMoney(whoHad, 10000);
        }
        public async Task ResumeSpinnerCompletedAsync()
        {
            SaveRoot!.SpinList.Add(ThisMod!.GameBoard1!.NumberRolled);
            EarnProcessFromRoll(ThisMod.GameBoard1.NumberRolled);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            if (GameStatus == EnumWhatStatus.NeedToSpin)
                GameStatus = EnumWhatStatus.LastSpin;
            if (ThisMod.GameBoard1.NumberRolled == 0)
            {
                BetweenNumbers();
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    await ThisMod.ShowGameMessageAsync("Spin again because it landed between two numbers");
                if (GameStatus == EnumWhatStatus.LastSpin)
                    GameStatus = EnumWhatStatus.NeedToSpin;
                await ContinueTurnAsync();
                return;
            }
            await FinishSpinProcessAsync();
            if (CanTradeForBig(false))
            {
                await ThisMod.ShowGameMessageAsync("Current Player Is Getting The $100,000 for 2 8s, 9s or 10s spinned in a row for a lucky break for being entertainer");
                await TradeForBigAsync();
                return;
            }
            await PossibleAutomateMoveAsync();
        }
        #endregion
        #region "Spinning Processes"
        private SpinnerPositionData GetSpinData
        {
            get
            {
                SpinnerPositionData output = new SpinnerPositionData();
                output.CanBetween = Rs!.NextBool(10); //10 percent chance.
                output.HighSpeedUpTo = Rs.GetRandomNumber(30, 15);
                output.ChangePositions = Rs.GetRandomNumber(120, 60);
                return output;
            }
        }
        public async Task StartSpinningAsync()
        {
            var thisSpin = GetSpinData;
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("spin", thisSpin);
            await StartSpinningAsync(thisSpin); //i forgot this maybe.
        }
        public async Task StartSpinningAsync(SpinnerPositionData thisSpin)
        {
            if (SaveRoot!.GameStatus == EnumWhatStatus.NeedChooseHouse || SaveRoot.GameStatus == EnumWhatStatus.NeedSellBuyHouse)
            {
                SaveRoot.GameStatus = EnumWhatStatus.NeedToSpin;
                ThisMod!.HandList!.Visible = false;
            }
            await SpinnerAnimationClass.AnimateSpinAsync(thisSpin, this);
            var thisNumber = _thisGlobal!.GetNumberSpun(_thisGlobal.SpinnerCanvas!.Position);
            SaveRoot.SpinPosition = _thisGlobal.SpinnerCanvas.Position;
            SaveRoot.ChangePosition = 0;
            SaveRoot.NumberRolled = thisNumber;
            await ResumeSpinnerCompletedAsync(); //forgor this part.
        }
        #endregion
    }
}