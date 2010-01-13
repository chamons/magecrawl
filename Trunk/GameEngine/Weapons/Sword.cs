﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class Sword : WeaponBase, Item
    {
        public Sword(string name, DiceRoll damage, double ctCost, string description, string flavorText)
        {
            m_itemDescription = description;
            m_flavorText = flavorText;
            Owner = null;
            m_name = name;
            m_damage = damage;
            m_ctCostToAttack = ctCost;
        }

        public override List<EffectivePoint> CalculateTargetablePoints()
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();

            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(0, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(0, -1), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(1, 1), .75f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(-1, -1), .75f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(-1, 1), .75f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(1, -1), .75f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, Owner.Position, Owner.Vision, true);

            return targetablePoints;
        }

        public override string AttackVerb
        {
            get
            {
                return "slashes";
            }
        }
    }
}
