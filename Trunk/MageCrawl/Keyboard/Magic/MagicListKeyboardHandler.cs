using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using libtcod;
using Magecrawl.GameUI.ListSelection;
using Magecrawl.GameUI.ListSelection.Requests;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Interfaces;

namespace Magecrawl.Keyboard.Magic
{
    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "SpellList")]
    internal class MagicListKeyboardHandler : InvokingKeystrokeHandler
    {
        // When we're brought up, get the keystroke used to call us, so we can use it to select target(s)
        private NamedKey m_keystroke;

        public override void NowPrimaried(object request)
        {
            m_keystroke = (NamedKey)request;
            m_gameInstance.SendPaintersRequest(new DisableAllOverlays());
            ListItemShouldBeEnabled magicSpellEnabledDelegate = s => m_engine.Player.CouldCastSpell((ISpell)s);
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, m_engine.Player.Spells.OfType<INamedItem>().ToList(), true, "Spellbook", magicSpellEnabledDelegate));
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
                m_gameInstance.SendPaintersRequest(new ListSelectionItemSelectedByChar(keystroke.Character, SpellSelectedDelegate));
            }
        }

        private void SpellSelectedDelegate(INamedItem spellName)
        {
            ISpell spell = (ISpell)spellName;
            if (!m_engine.Player.CouldCastSpell(spell))
                return;

            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false));

            HandleInvoke(spell, spell.Targeting, x => m_engine.Actions.CastSpell(spell, x), m_keystroke);
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(SpellSelectedDelegate));
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
