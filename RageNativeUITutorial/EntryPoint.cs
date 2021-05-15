using System.Collections.Generic;
using System.Windows.Forms;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

[assembly:
    Rage.Attributes.Plugin("Assassination Plugin", Description = "Simple assassination plugin.", Author = "Pranav Rao")]

namespace RageNativeUITutorial
{
    public class EntryPoint
    {
        private static UIMenu mainMenu;
        private static UIMenu vehicleSelectorMenu;
        private static MenuPool _menuPool;
        private static UIMenuItem navigateToSelectorMenuItem;
        private static UIMenuListItem modelList;
        private static UIMenuCheckboxItem invincibleCheckboxItem;
        private static UIMenuListItem directionListItem;
        private static UIMenuItem confirmItem;
        
        public static void Main()
        {
            _menuPool = new MenuPool();

            mainMenu = new UIMenu("Main Menu", "");
            _menuPool.Add(mainMenu);

            vehicleSelectorMenu = new UIMenu("Vehicle Selector Menu", "Select your vehicle");
            vehicleSelectorMenu.SetMenuWidthOffset(30);
            _menuPool.Add(vehicleSelectorMenu);

            mainMenu.AddItem(navigateToSelectorMenuItem = new UIMenuItem("Vehicle Selector Menu"));
            mainMenu.BindMenuToItem(vehicleSelectorMenu, navigateToSelectorMenuItem);
            vehicleSelectorMenu.ParentMenu = mainMenu;

            List<dynamic> listWithModels = new List<dynamic>()
            {
                "POLICE", "POLICE2", "POLICE3"
            };

            modelList = new UIMenuListItem("Model", listWithModels, 0);

            invincibleCheckboxItem = new UIMenuCheckboxItem("Invincible", false);
            mainMenu.AddItem(invincibleCheckboxItem);

            List<dynamic> directions = new List<dynamic>()
            {
                "Front",
                "Back"
            };

            directionListItem = new UIMenuListItem("Direction", directions, 0);
            mainMenu.AddItem(directionListItem);
            
            mainMenu.AddItem(confirmItem = new UIMenuItem("Confirm"));
            
            mainMenu.RefreshIndex();
            vehicleSelectorMenu.RefreshIndex();
            mainMenu.OnItemSelect += OnItemSelect;
            mainMenu.MouseControlsEnabled = false;
            mainMenu.AllowCameraMovement = true;
            vehicleSelectorMenu.MouseControlsEnabled = false;
            vehicleSelectorMenu.AllowCameraMovement = true;
            
            MainLogic();
            GameFiber.Hibernate();
        }

        public static void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender == mainMenu)
            {
                if (selectedItem == confirmItem)
                {
                    Model vehicleModel = new Model(modelList.IndexToItem(modelList.Index));

                    bool invincible = invincibleCheckboxItem.Checked;

                    string directionName = directionListItem.IndexToItem(directionListItem.Index);
                    Vector3 Position;
                    
                    if (directionName == "Front")
                    {
                        Position = Game.LocalPlayer.Character.GetOffsetPositionFront(5);
                    }
                    else
                    {
                        Position = Game.LocalPlayer.Character.GetOffsetPositionFront(-5);
                    }

                    Vehicle newVehicle = new Vehicle(vehicleModel, Position, 0f);
                    newVehicle.IsInvincible = invincible;
                }
            }
        }

        public static void MainLogic()
        {
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    GameFiber.Yield();
                    if (Game.IsKeyDown(Keys.F5))
                    {
                        mainMenu.Visible = !mainMenu.Visible;
                    }
                    
                    _menuPool.ProcessMenus();
                }
            });
        }
    }
}