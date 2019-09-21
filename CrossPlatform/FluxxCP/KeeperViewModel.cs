using BasicGameFramework.Attributes;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DrawableListsViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Linq;
namespace FluxxCP
{
    [SingletonGame]
    public class KeeperViewModel : ObservableObject, IKeeper
    {
        private EnumKeeperSection _Section;
        public EnumKeeperSection Section
        {
            get { return _Section; }
            set
            {
                if (SetProperty(ref _Section, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private readonly FluxxViewModel _thisMod;
        public CustomBasicList<HandViewModel<KeeperCard>>? KeeperHandList;
        public DetailCardViewModel? ThisDetail;
        public BasicGameCommand CloseCommand { get; set; }
        public BasicGameCommand ProcessCommand { get; set; }
        private readonly IFluxxEvent _thisEvent;
        private readonly FluxxMainGameClass _mainGame;
        public KeeperViewModel(FluxxViewModel thisMod, FluxxMainGameClass mainGame)
        {
            _thisMod = thisMod;
            _thisEvent = _thisMod;
            _mainGame = mainGame;
            CloseCommand = new BasicGameCommand(thisMod, items =>
            {
                _thisEvent.CloseKeeperScreen();
            }, items =>
            {
                if (thisMod.FluxxScreenUsed != EnumActionScreen.KeeperScreen)
                    return false;
                return Section == EnumKeeperSection.None;
            }, thisMod, thisMod.CommandContainer!);
            ProcessCommand = new BasicGameCommand(thisMod, async items =>
            {
                var thisList = GetSelectList();
                if (thisList.Count == 0)
                {
                    await thisMod.ShowGameMessageAsync("Must choose a keeper");
                    return;
                }
                if (Section == EnumKeeperSection.Exchange)
                {
                    if (thisList.Count != 2)
                    {
                        await thisMod.ShowGameMessageAsync("Must choose a keeper from yourself and from another player for exchange");
                        return;
                    }
                    if (ContainsCurrentPlayer(thisList) == false)
                    {
                        await thisMod.ShowGameMessageAsync("Must choose a keeper from your keeper list in order to exchange");
                        return;
                    }
                    KeeperPlayer keeperFrom = new KeeperPlayer { Player = _mainGame.WhoTurn };
                    var thisHand = GetCurrentPlayerKeeperHand(thisList);
                    keeperFrom.Card = thisHand.ObjectSelected();
                    if (keeperFrom.Card == 0)
                        throw new BasicBlankException("Keeper was never selected for current player");
                    thisList.RemoveSpecificItem(thisHand);
                    KeeperPlayer keeperTo = new KeeperPlayer();
                    keeperTo.Player = GetPlayerOfKeeperHand(thisList.Single());
                    keeperTo.Card = thisList.Single().ObjectSelected();
                    if (keeperTo.Card == 0)
                        throw new BasicBlankException("Keeper was never selected for player");
                    await _thisEvent.KeepersExchangedAsync(keeperFrom, keeperTo);
                    return;
                }
                if (thisList.Count != 1)
                {
                    await thisMod.ShowGameMessageAsync("Must choose only one keeper");
                    return;
                }
                int index = GetPlayerOfKeeperHand(thisList.Single());
                bool isTrashed;
                if (index == mainGame.WhoTurn && Section != EnumKeeperSection.Trash)
                {
                    await thisMod.ShowGameMessageAsync("Cannot steal a keeper from yourself");
                    return;
                }
                isTrashed = Section == EnumKeeperSection.Trash;
                KeeperPlayer tempKeep = new KeeperPlayer();
                tempKeep.Player = index;
                tempKeep.Card = thisList.Single().ObjectSelected();
                if (tempKeep.Card == 0)
                    throw new BasicBlankException("Keeper was never selected");
                await _thisEvent.StealTrashKeeperAsync(tempKeep, isTrashed);
            }, items =>
            {
                if (thisMod.FluxxScreenUsed != EnumActionScreen.KeeperScreen)
                    return false;
                return Section != EnumKeeperSection.None;
            }, thisMod, thisMod.CommandContainer!);
        }
        private CustomBasicList<HandViewModel<KeeperCard>> GetSelectList()
        {
            return KeeperHandList.Where(items => items.ObjectSelected() > 0).ToCustomBasicList();
        }
        private int GetPlayerOfKeeperHand(HandViewModel<KeeperCard> thisHand)
        {
            int index = KeeperHandList!.IndexOf(thisHand);
            return index + 1;
        }
        private bool ContainsCurrentPlayer(CustomBasicList<HandViewModel<KeeperCard>> thisList)
        {
            return thisList.Any(items => GetPlayerOfKeeperHand(items) == _mainGame.WhoTurn);
        }
        private HandViewModel<KeeperCard> GetCurrentPlayerKeeperHand(CustomBasicList<HandViewModel<KeeperCard>> thisList)
        {
            return thisList.Single(items => GetPlayerOfKeeperHand(items) == _mainGame.WhoTurn);
        }
        public HandViewModel<KeeperCard> GetKeeperHand(FluxxPlayerItem thisPlayer)
        {
            foreach (var thisHand in KeeperHandList!)
            {
                if (thisHand.HandList.Equals(thisPlayer.KeeperList))
                    return thisHand;
            }
            throw new BasicBlankException("Keeper Hand Not Found");
        }
        private void LinkKeepers()
        {
            _mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                var thisHand = new HandViewModel<KeeperCard>(_thisMod);
                thisHand.Visible = true;
                thisHand.AutoSelect = HandViewModel<KeeperCard>.EnumAutoType.SelectOneOnly;
                thisHand.HandList = thisPlayer.KeeperList;
                thisHand.Text = thisPlayer.NickName;
                thisHand.SendEnableProcesses(_thisMod, () =>
                {
                    return _thisMod.FluxxScreenUsed == EnumActionScreen.KeeperScreen && Section != EnumKeeperSection.None;
                });
                KeeperHandList!.Add(thisHand);
            });
        }
        public string GetButtonText()
        {
            return Section switch
            {
                EnumKeeperSection.None => "",
                EnumKeeperSection.Trash => "Trash A Keeper",
                EnumKeeperSection.Steal => "Steal A Keeper",
                EnumKeeperSection.Exchange => "Exchange A Keeper",
                _ => throw new BasicBlankException("Nothing Found"),
            };
        }
        public bool EntireVisible => _thisMod.KeeperVisible;
        public void Init()
        {
            KeeperHandList = new CustomBasicList<HandViewModel<KeeperCard>>();
            LinkKeepers();
            ThisDetail = new DetailCardViewModel();
        }
        public void LoadKeeperScreen()
        {
            if (_mainGame.ThisGlobal!.CurrentAction == null)
                throw new BasicBlankException("Must have a current action in order to load the keeper screen.  If a player wants to see the keepers only; use ShowKeepers method.");
            _thisMod.FluxxScreenUsed = EnumActionScreen.KeeperScreen;
            _thisMod.Title = "Keepers"; //hopefully this simple.
            if (_mainGame.ThisGlobal.CurrentAction.Deck == EnumActionMain.TrashAKeeper)
                Section = EnumKeeperSection.Trash;
            else if (_mainGame.ThisGlobal.CurrentAction.Deck == EnumActionMain.StealAKeeper)
                Section = EnumKeeperSection.Steal;
            else if (_mainGame.ThisGlobal.CurrentAction.Deck == EnumActionMain.ExchangeKeepers)
                Section = EnumKeeperSection.Exchange;
            else
                throw new BasicBlankException("Can't figure out the section when loading keepers");
            ThisDetail!.ShowCard(_mainGame.ThisGlobal.CurrentAction);
        }
        public void LoadSavedGame()
        {
            KeeperHandList!.Clear();
            LinkKeepers();
        }
        public void ShowKeepers()
        {
            Section = EnumKeeperSection.None;
            _thisMod.Title = "Keepers"; //hopefully this simple.
        }
        public void ShowSelectedKeepers(CustomBasicList<KeeperPlayer> tempList)
        {
            tempList.ForEach(thisTemp =>
            {
                var thisKeep = KeeperHandList![thisTemp.Player - 1];
                thisKeep.SelectOneFromDeck(thisTemp.Card);
            });
        }
        void IKeeper.VisibleChange()
        {
            OnPropertyChanged(nameof(EntireVisible));
        }
    }
}