using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using libtcod;
using Magecrawl.GameUI.Inventory.Requests;
using Magecrawl.GameUI.ListSelection.Requests;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Interfaces;

namespace Magecrawl.Keyboard.Inventory
{
    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "Inventory")]
    internal class InventoryScreenKeyboardHandler : BaseKeystrokeHandler
    {
        public override void NowPrimaried(object request)
        {
            if (request != null && ((bool)request) == true)
                m_gameInstance.SendPaintersRequest(new SaveListSelectionPosition());
            m_gameInstance.SendPaintersRequest(new DisableAllOverlays());
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, m_engine.Player.Items.OfType<INamedItem>().ToList(), true, "Inventory"));
            m_gameInstance.UpdatePainters();
        }

        public override void HandleKeystroke(NamedKey keystroke)
        {
            MethodInfo action;
            m_keyMappings.TryGetValue(keystroke, out action);
            if (action != null)
            {
                action.Invoke(this, null);
            }
            else if (keystroke.Code == TCODKeyCode.Char)
            {
                m_gameInstance.SendPaintersRequest(new ListSelectionItemSelectedByChar(keystroke.Character, ItemSelectedDelegate));
            }
        }

        private void ItemSelectedDelegate(INamedItem item)
        {
            if (item == null)
                return;

            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false));
            m_gameInstance.SetHandlerName("InventoryItem", "Inventory");

            List<ItemOptions> optionList = m_engine.GameState.GetOptionsForInventoryItem((IItem)item);
            m_gameInstance.SendPaintersRequest(new ShowInventoryItemWindow(true, (IItem)item, optionList));
            m_gameInstance.UpdatePainters();
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(ItemSelectedDelegate));          
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void HandleDirection(Direction direction)
        {
            m_gameInstance.SendPaintersRequest(new ChangeListSelectionPosition(direction));
            m_gameInstance.UpdatePainters();
        }

        private void North()
        {
            HandleDirection(Direction.North);
        }

        private void South()
        {
            HandleDirection(Direction.South);
        }
    }
}
