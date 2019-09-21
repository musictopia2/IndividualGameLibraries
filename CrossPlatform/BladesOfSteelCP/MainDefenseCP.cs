using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BladesOfSteelCP
{
    public class MainDefenseCP : HandViewModel<RegularSimpleCard>
    {
        protected override Task PrivateBoardSingleClickedAsync()
        {
            if (HandList.Count > 0)
                return Task.CompletedTask;
            return base.PrivateBoardSingleClickedAsync();
        }
        protected override void AfterPopulateObjects()
        {
            HandList.ForEach(thisCard =>
            {
                thisCard.Visible = true;
                thisCard.Drew = false;
                thisCard.IsSelected = false;
                thisCard.IsUnknown = false;
            });
        }
        readonly BladesOfSteelMainGameClass _mainGame;
        public MainDefenseCP(IBasicGameVM thisMod) : base(thisMod)
        {
            Maximum = 3;
            Text = "Defense Pile Played";
            AutoSelect = EnumAutoType.None;
            _mainGame = thisMod.MainContainer!.Resolve<BladesOfSteelMainGameClass>();
        }
        public bool HasCards => HandList.Count > 0;
        public bool CanAddDefenseCards(ICustomBasicList<RegularSimpleCard> thisList)
        {
            BladesOfSteelPlayerItem thisPlayer = _mainGame.PlayerList!.GetWhoPlayer();
            var attackStage = _mainGame.GetAttackStage(thisPlayer.AttackList);
            if (attackStage == EnumAttackGroup.GreatOne)
                return false;
            var defenseStage = _mainGame.GetDefenseStage(thisList);
            if (defenseStage == EnumDefenseGroup.StarGoalie)
                return true;
            if ((int)defenseStage > (int)attackStage)
                return true;
            if ((int)attackStage > (int)defenseStage)
                return false;
            int attackPoints = thisPlayer.AttackList.Sum(items => (int)items.Value);
            int defensePoints = thisList.Sum(items => (int)items.Value);
            return defensePoints >= attackPoints;
        }
    }
}