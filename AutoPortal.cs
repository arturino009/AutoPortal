using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Nodes;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Vector2 = System.Numerics.Vector2;

namespace AutoPortal;

public class AutoPortal : BaseSettingsPlugin<AutoPortalSettings>
{
    private IngameState InGameState => GameController.IngameState;
    private bool completedMap = false;
    private bool needToOpen = false;
    public override bool Initialise()
    {
        RegisterHotkey(Settings.PortalHotkey);
        return true;
    }

    private static void RegisterHotkey(HotkeyNode hotkey)
    {
        Input.RegisterKey(hotkey);
        hotkey.OnValueChanged += () => { Input.RegisterKey(hotkey); };
    }

    public override void AreaChange(AreaInstance area)
    {
        completedMap = InGameState.Data.ServerData.QuestFlags[QuestFlag.MapComplete];
    }

    public override void Render()
    {
        if (GameController.Area.CurrentArea.IsHideout || GameController.Area.CurrentArea.IsTown ||
                GameController.IngameState.IngameUi.NpcDialog.IsVisible ||
                GameController.IngameState.IngameUi.SellWindow.IsVisible ||
                !GameController.InGame || GameController.IsLoading) return;

        if (!completedMap && 
            Settings.AutoComplete && 
            InGameState.Data.ServerData.QuestFlags[QuestFlag.MapComplete] &&
            InGameState.Data.CurrentWorldArea.IsMapWorlds)
        {
            Input.KeyDown(Keys.I);
            Input.KeyUp(Keys.I);
            completedMap = true;
            needToOpen = true;
        }


        if (Settings.PortalHotkey.PressedOnce() && !InGameState.IngameUi.InventoryPanel.IsVisible)
        {
            Input.KeyDown(Keys.I);
            Input.KeyUp(Keys.I);
            needToOpen = true;
        }

        if(needToOpen && InGameState.IngameUi.InventoryPanel.IsVisible)
        {
            openPortal();
        }
    }

    private void openPortal()
    {
        var inventoryItems = InGameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems;
        var portalScroll = inventoryItems.Where(x => x.Item.Path.Contains("CurrencyPortal")).First();
        if (portalScroll != null)
        {
            var prevMousePos = new Vector2(InGameState.MousePosX, InGameState.MousePosY);
            Input.SetCursorPos(portalScroll.GetClientRect().Center.ToVector2Num());
            Thread.Sleep(20);
            Input.Click(MouseButtons.Right);
            Input.SetCursorPos(prevMousePos);
        }
        Input.KeyDown(Keys.I);
        Input.KeyUp(Keys.I);
        needToOpen = false;
    }
}