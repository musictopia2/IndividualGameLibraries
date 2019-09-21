using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
namespace FluxxCP
{
    [SingletonGame]
    public class GlobalClass
    {
        static internal CustomBasicList<string> DescriptionList = new CustomBasicList<string>();
        private readonly FluxxMainGameClass _mainGame;
        public GlobalClass(FluxxMainGameClass mainGame) //try here now.
        {
            Assembly thisA = Assembly.GetAssembly(GetType());
            string thisStr = thisA.ResourcesAllTextFromFile("FluxxDescriptions.json");
            DescriptionList = JsonConvert.DeserializeObject<CustomBasicList<string>>(thisStr);
            _mainGame = mainGame;
        }
        public DeckRegularDict<FluxxCardInformation> QuePlayList { get; set; } = new DeckRegularDict<FluxxCardInformation>();
        public ActionCard? CurrentAction { get; set; }
        public int IncreaseAmount()
        {
            if (_mainGame.SaveRoot!.RuleList.Any(items => items.EverythingByOne()))
                return 1;
            return 0;
        }
        public void RemoveLimits()
        {
            _mainGame.SaveRoot!.RuleList.RemoveAllAndObtain(items => items.Category == EnumRuleCategory.Hand || items.Category == EnumRuleCategory.Keeper); //hopefully that works too
        }
        public DeckRegularDict<RuleCard> GetLimitList()
        {
            return _mainGame.SaveRoot!.RuleList.Where(items => items.Category == EnumRuleCategory.Hand || items.Category == EnumRuleCategory.Keeper).ToRegularDeckDict();
        }
        public bool HasDoubleAgenda() => _mainGame.SaveRoot!.RuleList.Any(items => items.Deck == EnumRuleText.DoubleAgenda);
        private bool PlayerHasKeepers(bool most)
        {
            CustomBasicList<FluxxPlayerItem> tempList;
            if (most)
                tempList = _mainGame.PlayerList.OrderByDescending(items => items.KeeperList.Count).Take(2).ToCustomBasicList();
            else
                tempList = _mainGame.PlayerList.OrderBy(items => items.KeeperList.Count).Take(2).ToCustomBasicList();
            if (tempList.First().KeeperList.Count == tempList.Last().KeeperList.Count())
                return false;
            int tempTurn = _mainGame.SingleInfo!.Id;
            int index = tempList.First().Id;
            return index == tempTurn;
        }
        public bool HasFewestKeepers() => PlayerHasKeepers(false);
        public bool HasMostKeepers() => PlayerHasKeepers(true);
        public CustomBasicList<string> GetSortedKeeperList()
        {
            CustomBasicList<int> firstList = Enumerable.Range(23, 18).ToCustomBasicList();
            CustomBasicList<string> output = new CustomBasicList<string>();
            firstList.ForEach(thisIndex =>
            {
                EnumKeeper thisKeep = thisIndex.ToEnum<EnumKeeper>();
                output.Add(thisKeep.ToString().TextWithSpaces());
            });
            output.Sort(); //try this too.
            return output;
        }
        public CustomBasicList<int> TempActionHandList
        {
            get
            {
                return _mainGame.SaveRoot!.SavedActionData.TempHandList;
            }
            set
            {
                _mainGame.SaveRoot!.SavedActionData.TempHandList = value;
            }
        }
        public CustomBasicList<PreviousCard> EverybodyGetsOneList
        {
            get
            {
                return _mainGame.SaveRoot!.SavedActionData.PreviousList;
            }
            set
            {
                _mainGame.SaveRoot!.SavedActionData.PreviousList = value;
            }
        }
    }
}