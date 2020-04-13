using CommonBasicStandardLibraries.Exceptions;
using PaydayCP.Data;
using SkiaSharp;

namespace PaydayCP.Cards
{
    public class MailCard : CardInformation
    {
        public EnumMailType MailType { get; set; }
        public decimal AmountReceived { get; set; }
        public string Company { get; set; } = "";
        public string Description { get; set; } = "";
        public MailCard()
        {
            CardCategory = EnumCardCategory.Mail;
            DefaultSize = new SKSize(120, 85);
        }
        public override void Populate(int chosen)
        {
            int oldDeck;
            oldDeck = chosen - 24;
            if (oldDeck <= 0)
                throw new BasicBlankException("Must be 1 to 47.  If 0 is allowed; then I need to return new mail card");
            Deck = chosen;
            Index = oldDeck;
            switch (oldDeck)
            {
                case 1:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -5000;
                    Company = "Know-It-All University";
                    Description = "You will when you leave";
                    break;
                case 2:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -300;
                    Company = "Dr. Feemer";
                    Description = "Super Ski Weekend Set One Broken Leg";
                    break;
                case 3:
                    MailType = EnumMailType.Charity;
                    AmountReceived = -400;
                    Description = "Stop Polution";
                    break;
                case 4:
                    MailType = EnumMailType.MonsterCharge;
                    AmountReceived = -1000;
                    break;
                case 5:
                    MailType = EnumMailType.MadMoney;
                    AmountReceived = 300;
                    break;
                case 6:
                    MailType = EnumMailType.PayNeighbor;
                    AmountReceived = -2000;
                    break;
                case 7:
                    MailType = EnumMailType.MoveAhead; // that is it for this one
                    break;
                case 8:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -3000;
                    Company = "Dr. Patella";
                    Description = "New Knee (You Can Dance Again)";
                    break;
                case 9:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -300;
                    Company = "Zapp Electric Co.";
                    Description = "Shocking!";
                    break;
                case 10:
                    MailType = EnumMailType.PayNeighbor;
                    AmountReceived = -2000;
                    break;
                case 11:
                    MailType = EnumMailType.MadMoney;
                    AmountReceived = 100;
                    break;
                case 12:
                    MailType = EnumMailType.MonsterCharge;
                    AmountReceived = -3000;
                    break;
                case 13:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -2000;
                    Company = "M. Broider, M.D.";
                    Description = "Ten Stitches ($200 A Stitch)";
                    break;
                case 14:
                    MailType = EnumMailType.MoveAhead;
                    break;
                case 15:
                    MailType = EnumMailType.PayNeighbor;
                    AmountReceived = -300;
                    break;
                case 16:
                    MailType = EnumMailType.MadMoney;
                    AmountReceived = 400;
                    break;
                case 17:
                    MailType = EnumMailType.MonsterCharge;
                    AmountReceived = -4000;
                    break;
                case 18:
                    MailType = EnumMailType.Charity;
                    AmountReceived = -400;
                    Description = "Scholarship Drive";
                    break;
                case 19:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -700;
                    Company = "Big Noise Inc.";
                    Description = "Installed Car stereo system";
                    break;
                case 20:
                    MailType = EnumMailType.MoveAhead;
                    break;
                case 21:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -300;
                    Company = "The Six Tubas";
                    Description = "We played at your kid's birthday party";
                    break;
                case 22:
                    MailType = EnumMailType.MadMoney;
                    AmountReceived = 200;
                    break;
                case 23:
                    MailType = EnumMailType.PayNeighbor;
                    AmountReceived = -200;
                    break;
                case 24:
                    MailType = EnumMailType.Charity;
                    AmountReceived = -300;
                    Description = "Citizens for a clean environment";
                    break;
                case 25:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -800;
                    Company = "Boom Box Music Club";
                    Description = "(Went a little overboard didn't we)";
                    break;
                case 26:
                    MailType = EnumMailType.MoveAhead;
                    break;
                case 27:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -200;
                    Company = "Dr. I.M. Blurd";
                    Description = "One pair of designer eyeglasses";
                    break;
                case 28:
                    MailType = EnumMailType.MadMoney;
                    AmountReceived = 2000;
                    break;
                case 29:
                    MailType = EnumMailType.PayNeighbor;
                    AmountReceived = -300;
                    break;
                case 30:
                    MailType = EnumMailType.Charity;
                    AmountReceived = -400;
                    Description = "Whale Rescue Patrol";
                    break;
                case 31:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -1500;
                    Company = "Mo Larr, D.D.S.";
                    Description = "Fasion Braces";
                    break;
                case 32:
                    MailType = EnumMailType.MoveAhead;
                    break;
                case 33:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -100;
                    Company = "Pearl E. White, D.D.S.";
                    Description = "Drilling, filling, & billing";
                    break;
                case 34:
                    MailType = EnumMailType.MadMoney;
                    AmountReceived = 1000;
                    break;
                case 35:
                    MailType = EnumMailType.PayNeighbor;
                    AmountReceived = -300;
                    break;
                case 36:
                    MailType = EnumMailType.Charity;
                    AmountReceived = -400;
                    Description = "Save Threatened Species";
                    break;
                case 37:
                    MailType = EnumMailType.MadMoney;
                    AmountReceived = 2000;
                    break;
                case 38:
                    MailType = EnumMailType.PayNeighbor;
                    AmountReceived = -200;
                    break;
                case 39:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -200;
                    Company = "Tick Toc Inc.";
                    Description = "We cleaned your clock";
                    break;
                case 40:
                    MailType = EnumMailType.MoveAhead;
                    break;
                case 41:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -600;
                    Company = "Yakity-Yak Telephone Co.";
                    Description = "(Get a boyfriend in the neighborhood!)";
                    break;
                case 42:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -1500;
                    Company = "Health Club Family Membership";
                    Description = "(Including pets)";
                    break;
                case 43:
                    MailType = EnumMailType.Charity;
                    AmountReceived = -200;
                    Description = "Support Local Recycling";
                    break;
                case 44:
                    MailType = EnumMailType.Bill;
                    AmountReceived = -2500;
                    Company = "Away We Go Travel Agency";
                    Description = "Two-week vacation in the sun";
                    break;
                case 45:
                    MailType = EnumMailType.MadMoney;
                    AmountReceived = 1500;
                    break;
                case 46:
                    MailType = EnumMailType.MoveAhead;
                    break;
                case 47:
                    MailType = EnumMailType.PayNeighbor;
                    AmountReceived = -1000;
                    break;
                default:
                    throw new BasicBlankException($"Deck must be between 1 and 47 for the mail, not {oldDeck}");
            }
        }
    }
}
