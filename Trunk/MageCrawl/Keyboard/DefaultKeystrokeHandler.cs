﻿using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class DefaultKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        public DefaultKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_keyMappings = null;
        }

        #region Mappable key commands

        /*
         * BCL: see file MageCrawl/dist/KeyMappings.xml. To add a new mappable action, define a private method for it here,
         * then map it to an unused key in KeyMappings.xml. The action should take no parameters and should return nothing.
         */

        private void North()
        {
            m_engine.MovePlayer(Direction.North);
            m_gameInstance.UpdatePainters();
        }

        private void South()
        {
            m_engine.MovePlayer(Direction.South);
            m_gameInstance.UpdatePainters();
        }

        private void East()
        {
            m_engine.MovePlayer(Direction.East);
            m_gameInstance.UpdatePainters();
        }

        private void West()
        {
            m_engine.MovePlayer(Direction.West);
            m_gameInstance.UpdatePainters();
        }

        private void Northeast()
        {
            m_engine.MovePlayer(Direction.Northeast);
            m_gameInstance.UpdatePainters();
        }

        private void Northwest()
        {
            m_engine.MovePlayer(Direction.Northwest);
            m_gameInstance.UpdatePainters();
        }

        private void Southeast()
        {
            m_engine.MovePlayer(Direction.Southeast);
            m_gameInstance.UpdatePainters();
        }

        private void Southwest()
        {
            m_engine.MovePlayer(Direction.Southwest);
            m_gameInstance.UpdatePainters();
        }

        private void Quit()
        {
            m_gameInstance.IsQuitting = true;
        }

        private void Operate()
        {
            m_gameInstance.SetHandlerName("Operate");
        }

        private void GetItem()
        {
            m_engine.PlayerGetItem();
            m_gameInstance.UpdatePainters();
        }

        private void Save()
        {
            m_engine.Save();
            m_gameInstance.UpdatePainters();
        }

        private void Load()
        {
            try
            {
                m_engine.Load();
                m_gameInstance.UpdatePainters();
            }
            catch (System.IO.FileNotFoundException)
            {
                // TODO: Inform user somehow
                m_gameInstance.UpdatePainters();
            }
        }

        private void MoveableOnOff()
        {
            m_gameInstance.SendPaintersRequest("DebuggingMoveableOnOff", m_engine);
            m_gameInstance.UpdatePainters();
        }

        private void DebuggingFOVOnOff()
        {
            m_gameInstance.SendPaintersRequest("DebuggingFOVOnOff", m_engine);
            m_gameInstance.UpdatePainters();
        }

        private void FOVOnOff()
        {
            m_gameInstance.SendPaintersRequest("SwapFOVEnabledStatus");
            m_gameInstance.UpdatePainters();
        }

        private void Wait()
        {
            m_engine.PlayerWait();
            m_gameInstance.UpdatePainters();
        }

        private NamedKey GetNamedKeyForMethodInfo(MethodInfo info)
        {
            foreach(NamedKey key in m_keyMappings.Keys)
            {
                if (m_keyMappings[key] == info)
                    return key;
            }
            throw new System.ArgumentException("GetNamedKeyForMethodInfo - Can't find NamedKey for method?");
        }

        private void Attack()
        {
            List<EffectivePoint> targetPoints = m_engine.Player.CurrentWeapon.CalculateTargetablePoints();
            OnTargetSelection attackDelegate = new OnTargetSelection(OnAttack);
            NamedKey attackKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_gameInstance.SetHandlerName("Target", targetPoints, attackDelegate, attackKey);
        }

        private void OnAttack(Point selection)
        {
            if (selection != m_engine.Player.Position)
                m_engine.PlayerAttack(selection);
        }

        private void ViewMode()
        {
            m_gameInstance.SetHandlerName("Viewmode");
        }

        private void Inventory()
        {
            m_gameInstance.SetHandlerName("Inventory");
        }

        private void TextBoxPageUp()
        {
            m_gameInstance.TextBox.TextBoxScrollUp();
        }

        private void TextBoxPageDown()
        {
            m_gameInstance.TextBox.TextBoxScrollDown();
        }

        private void TextBoxClear()
        {
            m_gameInstance.TextBox.Clear();
        }

        private void HealSpell()
        {
            if (m_engine.PlayerCouldCastSpell("Heal"))
            {
                m_engine.PlayerCastSpell("Heal", Point.Invalid);
                m_gameInstance.UpdatePainters();
            }
        }

        private void BlastSpell()
        {
            if (m_engine.PlayerCouldCastSpell("Blast"))
            {
                m_engine.PlayerCastSpell("Blast", Point.Invalid);
                m_gameInstance.UpdatePainters();
            }
        }

        private void ZapSpell()
        {
            if (m_engine.PlayerCouldCastSpell("Zap"))
            {
                // We should ask engine if we need a target, but right now I 'know' we do
                // We should get targetting distance from engine as well
                List<EffectivePoint> targetablePoints = PointListUtils.PointListFromBurstPosition(m_engine.Player.Position, 5);
                m_engine.FilterNotTargetablePointsFromList(targetablePoints);
                NamedKey zapKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
                OnTargetSelection selectionDelegate = new OnTargetSelection(s =>
                        {
                            m_engine.PlayerCastSpell("Zap", s);
                            m_gameInstance.UpdatePainters();
                        });
                m_gameInstance.SetHandlerName("Target", targetablePoints, selectionDelegate, zapKey);
            }
        }
        
        private void Escape()
        {
        }

        private void Select()
        {
        }

        #endregion
    }
}
