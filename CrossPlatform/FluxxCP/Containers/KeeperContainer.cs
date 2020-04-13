using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Cards;
using FluxxCP.Data;
using FluxxCP.UICP;
using System.Threading.Tasks;

namespace FluxxCP.Containers
{
    [SingletonGame]
    [AutoReset]
    public class KeeperContainer
    {
        private readonly FluxxGameContainer _gameContainer;
        private readonly FluxxDelegates _delegates;
        public CustomBasicList<HandObservable<KeeperCard>> KeeperHandList;
        public DetailCardObservable CardDetail;


        public KeeperContainer(FluxxGameContainer gameContainer, FluxxDelegates delegates)
        {
            _gameContainer = gameContainer;
            _delegates = delegates;
            KeeperHandList = new CustomBasicList<HandObservable<KeeperCard>>();
            CardDetail = new DetailCardObservable();
        }

        public void Init() //still needs this one
        {
            LinkKeepers();
        }
        private void LinkKeepers()
        {
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                var thisHand = new HandObservable<KeeperCard>(_gameContainer.Command);
                thisHand.Visible = true;
                thisHand.AutoSelect = HandObservable<KeeperCard>.EnumAutoType.None;
                thisHand.HandList = thisPlayer.KeeperList;
                thisHand.Text = thisPlayer.NickName;
                //thisHand.ConsiderSelectOneAsync += ThisHand_ConsiderSelectOneAsync;
                thisHand.ObjectClickedAsync += ThisHand_ObjectClickedAsync;
                //something else has to handle the enable processes.

                //thisHand.SendEnableProcesses(this, () =>
                //{
                //    return _thisMod.FluxxScreenUsed == EnumActionScreen.KeeperScreen && Section != EnumKeeperSection.None;
                //});
                KeeperHandList!.Add(thisHand);
            });
        }

        private Task ThisHand_ObjectClickedAsync(KeeperCard payLoad, int index)
        {

            if (Section != EnumKeeperSection.None)
            {
                return Task.CompletedTask;
            }
            if ((int)payLoad.Deck == CardDetail.CurrentCard.Deck)
                CardDetail.ResetCard();
            else
                CardDetail.ShowCard(payLoad);
            return Task.CompletedTask;
        }

        private void ChangeKeeperClick(HandObservable<KeeperCard>.EnumAutoType select)
        {
            KeeperHandList.ForEach(x => x.AutoSelect = select);
        }
        

        public void LoadSavedGame()
        {
            KeeperHandList!.Clear();
            LinkKeepers();
        }
        public EnumKeeperSection Section { get; set; } = EnumKeeperSection.None;
        public void ShowSelectedKeepers(CustomBasicList<KeeperPlayer> tempList)
        {
            tempList.ForEach(thisTemp =>
            {
                var thisKeep = KeeperHandList![thisTemp.Player - 1];
                thisKeep.SelectOneFromDeck(thisTemp.Card);
            });
        }

        public void ShowKeepers()
        {
            Section = EnumKeeperSection.None;
            ChangeKeeperClick(HandObservable<KeeperCard>.EnumAutoType.None);
        }

        public async Task LoadKeeperScreenAsync()
        {
            if (_gameContainer.CurrentAction == null)
                throw new BasicBlankException("Must have a current action in order to load the keeper screen.  If a player wants to see the keepers only; use ShowKeepers method.");
            if (_gameContainer.CurrentAction.Deck == EnumActionMain.TrashAKeeper)
                Section = EnumKeeperSection.Trash;
            else if (_gameContainer.CurrentAction.Deck == EnumActionMain.StealAKeeper)
                Section = EnumKeeperSection.Steal;
            else if (_gameContainer.CurrentAction.Deck == EnumActionMain.ExchangeKeepers)
                Section = EnumKeeperSection.Exchange;
            else
                throw new BasicBlankException("Can't figure out the section when loading keepers");
            CardDetail!.ShowCard(_gameContainer.CurrentAction);
            ChangeKeeperClick(HandObservable<KeeperCard>.EnumAutoType.SelectOneOnly); //hopefully this simple.
            if (_delegates.LoadKeeperScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is loading keeper screen.  Rethink");
            }
            await _delegates.LoadKeeperScreenAsync.Invoke(this);
        }
        public HandObservable<KeeperCard> GetKeeperHand(FluxxPlayerItem thisPlayer)
        {
            foreach (var thisHand in KeeperHandList!)
            {
                if (thisHand.HandList.Equals(thisPlayer.KeeperList))
                    return thisHand;
            }
            throw new BasicBlankException("Keeper Hand Not Found");
        }
        public string ButtonText { get; set; } = "";

    }
}