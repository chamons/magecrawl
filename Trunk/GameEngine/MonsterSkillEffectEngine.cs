﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Magecrawl.Actors;
using Magecrawl.EngineInterfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class MonsterSkillEffectEngine : IMonsterSkillEngine
    {
        public const int SlingDistance = 4;
        private CoreGameEngine m_engine;

        internal MonsterSkillEffectEngine(CoreGameEngine engine)
        {
            m_engine = engine;
        }

        public bool UseSkill(ICharacterCore invoker, MonsterSkillType skill, Point target)
        {
            return UseSkill((Character)invoker, skill, target);
        }

        internal bool UseSkill(Character invoker, MonsterSkillType skill, Point target)
        {
            // Find the method implementing the skill
            MethodInfo skillMethod = GetType().GetMethod("Handle" + skill.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);

            // And invoke it
            return (bool)skillMethod.Invoke(this, new object[] { invoker, skill, target });
        }

        private bool HandleSlingStone(Character invoker, MonsterSkillType skill, Point target)
        {
            Character targetCharacter = ValidTargetLessThanOrEqualTo(invoker, target, SlingDistance);
            if (targetCharacter == null)
                return false;

            List<Point> targetList = new List<Point>() { target };
            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetList, invoker.Position, invoker.Vision, true);
            CoreGameEngine.Instance.FilterNotVisibleBothWaysFromList(invoker.Position, targetList);

            if (targetList.Count < 1)
                return false;

            CoreGameEngine.Instance.SendTextOutput(String.Format("{0} slings a stone at {1}.", invoker.Name, targetCharacter.Name));           
            CoreGameEngine.Instance.CombatEngine.RangedBoltToLocation(invoker, target, (new DiceRoll(5, 3)).Roll(), null, null);
            
            // Rest to pass a turn
            CoreGameEngine.Instance.Wait(invoker);
            return true;
        }

        private bool HandleFirstAid(Character invoker, MonsterSkillType skill, Point target)
        {
            Character targetCharacter = ValidTargetLessThanOrEqualTo(invoker, target, 1);
            if (targetCharacter == null)
                return false;

            // If we get here, it's a valid first aid. Increase target's HP by amount
            string targetString = targetCharacter == invoker ? "themself" : "the " + targetCharacter.Name;
            CoreGameEngine.Instance.SendTextOutput(String.Format("The {0} applies some fast combat medicine on {1}.", invoker.Name, targetString));
            int amountToHeal = (new DiceRoll(4, 3)).Roll();
            targetCharacter.Heal(amountToHeal, false);
            CoreGameEngine.Instance.Wait(invoker);
            return true;
        }

        private bool HandleDoubleSwing(Character invoker, MonsterSkillType skill, Point target)
        {
            Character targetCharacter = ValidTarget(invoker, target, 1);
            if (targetCharacter == null)
                return false;

            // If we get here, it's a valid double swing. Attack two times in a row.
            CoreGameEngine.Instance.SendTextOutput(String.Format("{0} wildly swings at {1} twice.", invoker.Name, targetCharacter.Name));
            m_engine.Attack(invoker, target);
            m_engine.Attack(invoker, target);

            return true;
        }

        private bool HandleRush(Character invoker, MonsterSkillType skill, Point target)
        {
            Character targetCharacter = ValidTarget(invoker, target, 2);
            if (targetCharacter == null)
                return false;

            // If we get here, it's a valid rush. Move towards target and attack at reduced time cost.
            CoreGameEngine.Instance.SendTextOutput(String.Format("{0} rushes towards {1} and attacks.", invoker.Name, targetCharacter.Name));
            List<Point> pathToPoint = CoreGameEngine.Instance.PathToPoint(invoker, target, false, false, true);
            m_engine.Move(invoker, PointDirectionUtils.ConvertTwoPointsToDirection(invoker.Position, pathToPoint[0]));
            m_engine.Attack(invoker, target);

            return true;
        }

        private Character ValidTarget(Character invoker, Point targetSquare, int requiredDistance)
        {
            // First the distance between us and target must be requiredDistance.
            List<Point> pathToPoint = CoreGameEngine.Instance.PathToPoint(invoker, targetSquare, false, false, true);
            if (pathToPoint.Count != requiredDistance)
                return null;

            return m_engine.CombatEngine.FindTargetAtPosition(targetSquare);            
        }

        private Character ValidTargetLessThanOrEqualTo(Character invoker, Point targetSquare, int requiredDistance)
        {
            List<Point> pathToPoint = CoreGameEngine.Instance.PathToPoint(invoker, targetSquare, false, false, true);
            if (pathToPoint.Count > requiredDistance)
                return null;

            return m_engine.CombatEngine.FindTargetAtPosition(targetSquare);
        }
    }
}
