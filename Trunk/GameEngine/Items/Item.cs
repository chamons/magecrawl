﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Items
{
    internal class Item : IItem, IXmlSerializable
    {
        private Point m_position;

        internal Item()
        {
            m_position = Point.Invalid;
        }

        internal Item(int x, int y)
        {
            m_position = new Point(x, y);
        }

        public Point Position
        {
            get
            {
                return m_position;
            }
            internal set
            {
                m_position = value;
            }
        }

        public string Name
        {
            get
            {
                return "Test Item";
            }
        }

        public string ItemDescription
        {
            get 
            {
                return "Item Description\n\n";
            }
        }

        public string FlavorDescription
        {
            get 
            {
                return "Item Flavor Description\n";
            }
        }

        internal virtual List<ItemOptions> PlayerOptions
        {
            get
            {
                return new List<ItemOptions>() { new ItemOptions("Drop", true) };
            }
        }

        #region SaveLoad

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            m_position = m_position.ReadXml(reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", "Item");
            m_position.WriteToXml(writer, "Position");
        }

        internal static Item CreateItemObjectFromTypeString(string s)
        {
            switch (s)
            {
                case "Item":
                    return new Item();
                default:
                    throw new System.ArgumentException("Invalid type in CreateItemObjectFromTypeString");
            }
        }

        #endregion
    }
}
