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
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Logic;
namespace LifeBoardGameCP.Cards
{
    public static class CardsModule
    {
        public static CareerInfo GetCareerCard(int deck)
        {
            CareerInfo thisCard = new CareerInfo() { Deck = deck, DegreeRequired = false };
            switch (deck)
            {
                case 1:
                    {
                        thisCard.Career = EnumCareerType.Accountant;
                        thisCard.Scale1 = EnumPayScale.DarkBlueGroup;
                        thisCard.DegreeRequired = true;
                        thisCard.Scale2 = EnumPayScale.RedGroup;
                        break;
                    }

                case 2:
                    {
                        thisCard.Career = EnumCareerType.Artist;
                        thisCard.Scale1 = EnumPayScale.DarkBlueGroup;
                        thisCard.Scale2 = EnumPayScale.RedGroup;
                        thisCard.Description = "Collect $10,000 from a player who buys your art (spins a \"1\").";
                        thisCard.Title = "A Sale";
                        break;
                    }

                case 3:
                    {
                        thisCard.Career = EnumCareerType.Athlete;
                        thisCard.Scale1 = EnumPayScale.DarkBlueGroup;
                        thisCard.Scale2 = EnumPayScale.RedGroup; // was red, not green for athlete
                        thisCard.Description = "You may trade in 4 LIFE tiles to get the yellow Salary card, trading salaries if necessary.";
                        thisCard.Title = "Big Leagues";
                        break;
                    }

                case 4:
                    {
                        thisCard.Career = EnumCareerType.ComputerConsultant;
                        thisCard.Scale1 = EnumPayScale.DarkBlueGroup;
                        thisCard.Scale2 = EnumPayScale.GreenGroup;
                        thisCard.Title = "Tech Support";
                        thisCard.Description = "Any time the spinner stops between numbers or comes off the track, collect 50000 to fix it.";
                        break;
                    }

                case 5:
                    {
                        thisCard.Career = EnumCareerType.Doctor; // that one was fine.
                        thisCard.DegreeRequired = true;
                        thisCard.Scale1 = EnumPayScale.YellowGroup;
                        thisCard.Scale2 = EnumPayScale.GreenGroup; // messed up in the old. got transferred to the new.
                        break;
                    }

                case 6:
                    {
                        thisCard.Career = EnumCareerType.Entertainer;
                        thisCard.Scale1 = EnumPayScale.RedGroup;
                        thisCard.Scale2 = EnumPayScale.DarkBlueGroup;
                        thisCard.Title = "Big Break.";
                        thisCard.Description = "If two 8s, 9s, or 10s, are spun in a row, replace your salary with the yellow Salary card, trading salaries if necessary.";
                        break;
                    }

                case 7:
                    {
                        thisCard.Career = EnumCareerType.PoliceOfficer;
                        thisCard.Scale1 = EnumPayScale.RedGroup;
                        thisCard.Scale2 = EnumPayScale.GreenGroup;
                        thisCard.Title = "Highway Patrol";
                        thisCard.Description = "Collect $10,000 from any opponent who speeds (spins a \"10\").";
                        break;
                    }

                case 8:
                    {
                        thisCard.Career = EnumCareerType.SalesPerson;
                        thisCard.Scale1 = EnumPayScale.RedGroup;
                        thisCard.Scale2 = EnumPayScale.GreenGroup;
                        thisCard.Title = "Commission";
                        thisCard.Description = "Collect $5,000 when another player buys stock or insurance.";
                        break;
                    }

                case 9:
                    {
                        thisCard.Career = EnumCareerType.Teacher;
                        thisCard.Scale1 = EnumPayScale.RedGroup;
                        thisCard.Scale2 = EnumPayScale.GreenGroup;
                        thisCard.Title = "Summer Job";
                        thisCard.Description = "You may draw a Career card after all players have a job.  You do not get a salary for this career but get the special benefits.";
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Must be between 1 and 9; not " + deck);
                    }
            }
            return thisCard;
        }
        public static HouseInfo GetHouseCard(int deck)
        {
            var thisCard = new HouseInfo() { Deck = deck };
            switch (deck)
            {
                case 10:
                    {
                        thisCard.HouseCategory = EnumHouseType.BeachHouse;
                        thisCard.HousePrice = 140000;
                        thisCard.InsuranceCost = 35000;
                        var tempList = new CustomBasicList<decimal>() { 100000, 100000, 130000, 130000, 130000, 130000, 130000, 130000, 150000, 150000 };
                        thisCard.SellingPrices = tempList.GetSellingPrices();
                        thisCard.Title = "High Winds Realty";
                        thisCard.Description = "Only 50 yeards from Monsoon Beach.  Sun deck, boat deck, hurricane wall.  Hurry while it lasts!";
                        break;
                    }

                case 11:
                    {
                        thisCard.HouseCategory = EnumHouseType.CozyCondo;
                        thisCard.Title = "Sandwich Inn Realty";
                        thisCard.Description = "This beautiful 2-level condominium comes complete with gourmet kitchen and rooftop garden.";
                        thisCard.HousePrice = 100000;
                        thisCard.InsuranceCost = 25000;
                        var tempList = new CustomBasicList<decimal>() { 90000, 90000, 90000, 90000, 110000, 110000, 110000, 110000, 125000, 125000 };
                        thisCard.SellingPrices = tempList.GetSellingPrices();
                        break;
                    }

                case 12:
                    {
                        thisCard.HouseCategory = EnumHouseType.DutchColonial;
                        thisCard.Title = "Wooden Shoe Realty";
                        thisCard.Description = "8 spacious rooms w/study, den.  Solar-heated, wood-burning stoves, solid oak floors.";
                        thisCard.HousePrice = 120000;
                        thisCard.InsuranceCost = 30000;
                        var TempList = new CustomBasicList<decimal>() { 100000, 100000, 130000, 130000, 130000, 130000, 130000, 130000, 150000, 150000 };
                        thisCard.SellingPrices = TempList.GetSellingPrices();
                        break;
                    }

                case 13:
                    {
                        thisCard.HouseCategory = EnumHouseType.FarmHouse;
                        thisCard.Title = "Eurell B. Milken Realty";
                        thisCard.Description = "Located on 50 rolling acres!  Garbanzo bean crops, prizewinning pigs & daily cows.  Spacious barn w/silo.";
                        thisCard.HousePrice = 160000;
                        thisCard.InsuranceCost = 40000;
                        var tempList = new CustomBasicList<decimal>() { 140000, 140000, 180000, 180000, 180000, 180000, 180000, 200000, 200000, 200000 };
                        thisCard.SellingPrices = tempList.GetSellingPrices();
                        break;
                    }

                case 14:
                    {
                        thisCard.HouseCategory = EnumHouseType.LogCabin;
                        thisCard.Title = "Rod N. Realty";
                        thisCard.Description = "Rustic charm in a woodland setting.  Loft w/skylight, stone fireplace.  Near Lake Ketcheefishee.";
                        thisCard.HousePrice = 80000;
                        thisCard.InsuranceCost = 20000;
                        var templist = new CustomBasicList<decimal>() { 70000, 70000, 70000, 80000, 80000, 80000, 80000, 80000, 100000, 100000 };
                        thisCard.SellingPrices = templist.GetSellingPrices();
                        break;
                    }

                case 15:
                    {
                        thisCard.HouseCategory = EnumHouseType.MobileHome;
                        thisCard.Title = "Deals On Wheels Realty";
                        thisCard.Description = "Aluminum-sided little beauty!  Great location, lovely view.  Trailer hitch included.";
                        thisCard.HousePrice = 60000;
                        thisCard.InsuranceCost = 15000;
                        var tempList = new CustomBasicList<decimal>() { 30000, 30000, 30000, 60000, 60000, 60000, 60000, 80000, 80000, 80000 };
                        thisCard.SellingPrices = tempList.GetSellingPrices();
                        break;
                    }

                case 16:
                    {
                        thisCard.HouseCategory = EnumHouseType.SplitLevel;
                        thisCard.Title = "Faultline Realty";
                        thisCard.Description = "Was one level before the 'quake.  Now a real fixer-upper for adventurous folks!";
                        thisCard.HousePrice = 40000;
                        thisCard.InsuranceCost = 10000;
                        var tempList = new CustomBasicList<decimal>() { 20000, 20000, 20000, 20000, 40000, 40000, 40000, 40000, 60000, 60000 };
                        thisCard.SellingPrices = tempList.GetSellingPrices();
                        break;
                    }

                case 17:
                    {
                        thisCard.HouseCategory = EnumHouseType.Tudor;
                        thisCard.Title = "Tutime Realty";
                        thisCard.Description = "Tufloors, tubaths, tucar garage.  Perfect for tupeople with tukids or more!";
                        thisCard.HousePrice = 180000;
                        thisCard.InsuranceCost = 45000;
                        var tempList = new CustomBasicList<decimal>() { 170000, 170000, 215000, 215000, 215000, 215000, 215000, 215000, 215000, 250000 };
                        thisCard.SellingPrices = tempList.GetSellingPrices();
                        break;
                    }

                case 18:
                    {
                        thisCard.HouseCategory = EnumHouseType.Victorian;
                        thisCard.Title = "Blithering Heights Realty";
                        thisCard.Description = "Library, parlor, servants' quarters, marble fireplaces, wraparound porch.";
                        thisCard.HousePrice = 200000;
                        thisCard.InsuranceCost = 50000;
                        var tempList = new CustomBasicList<decimal>() { 180000, 180000, 225000, 225000, 225000, 225000, 225000, 225000, 300000, 300000 };
                        thisCard.SellingPrices = tempList.GetSellingPrices();
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Must be between 10 and 18; not " + deck);
                    }
            }
            return thisCard;
        }
        public static SalaryInfo GetSalaryCard(int deck)
        {
            var thisCard = new SalaryInfo() { Deck = deck };
            switch (deck)
            {
                case 19:
                    {
                        thisCard.PayCheck = 100000;
                        thisCard.TaxesDue = 45000;
                        thisCard.WhatGroup = EnumPayScale.YellowGroup;
                        break;
                    }

                case 20:
                    {
                        thisCard.PayCheck = 60000;
                        thisCard.TaxesDue = 25000;
                        thisCard.WhatGroup = EnumPayScale.GreenGroup;
                        break;
                    }

                case 21:
                    {
                        thisCard.PayCheck = 50000;
                        thisCard.TaxesDue = 20000;
                        thisCard.WhatGroup = EnumPayScale.DarkBlueGroup;
                        break;
                    }

                case 22:
                    {
                        thisCard.PayCheck = 40000;
                        thisCard.TaxesDue = 15000;
                        thisCard.WhatGroup = EnumPayScale.RedGroup;
                        break;
                    }

                case 23:
                    {
                        thisCard.PayCheck = 90000;
                        thisCard.TaxesDue = 40000;
                        thisCard.WhatGroup = EnumPayScale.GreenGroup;
                        break;
                    }

                case 24:
                    {
                        thisCard.PayCheck = 80000;
                        thisCard.TaxesDue = 35000;
                        thisCard.WhatGroup = EnumPayScale.RedGroup;
                        break;
                    }

                case 25:
                    {
                        thisCard.PayCheck = 20000;
                        thisCard.TaxesDue = 5000;
                        thisCard.WhatGroup = EnumPayScale.DarkBlueGroup;
                        break;
                    }

                case 26:
                    {
                        thisCard.PayCheck = 30000;
                        thisCard.TaxesDue = 10000;
                        thisCard.WhatGroup = EnumPayScale.RedGroup;
                        break;
                    }

                case 27:
                    {
                        thisCard.PayCheck = 70000;
                        thisCard.TaxesDue = 30000;
                        thisCard.WhatGroup = EnumPayScale.GreenGroup;
                        break;
                    }

                default:
                    {
                        throw new Exception("Must be between 19 and 27; not " + deck);
                    }
            }
            return thisCard;
        }
        public static StockInfo GetStockCard(int deck)
        {
            if (deck < 28 || deck > 36)
                throw new Exception("Must be between 28 and 36; not " + deck);
            int newNum;
            newNum = deck - 27;
            return new StockInfo() { Deck = deck, Value = newNum };
        }
        public static TileInfo GetTileInfo(int deck)
        {
            switch (deck)
            {
                case 1:
                    {
                        return new TileInfo() { AmountReceived = 30000, Description = "Compose a Symphony" };
                    }

                case 2:
                    {
                        return new TileInfo() { AmountReceived = 10000, Description = "Invent New Ice Cream Flavor" };
                    }

                case 3:
                    {
                        return new TileInfo() { AmountReceived = 20000, Description = "Pulitzer Prize" };
                    }

                case 4:
                    {
                        return new TileInfo() { AmountReceived = 20000, Description = "Discover New Planet" };
                    }

                case 5:
                    {
                        return new TileInfo() { AmountReceived = 40000, Description = "Find New Energy Source" };
                    }

                case 6:
                    {
                        return new TileInfo() { AmountReceived = 10000, Description = "Swim English Channel" };
                    }

                case 7:
                    {
                        return new TileInfo() { AmountReceived = 50000, Description = "Become President" };
                    }

                case 8:
                    {
                        return new TileInfo() { AmountReceived = 10000, Description = "Build Better Mousetrap" };
                    }

                case 9:
                    {
                        return new TileInfo() { AmountReceived = 30000, Description = "Paint a Masterpiece" };
                    }

                case 10:
                    {
                        return new TileInfo() { AmountReceived = 10000, Description = "Win Dance Contest" };
                    }

                case 11:
                    {
                        return new TileInfo() { AmountReceived = 20000, Description = "Design New Computer" };
                    }

                case 12:
                    {
                        return new TileInfo() { AmountReceived = 20000, Description = "Lifetime Achievement Award" };
                    }

                case 13:
                    {
                        return new TileInfo() { AmountReceived = 30000, Description = "Family Horse Wins Derby" };
                    }

                case 14:
                    {
                        return new TileInfo() { AmountReceived = 40000, Description = "Cure the Common Cold" };
                    }

                case 15:
                    {
                        return new TileInfo() { AmountReceived = 10000, Description = "Run World Record Mile" };
                    }

                case 16:
                    {
                        return new TileInfo() { AmountReceived = 20000, Description = "Humanitarian Award" };
                    }

                case 17:
                    {
                        return new TileInfo() { AmountReceived = 30000, Description = "Toy Invention Sells Big" };
                    }

                case 18:
                    {
                        return new TileInfo() { AmountReceived = 40000, Description = "Create New Teaching Method" };
                    }

                case 19:
                    {
                        return new TileInfo() { AmountReceived = 50000, Description = "Solution to Pollution" };
                    }

                case 20:
                    {
                        return new TileInfo() { AmountReceived = 10000, Description = "Climb Mt. Everest" };
                    }

                case 21:
                    {
                        return new TileInfo() { AmountReceived = 10000, Description = "Invent New Sport" };
                    }

                case 22:
                    {
                        return new TileInfo() { AmountReceived = 50000, Description = "Nobel Peace Prize" };
                    }

                case 23:
                    {
                        return new TileInfo() { AmountReceived = 30000, Description = "Write Great American Novel" };
                    }

                case 24:
                    {
                        return new TileInfo() { AmountReceived = 40000, Description = "Save Endangered Species" };
                    }

                case 25:
                    {
                        return new TileInfo() { AmountReceived = 20000, Description = "Open Health Food Chain" };
                    }

                default:
                    {
                        throw new Exception("Must be between 1 and 25; not " + deck);
                    }
            }
        }
    }
}
