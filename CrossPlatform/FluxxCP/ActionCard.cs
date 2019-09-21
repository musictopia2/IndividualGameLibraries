using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
namespace FluxxCP
{
    public class ActionCard : FluxxCardInformation
    {
        public ActionCard()
        {
            CardType = EnumCardType.Action;
        }
        public new EnumActionMain Deck
        {
            get
            {
                return (EnumActionMain)base.Deck;
            }
            set
            {
                base.Deck = (int)value;
            }
        }
        public EnumActionScreen Category { get; set; }
        public override string Text() //now text has to be function.
        {
            return Deck switch
            {
                EnumActionMain.Draw2AndUseEm => "Draw 2 And Use 'Em",
                EnumActionMain.Draw3Play2OfThem => "Draw 3, Play 2 Of Them",
                EnumActionMain.DiscardDraw => "Discard & Draw",
                EnumActionMain.LetsSimplify => "Let's Simplify",
                _ => Deck.ToString().TextWithSpaces(),
            };
        }
        public override void Populate(int chosen)
        {
            Deck = (EnumActionMain)chosen;
            PopulateDescription();
            switch (Deck)
            {
                case EnumActionMain.DiscardDraw:
                case EnumActionMain.EmptyTheTrash:
                case EnumActionMain.Jackpot:
                case EnumActionMain.NoLimits:
                case EnumActionMain.RulesReset:
                case EnumActionMain.ScrambleKeepers:
                case EnumActionMain.TakeAnotherTurn:
                    Category = EnumActionScreen.None;
                    break;
                case EnumActionMain.ExchangeKeepers:
                case EnumActionMain.StealAKeeper:
                case EnumActionMain.TrashAKeeper:
                    Category = EnumActionScreen.KeeperScreen;
                    break;
                case EnumActionMain.Taxation:
                    Category = EnumActionScreen.OtherPlayer;
                    break;
                default:
                    Category = EnumActionScreen.ActionScreen;
                    break;
            }
        }
    }
}