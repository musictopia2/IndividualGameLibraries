using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using FluxxCP.Data;

namespace FluxxCP.Cards
{
    public class RuleCard : FluxxCardInformation
    {
        public RuleCard()
        {
            CardType = EnumCardType.Rule;
        }
        public EnumRuleCategory Category { get; set; }
        public bool PlayOptional() => Deck == EnumRuleText.RichBonus;
        public bool EverythingByOne() => Deck == EnumRuleText.Inflation;
        public bool Play2Goals() => Deck == EnumRuleText.DoubleAgenda;
        public new EnumRuleText Deck
        {
            get
            {
                return (EnumRuleText)base.Deck;
            }
            set
            {
                base.Deck = (int)value;
            }
        }
        public int HowMany { get; set; }
        public EnumRuleBonus BonusType { get; set; } = EnumRuleBonus.None;
        public override bool IncreaseOne()
        {
            if (HowMany > 0 || Deck == EnumRuleText.HandLimit0)
                return true;
            return base.IncreaseOne();
        }
        public override string Text()
        {

            return Deck.ToString().TextWithSpaces();
        }
        public bool IsLimit() => Category == EnumRuleCategory.Hand || Category == EnumRuleCategory.Keeper;
        public override void Populate(int Chosen)
        {
            Deck = (EnumRuleText)Chosen;
            PopulateDescription();
            switch (Deck)
            {
                case EnumRuleText.BasicRule:
                    HowMany = 1;
                    Category = EnumRuleCategory.Basic;
                    break;
                case EnumRuleText.Play2:
                    HowMany = 2;
                    Category = EnumRuleCategory.Play;
                    break;
                case EnumRuleText.Play3:
                    HowMany = 3;
                    Category = EnumRuleCategory.Play;
                    break;
                case EnumRuleText.Play4:
                    HowMany = 4;
                    Category = EnumRuleCategory.Play;
                    break;
                case EnumRuleText.PlayAll:
                    HowMany = -1; //means unlimited.
                    Category = EnumRuleCategory.Play;
                    break;
                case EnumRuleText.KeeperLimit2:
                    HowMany = 2;
                    Category = EnumRuleCategory.Keeper;
                    break;
                case EnumRuleText.KeeperLimit3:
                    HowMany = 3;
                    Category = EnumRuleCategory.Keeper;
                    break;
                case EnumRuleText.KeeperLimit4:
                    HowMany = 4;
                    Category = EnumRuleCategory.Keeper;
                    break;
                case EnumRuleText.Draw2:
                    HowMany = 2;
                    Category = EnumRuleCategory.Draw;
                    break;
                case EnumRuleText.Draw3:
                    HowMany = 3;
                    Category = EnumRuleCategory.Draw;
                    break;
                case EnumRuleText.Draw4:
                    HowMany = 4;
                    Category = EnumRuleCategory.Draw;
                    break;
                case EnumRuleText.Draw5:
                    HowMany = 5;
                    Category = EnumRuleCategory.Draw;
                    break;
                case EnumRuleText.HandLimit0:
                    HowMany = 0;
                    Category = EnumRuleCategory.Hand;
                    break;
                case EnumRuleText.HandLimit1:
                    HowMany = 1;
                    Category = EnumRuleCategory.Hand;
                    break;
                case EnumRuleText.HandLimit2:
                    HowMany = 2;
                    Category = EnumRuleCategory.Hand;
                    break;
                case EnumRuleText.NoHandBonus:
                    BonusType = EnumRuleBonus.NoHand;
                    HowMany = 3;
                    break;
                case EnumRuleText.PoorBonus:
                    BonusType = EnumRuleBonus.PoorBonus;
                    HowMany = 1;
                    break;
                case EnumRuleText.RichBonus:
                    BonusType = EnumRuleBonus.RichBonus;
                    HowMany = 1;
                    break;

                default:
                    //looks like no way to double check this time.
                    break;
            }
        }
    }
}
