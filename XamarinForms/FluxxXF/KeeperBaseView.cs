using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace FluxxXF
{
    public abstract class KeeperBaseView : CustomControlBase
    {
        readonly Grid _grid;
        private readonly FluxxVMData _model;

        public KeeperBaseView(FluxxGameContainer gameContainer,
            KeeperContainer keeperContainer,
            ActionContainer actionContainer,
            FluxxVMData model
            )
        {
            _grid = new Grid();
            GameContainer = gameContainer;
            KeeperContainer = keeperContainer;
            ActionContainer = actionContainer;
            _model = model;
            if (gameContainer.PlayerList.Count() <= 3)
                AddAutoRows(_grid, 3);
            else
                AddAutoRows(_grid, 4);

            if (gameContainer.PlayerList.Count() == 2 || gameContainer.PlayerList.Count() == 4)
            {
                AddLeftOverColumn(_grid, 50);
                AddLeftOverColumn(_grid, 50);
            }
            else
            {
                AddLeftOverColumn(_grid, 33);
                AddLeftOverColumn(_grid, 33);
                AddLeftOverColumn(_grid, 33);
            }
            var list = gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();

            var card = new ShowCardUI(_model, actionContainer, keeperContainer, EnumShowCategory.KeeperScreen);
            card.WidthRequest = 1000;
            AddControlToGrid(_grid, card, 0, 0);
            Grid.SetColumnSpan(card, 3);
            card.HorizontalOptions = LayoutOptions.Start;
            card.VerticalOptions = LayoutOptions.Start;
            //hopefully it works when overrided version runs the methods for button and process.
            Button button;
            if (KeeperCategory == EnumKeeperCategory.Show)
            {
                button = GetGamingButton("Close Keeper Screen", nameof(KeeperShowViewModel.CloseKeeperAsync));
                AddControlToGrid(_grid, button, _grid.RowDefinitions.Count - 1, 0);
            }
            else if (KeeperCategory == EnumKeeperCategory.Process)
            {
                button = GetGamingButton(keeperContainer.ButtonText, CommandText);
                AddControlToGrid(_grid, button, _grid.RowDefinitions.Count - 1, 0);
            }
            else
            {
                throw new BasicBlankException("Keeper category not supported.  Rethink");
            }
            Grid.SetColumnSpan(button, 3);

            int x = 0;
            int row = 0;
            int column = 0;
            foreach (var player in list)
            {
                x += 1;
                if (x == 1 && player.PlayerCategory != EnumPlayerCategory.Self)
                    throw new Exception("Failed to do self first");
                KeeperHandXF hand = new KeeperHandXF();
                var keeper = keeperContainer.GetKeeperHand(player);
                hand.LoadList(keeper, "");
                if (x == 1)
                {
                    row = 1;
                    column = 0;
                }
                else if (x == 2)
                {
                    row = 1;
                    column = 1;
                }
                else if (x == 3 && gameContainer.PlayerList.Count() == 4)
                {
                    row = 2;
                    column = 0;
                }
                else if (x == 3)
                {
                    row = 1;
                    column = 2;
                }
                else if (x == 4 && gameContainer.PlayerList.Count() == 4)
                {
                    row = 2;
                    column = 1;
                }
                else if (x == 4)
                {
                    row = 2;
                    column = 0;
                }
                else if (x == 5)
                {
                    row = 2;
                    column = 1;
                }
                else if (x == 6)
                {
                    row = 2;
                    column = 2;
                }
                hand.MinimumWidthRequest = 300;
                AddControlToGrid(_grid, hand, row, column);
            }

            Content = _grid;

        }
        protected virtual string CommandText => "";

        protected abstract EnumKeeperCategory KeeperCategory { get; }

        protected FluxxGameContainer GameContainer { get; }
        protected KeeperContainer KeeperContainer { get; }
        protected ActionContainer ActionContainer { get; }
    }
}
