using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
namespace PaydayCP
{
    public class DealCard : CardInformation
    {
        public DealCard()
        {
            DefaultSize = new SKSize(100, 70);
            CardCategory = EnumCardCategory.Deal;
        }
        public string Business { get; set; } = "";
        public decimal Cost { get; set; }
        public decimal Value { get; set; }
        internal decimal Profit()
        {
            return Value - Cost;
        }
        public override void Populate(int chosen)
        {
            if (chosen == 0)
                throw new BasicBlankException("Must be between 1 And 24.  If 0 Is allowed; then I need to return a New deal card");
            Deck = chosen;
            Index = Deck;
            switch (chosen)
            {
                case 1:
                    Business = "Rock Ship International";
                    Cost = 12000;
                    Value = 20000;
                    break;
                case 2:
                    Business = "Pop's Soda Pop Inc.";
                    Cost = 3500;
                    Value = 6000;
                    break;
                case 3:
                    Business = "Heavenly Pink Cosmetic Company";
                    Cost = 3500;
                    Value = 6000;
                    break;
                case 4:
                    Business = "Humungous Hippos Ltd.";
                    Cost = 10000;
                    Value = 18000;
                    break;
                case 5:
                    Business = "Chuckles Comedy Club";
                    Cost = 3000;
                    Value = 6500;
                    break;
                case 6:
                    Business = "Hot Rock Jewelry Co.";
                    Cost = 11000;
                    Value = 16000;
                    break;
                case 7:
                    Business = "Fly-By-Night Airlines";
                    Cost = 11000;
                    Value = 19000;
                    break;
                case 8:
                    Business = "Burger 'N' Buns";
                    Cost = 2000;
                    Value = 6000;
                    break;
                case 9:
                    Business = "Louie's Limbo Inc.";
                    Cost = 6000;
                    Value = 11000;
                    break;
                case 10:
                    Business = "Dipsydoodle Noodles";
                    Cost = 8000;
                    Value = 12000;
                    break;
                case 11:
                    Business = "Sahara Safari";
                    Cost = 9500;
                    Value = 14000;
                    break;
                case 12:
                    Business = "Pete's Pizza Palace";
                    Cost = 8000;
                    Value = 12000;
                    break;
                case 13:
                    Business = "Galloping Golf Ball Co.";
                    Cost = 8000;
                    Value = 12000;
                    break;
                case 14:
                    Business = "Teen Jeans Inc.";
                    Cost = 7000;
                    Value = 14000;
                    break;
                case 15:
                    Business = "Shepard's Pie Co.";
                    Cost = 3500;
                    Value = 6500;
                    break;
                case 16:
                    Business = "Tippytoe Ballet School";
                    Cost = 2000;
                    Value = 5500;
                    break;
                case 17:
                    Business = "Rock More Records";
                    Cost = 10000;
                    Value = 15000;
                    break;
                case 18:
                    Business = "Wheel 'N' Squeals Skateboards";
                    Cost = 2000;
                    Value = 5500;
                    break;
                case 19:
                    Business = "Miss Muffet's Tuffets";
                    Cost = 4000;
                    Value = 7500;
                    break;
                case 20:
                    Business = "Gotcha Security Inc.";
                    Cost = 2500;
                    Value = 6500;
                    break;
                case 21:
                    Business = "Everglades Condo";
                    Cost = 12000;
                    Value = 20000;
                    break;
                case 22:
                    Business = "Laughing Gas Inc.";
                    Cost = 7500;
                    Value = 13000;
                    break;
                case 23:
                    Business = "Fish 'N' Cheaps Pet Store";
                    Cost = 3500;
                    Value = 7000;
                    break;
                case 24:
                    Business = "Yum Yum Yogurt";
                    Cost = 5000;
                    Value = 9000;
                    break;
                default:
                    throw new BasicBlankException($"Deck must be between 1 and 24 for the deal, not {chosen}");
            }
        }
    }
}