﻿using System;
using System.Globalization;
using System.Xml;

namespace Magecrawl.Utilities
{
    public struct DiceRoll
    {
        private static Random random = new Random();
        public static DiceRoll Invalid = new DiceRoll(-1, -1, -1, -1.0);
        public static DiceRoll Zero = new DiceRoll(0, 0, 0, 1);

        public short Rolls;
        public short DiceFaces;
        public double Multiplier;
        public short ToAdd;

        public DiceRoll(int rolls, int diceFaces)
            : this((short)rolls, (short)diceFaces, (short)0, (short)1)
        {
        }

        public DiceRoll(int rolls, int diceFaces, int toAdd)
            : this((short)rolls, (short)diceFaces, (short)toAdd, (short)1)
        {
        }

        public DiceRoll(int rolls, int diceFaces, int toAdd, double multiplier)
            : this((short)rolls, (short)diceFaces, (short)toAdd, multiplier)
        {
        }

        public DiceRoll(short rolls, short diceFaces)
            : this(rolls, diceFaces, (short)0, (short)1)
        {
        }

        public DiceRoll(short rolls, short diceFaces, short toAdd)
            : this(rolls, diceFaces, (short)toAdd, (short)1)
        {
        }

        public DiceRoll(string s)
        {
            if (s == "0")
            {
                Rolls = 0;
                DiceFaces = 0;
                Multiplier = 1;
                ToAdd = 0;
                return;
            }

            string[] damageParts = s.Split(',');

            Rolls = short.Parse(damageParts[0], CultureInfo.InvariantCulture);
            DiceFaces = short.Parse(damageParts[1], CultureInfo.InvariantCulture);
            
            if (damageParts.Length > 2)
                ToAdd = short.Parse(damageParts[2], CultureInfo.InvariantCulture);
            else
                ToAdd = 0;

            if (damageParts.Length > 3)
                Multiplier = double.Parse(damageParts[3], CultureInfo.InvariantCulture);
            else
                Multiplier = 1;
        }

        public DiceRoll(short rolls, short diceFaces, short toAdd, double multiplier)
        {
            Rolls = rolls;
            DiceFaces = diceFaces;
            ToAdd = toAdd;
            Multiplier = multiplier;
        }
        
        public void Add(DiceRoll other)
        {
            // If the other is zero, nothing to do.
            if (other.Equals(Zero))
                return;

            // If we're zero, take their values
            if (this.Equals(Zero))
            {
                Rolls = other.Rolls;
                DiceFaces = other.DiceFaces;
                Multiplier = other.Multiplier;
                ToAdd = other.ToAdd;
                return;
            }

            // If neither are zero, the d'ness better match
            if (DiceFaces != other.DiceFaces)
                throw new InvalidOperationException(string.Format("Can't add dice rolls: {0} + {1}", this, other));

            Rolls += other.Rolls;
            ToAdd += other.ToAdd;
            Multiplier += 1 - other.Multiplier;
        }

        public bool Equals(DiceRoll other)
        {
            return Rolls == other.Rolls && DiceFaces == other.DiceFaces && ToAdd == other.ToAdd && Multiplier == other.Multiplier;
        }

        public short Roll()
        {
            short total = 0;
            for (short i = 0; i < Rolls; i++)
            {
                total += (short)(random.Next(DiceFaces) + 1);
            }
            return (short)(Multiplier * (total + ToAdd));
        }

        public int RollMaxDamage()
        {
            return (int)Math.Round((double)(Multiplier * (DiceFaces * Rolls)) + ToAdd);
        }

        public override string ToString()
        {
            bool hasMult = Multiplier != 1;
            bool hasConstant = ToAdd != 0;

            string multiplierFrontString = hasMult ? Multiplier.ToString() + "*" : "";
            string frontParen = hasMult || hasConstant ? "(" : "";
            string endParen = hasMult || hasConstant ? ")" : "";
            string constantSign = ToAdd >= 0 ? "+" : "";    // Minus will come with number
            string constantEnd = hasConstant ? constantSign + ToAdd.ToString() : "";

            // 1d1 doesn't look nice
            if (!hasMult && !hasConstant && Rolls == 1 && DiceFaces == 1)
                return "1";

            return string.Format("{0}{1}{2}d{3}{4}{5}", multiplierFrontString, frontParen, Rolls.ToString(), DiceFaces.ToString(), constantEnd, endParen);
        }

        #region SaveLoad

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            reader.ReadStartElement();
            Rolls = (short)reader.ReadContentAsInt();
            reader.ReadEndElement();

            reader.ReadStartElement();
            DiceFaces = (short)reader.ReadContentAsInt();
            reader.ReadEndElement();

            reader.ReadStartElement();
            Multiplier = (short)reader.ReadContentAsDouble();
            reader.ReadEndElement();

            reader.ReadStartElement();
            ToAdd = (short)reader.ReadContentAsInt();
            reader.ReadEndElement();

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("DamageRoll");
            writer.WriteElementString("Rolls", Rolls.ToString());
            writer.WriteElementString("NumberFaces", DiceFaces.ToString());
            writer.WriteElementString("Multiplier", Multiplier.ToString());
            writer.WriteElementString("AddSub", ToAdd.ToString());
            writer.WriteEndElement();
        }

        #endregion
    }
}
