using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using LifeBoardGameCP.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeBoardGameCP.Logic
{
    internal static class LoadSpaceProcesses
    {
        internal static void PopulateSpaces(LifeBoardGameGameContainer gameContainer)
        {
            SpaceInfo space;
            for (var x = 1; x <= 147; x++)
            {
                space = new SpaceInfo();
                switch (x)
                {
                    case 1:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 20000;
                            space.Description = "Scholarship!";
                            break;
                        }

                    case 2:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -5000;
                            space.Description = "Buy books and supplies.";
                            break;
                        }

                    case 3:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Make new friends.";
                            break;
                        }

                    case 4:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 5000;
                            space.Description = "Part time job.";
                            break;
                        }

                    case 5:
                        {
                            space.ActionInfo = EnumActionType.WillMissTurn;
                            space.Description = "Study for exams.";
                            break;
                        }

                    case 6:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Study abroad.";
                            break;
                        }

                    case 7:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -5000;
                            space.Description = "Spring Break!";
                            break;
                        }

                    case 8:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Dean's List!";
                            break;
                        }

                    case 9:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -5000;
                            space.Description = "Your buddies crash your car.";
                            space.WhatInsurance = EnumInsuranceType.NeedCar;
                            break;
                        }

                    case 10:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Graduation day!";
                            break;
                        }

                    case 11:
                        {
                            space.ActionInfo = EnumActionType.StartCareer;
                            space.Description = "CAREER CHOICE!";
                            break;
                        }

                    case 12:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 13:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -5000;
                            space.Description = "Rent apartment.";
                            break;
                        }

                    case 14:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 10000;
                            space.Description = "Inheritance.";
                            break;
                        }

                    case 15:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 16:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Adopt a pet.";
                            break;
                        }

                    case 17:
                        {
                            space.ActionInfo = EnumActionType.WillMissTurn;
                            space.Description = "Lost!";
                            break;
                        }

                    case 18:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Birthday Party!";
                            break;
                        }

                    case 19:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.CareerSpace = EnumCareerType.Doctor;
                            space.AmountReceived = -5000;
                            space.Description = "Ski accident";
                            break;
                        }

                    case 20:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 10000;
                            space.Description = "Win marathan";
                            break;
                        }

                    case 21:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Visit a muesum.";
                            break;
                        }

                    case 22:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Cycle to work.";
                            break;
                        }

                    case 23:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 24:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -15000;
                            space.Description = "Car rolls away.";
                            space.WhatInsurance = EnumInsuranceType.NeedCar;
                            break;
                        }

                    case 25:
                        {
                            space.ActionInfo = EnumActionType.GetMarried;
                            break;
                        }

                    case 26:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -10000;
                            space.Description = "Wedding reception.";
                            break;
                        }

                    case 27:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Happy honeymoon!";
                            break;
                        }

                    case 28:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -10000;
                            space.CareerSpace = EnumCareerType.SalesPerson;
                            space.Description = "Upgrade computer.";
                            break;
                        }

                    case 29:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -10000;
                            space.Description = "Car accident";
                            space.WhatInsurance = EnumInsuranceType.NeedCar;
                            break;
                        }

                    case 30:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -10000;
                            space.Description = "Attend high-tech seminar.";
                            space.CareerSpace = EnumCareerType.ComputerConsultant;
                            break;
                        }

                    case 31:
                        {
                            space.ActionInfo = EnumActionType.AttendNightSchool;
                            break;
                        }

                    case 32:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 33:
                        {
                            space.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 34:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 50000;
                            space.Description = "Win lottery!";
                            break;
                        }

                    case 35:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Visit in-laws.";
                            break;
                        }

                    case 36:
                        {
                            space.ActionInfo = EnumActionType.MayBuyHouse;
                            break;
                        }

                    case 37:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 38:
                        {
                            space.ActionInfo = EnumActionType.FindNewJob;
                            space.Description = "Lose your job." + Constants.vbCrLf + "Start new career.";
                            break;
                        }

                    case 39:
                        {
                            space.ActionInfo = EnumActionType.GotBabyBoy;
                            break;
                        }

                    case 40:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -5000;
                            space.Description = "Furnish baby room.";
                            space.CareerSpace = EnumCareerType.SalesPerson;
                            break;
                        }

                    case 41:
                        {
                            space.ActionInfo = EnumActionType.GotBabyGirl;
                            break;
                        }

                    case 42:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 10000;
                            space.Description = "Win talent show.";
                            break;
                        }

                    case 43:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 44:
                        {
                            space.ActionInfo = EnumActionType.HadTwins;
                            space.Description = "Twins!";
                            break;
                        }

                    case 45:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -20000;
                            space.Description = "50-yard line seats at the big game.";
                            space.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 46:
                        {
                            space.ActionInfo = EnumActionType.GotBabyGirl;
                            break;
                        }

                    case 47:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -5000;
                            space.Description = "Attend Hollywood movie premier.";
                            space.CareerSpace = EnumCareerType.Entertainer;
                            break;
                        }

                    case 48:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -40000;
                            space.Description = "House flooded!";
                            space.WhatInsurance = EnumInsuranceType.NeedHouse;
                            break;
                        }

                    case 49:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -5000;
                            space.Description = "Family physicals.";
                            space.CareerSpace = EnumCareerType.Doctor;
                            break;
                        }

                    case 50:
                        {
                            space.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 51:
                        {
                            space.ActionInfo = EnumActionType.GotBabyBoy;
                            break;
                        }

                    case 52:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 53:
                        {
                            space.ActionInfo = EnumActionType.GotBabyGirl;
                            break;
                        }

                    case 54:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -1500;
                            space.Description = "Tree falls on house.";
                            space.WhatInsurance = EnumInsuranceType.NeedHouse;
                            break;
                        }

                    case 55:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Return lost wallet.";
                            break;
                        }

                    case 56:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -5000;
                            space.Description = "Buy high-definition TV.";
                            space.CareerSpace = EnumCareerType.SalesPerson;
                            break;
                        }

                    case 57:
                        {
                            space.ActionInfo = EnumActionType.StockBoomed;
                            break;
                        }

                    case 58:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Family picnic.";
                            break;
                        }

                    case 59:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Visit Mount Rushmore.";
                            break;
                        }

                    case 60:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 61:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -15000;
                            space.Description = "Car stolen!";
                            space.WhatInsurance = EnumInsuranceType.NeedCar;
                            break;
                        }

                    case 62:
                        {
                            space.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 63:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Run for mayor.";
                            break;
                        }

                    case 64:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Vote!";
                            break;
                        }

                    case 65:
                        {
                            space.ActionInfo = EnumActionType.GotBabyBoy;
                            break;
                        }

                    case 66:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 67:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -25000;
                            space.Description = "Buy luxury cruise online.";
                            space.CareerSpace = EnumCareerType.ComputerConsultant;
                            break;
                        }

                    case 68:
                        {
                            space.ActionInfo = EnumActionType.AttendNightSchool;
                            break;
                        }

                    case 69:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Learn CPR.";
                            break;
                        }

                    case 70:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -20000;
                            space.Description = "Art auction.";
                            space.CareerSpace = EnumCareerType.Artist;
                            break;
                        }

                    case 71:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 72:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Volunteer at charity sports event.";
                            break;
                        }

                    case 73:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 10000;
                            space.Description = "Win photography contest.";
                            break;
                        }

                    case 74:
                        {
                            space.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 75:
                        {
                            space.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 76:
                        {
                            space.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 77:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -25000;
                            space.Description = "Tennis camp.";
                            space.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 78:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -25000;
                            space.Description = "Donate computer network.";
                            space.CareerSpace = EnumCareerType.ComputerConsultant;
                            break;
                        }

                    case 79:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 80:
                        {
                            space.ActionInfo = EnumActionType.StockCrashed;
                            break;
                        }

                    case 81:
                        {
                            space.ActionInfo = EnumActionType.MaySellHouse;
                            break;
                        }

                    case 82:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.PerKid = true;
                            space.AmountReceived = -5000;
                            space.Description = "Day care.";
                            space.CareerSpace = EnumCareerType.Teacher;
                            break;
                        }

                    case 83:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 80000;
                            space.Description = "Write best-seller.";
                            break;
                        }

                    case 84:
                        {
                            space.ActionInfo = EnumActionType.HadTwins;
                            space.Description = "Adopt twins!";
                            break;
                        }

                    case 85:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -15000;
                            space.Description = "Invest in broadway play.";
                            space.CareerSpace = EnumCareerType.Entertainer;
                            break;
                        }

                    case 86:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 87:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Join health club.";
                            break;
                        }

                    case 88:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -35000;
                            space.Description = "Family portrait.";
                            space.CareerSpace = EnumCareerType.Artist;
                            break;
                        }

                    case 89:
                        {
                            space.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 90:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -25000;
                            space.Description = "Buy sport utility vehicle.";
                            space.CareerSpace = EnumCareerType.SalesPerson;
                            break;
                        }

                    case 91:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 75000;
                            space.Description = "Tax refund.";
                            break;
                        }

                    case 92:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -15000;
                            space.Description = "Host Police Charity Ball!";
                            space.CareerSpace = EnumCareerType.PoliceOfficer;
                            break;
                        }

                    case 93:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 94:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 80000;
                            space.Description = "Find buried treasure!";
                            break;
                        }

                    case 95:
                        {
                            space.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 96:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 97:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.CareerSpace = EnumCareerType.Artist;
                            space.AmountReceived = -25000;
                            space.Description = "Donate to Art Institute.";
                            break;
                        }

                    case 98:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Recycle.";
                            break;
                        }

                    case 99:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 95000;
                            space.Description = "TV Game show winner!";
                            break;
                        }

                    case 100:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -5000;
                            space.Description = "Summer School.";
                            space.PerKid = true;
                            space.CareerSpace = EnumCareerType.Teacher;
                            break;
                        }

                    case 101:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Have a Family Game Night.";
                            break;
                        }

                    case 102:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Learn sign language.";
                            break;
                        }

                    case 103:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -90000;
                            space.Description = "Buy lakeside cabin.";
                            break;
                        }

                    case 104:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 105:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -50000;
                            space.Description = "Burglar!";
                            space.WhatInsurance = EnumInsuranceType.NeedHouse;
                            break;
                        }

                    case 106:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 10000;
                            space.Description = "Win Nobel Peace Prize.";
                            break;
                        }

                    case 107:
                        {
                            space.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 108:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -30000;
                            space.Description = "Buy home gym.";
                            space.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 109:
                        {
                            space.ActionInfo = EnumActionType.StockCrashed;
                            break;
                        }

                    case 110:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -125000;
                            space.Description = "Tornado hits house!";
                            space.WhatInsurance = EnumInsuranceType.NeedHouse;
                            break;
                        }

                    case 111:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 112:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -25000;
                            space.Description = "Life-saving operation.";
                            space.CareerSpace = EnumCareerType.Doctor;
                            break;
                        }

                    case 113:
                        {
                            space.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 114:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -30000;
                            space.Description = "Buy sailboat.";
                            space.CareerSpace = EnumCareerType.SalesPerson;
                            break;
                        }

                    case 115:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -35000;
                            space.Description = "Sponsor golf tournament.";
                            space.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 116:
                        {
                            space.ActionInfo = EnumActionType.FindNewJob;
                            space.Description = "Mid-life crisis." + Constants.vbCrLf + "Start new career.";
                            break;
                        }

                    case 117:
                        {
                            space.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 118:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 119:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -100000;
                            space.CareerSpace = EnumCareerType.Entertainer;
                            space.Description = "Host on-line concert.";
                            break;
                        }

                    case 120:
                        {
                            space.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 121:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Help the homeless.";
                            break;
                        }

                    case 122:
                        {
                            space.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 123:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -100000;
                            space.Description = "Have cosmetic surgery.";
                            space.CareerSpace = EnumCareerType.Doctor;
                            break;
                        }

                    case 124:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -50000;
                            space.Description = "College.";
                            space.CareerSpace = EnumCareerType.Teacher;
                            break;
                        }

                    case 125:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 126:
                        {
                            space.ActionInfo = EnumActionType.PayTaxes;
                            break;
                        }

                    case 127:
                        {
                            space.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 128:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Visit memorial.";
                            break;
                        }

                    case 129:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -125000;
                            space.CareerSpace = EnumCareerType.Artist;
                            space.Description = "Sponsor art exhihit.";
                            break;
                        }

                    case 130:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Grand Canyon vacation.";
                            break;
                        }

                    case 131:
                        {
                            space.ActionInfo = EnumActionType.TradeSalary;
                            break;
                        }

                    case 132:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 133:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Go fishing.";
                            break;
                        }

                    case 134:
                        {
                            space.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 135:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -65000;
                            space.Description = "Hire jockey for your racehorse.";
                            space.CareerSpace = EnumCareerType.Athlete;
                            break;
                        }

                    case 136:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Go hiking.";
                            break;
                        }

                    case 137:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 138:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Plant a tree.";
                            break;
                        }

                    case 139:
                        {
                            space.ActionInfo = EnumActionType.SpinAgainIfBehind;
                            break;
                        }

                    case 140:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "Support wildlife fund.";
                            break;
                        }

                    case 141:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -45000;
                            space.CareerSpace = EnumCareerType.ComputerConsultant;
                            space.Description = "Have a web site designed.";
                            break;
                        }

                    case 142:
                        {
                            space.ActionInfo = EnumActionType.ObtainLifeTile;
                            space.Description = "You're a grandparent!";
                            break;
                        }

                    case 143:
                        {
                            space.ActionInfo = EnumActionType.GetPaid;
                            break;
                        }

                    case 144:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -35000;
                            space.Description = "Throw party for entertainment award winners.";
                            space.CareerSpace = EnumCareerType.Entertainer;
                            break;
                        }

                    case 145:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = -45000;
                            space.Description = "Invest in E-commerce company.";
                            space.CareerSpace = EnumCareerType.ComputerConsultant;
                            break;
                        }

                    case 146:
                        {
                            space.ActionInfo = EnumActionType.CollectPayMoney;
                            space.AmountReceived = 20000;
                            space.Description = "Pension";
                            break;
                        }

                    case 147:
                        {
                            space.ActionInfo = EnumActionType.WillRetire;
                            break;
                        }
                }
                if (space.ActionInfo == EnumActionType.ObtainLifeTile)
                    space.GetLifeTile = true;
                if (space.ActionInfo == EnumActionType.GotBabyBoy)
                    space.GetLifeTile = true;
                if (space.ActionInfo == EnumActionType.GotBabyGirl)
                    space.GetLifeTile = true;
                if (space.ActionInfo == EnumActionType.HadTwins)
                    space.GetLifeTile = true;
                if (space.ActionInfo == EnumActionType.GetMarried)
                    space.GetLifeTile = true;
                if (space.ActionInfo == EnumActionType.TradeSalary)
                    space.IsOptional = true;
                if (space.ActionInfo == EnumActionType.MayBuyHouse)
                    space.IsOptional = true;
                if (space.ActionInfo == EnumActionType.MaySellHouse)
                    space.IsOptional = true;
                if (space.ActionInfo == EnumActionType.AttendNightSchool)
                {
                    space.IsOptional = true;
                    space.AmountReceived = -20000;
                    space.CareerSpace = EnumCareerType.Teacher;
                }
                if (space.ActionInfo == EnumActionType.PayTaxes)
                    space.CareerSpace = EnumCareerType.Accountant;
                gameContainer.SpaceList.Add(space);
            }
        }
    }
}
