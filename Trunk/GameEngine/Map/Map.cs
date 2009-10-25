﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;
using System.IO;

namespace Magecrawl.GameEngine
{
    internal sealed class Map : Interfaces.IMap, IXmlSerializable
    {
        private int m_width;
        private int m_height;
        private MapTile[,] m_map;
        private List<MapObject> m_mapObjects;
        private List<Monster> m_monsterList;

        internal Map()
        {
            m_width = -1;
            m_height = -1;
        }

        internal Map(int width, int height)
        {
            m_mapObjects = new List<MapObject>();
            m_monsterList = new List<Monster>();

            CreateDemoMap();
        }

        internal bool KillMonster(Monster m)
        {
            return m_monsterList.Remove(m);
        }

        public int Width
        {
            get
            {
                return m_width;
            }
        }

        public int Height
        {
            get
            {
                return m_height;
            }
        }

        public IList<IMapObject> MapObjects
        {
            get 
            {
                return m_mapObjects.ConvertAll<IMapObject>(new Converter<MapObject, IMapObject>(delegate(MapObject m) { return m as IMapObject; }));
            }
        }

        public IList<ICharacter> Monsters
        {
            get 
            {
                return m_monsterList.ConvertAll<ICharacter>(new Converter<Monster, ICharacter>(delegate(Monster m) { return m as ICharacter; }));
            }
        }

        public IMapTile this[int width, int height]
        {
            get 
            {
                return m_map[width, height];
            }
        }

        // We can't overload this[], and sometimes we need to set internal attributes :(
        public MapTile GetInternalTile(int width, int height)
        {
            return m_map[width, height];
        }

        private void CreateDemoMap()
        {
            using(StreamReader reader = File.OpenText("map.txt"))
            {              
                string sizeLine = reader.ReadLine();
                string [] sizes = sizeLine.Split(' ');
                m_width = Int32.Parse(sizes[0]);
                m_height = Int32.Parse(sizes[0]);
                m_map = new MapTile[m_width, m_height];

                for (int j = 0; j < m_height; ++j)
                {
                    string tileLine = reader.ReadLine();
                    for (int i = 0; i < m_width; ++i)
                    {
                        m_map[i, j] = new MapTile();
                        if (tileLine[i] != '#')
                            m_map[i, j].Terrain = TerrainType.Floor;
                        switch (tileLine[i])
                        {
                            case ':':
                                m_mapObjects.Add(new MapDoor(new Point(i, j)));
                                break;
                            case 'M':
                                m_monsterList.Add(new Monster(i, j));
                                break;
                        }
                    }
                }
            }
        }

        internal bool IsPointOnMap(Point p)
        {
            return (p.X >= 0) && (p.Y >= 0) && (p.X < Width) && (p.Y < Height);
        }

        #region SaveLoad
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            m_width = reader.ReadElementContentAsInt();
            m_height = reader.ReadElementContentAsInt();
            m_map = new MapTile[m_width, m_height];

            for (int i = 0; i < m_width; ++i)
            {
                for (int j = 0; j < m_height; ++j)
                {
                    m_map[i, j] = new MapTile();
                    m_map[i, j].ReadXml(reader);
                }
            }

            // Read Map Features
            m_mapObjects = new List<MapObject>();

            ReadListFromXMLCore readDel = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                MapObject newObj = MapObject.CreateMapObjectFromTypeString(typeString);
                newObj.ReadXml(reader);
                m_mapObjects.Add(newObj);
            });
            ReadListFromXML(reader, readDel);

            // Read Monstesr
            m_monsterList = new List<Monster>();

            readDel = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                Monster newObj = Monster.CreateMonsterObjectFromTypeString(typeString);
                newObj.ReadXml(reader);
                m_monsterList.Add(newObj);
            });
            ReadListFromXML(reader, readDel);

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Map");
            writer.WriteElementString("Width", m_width.ToString());
            writer.WriteElementString("Height", m_height.ToString());

            for (int i = 0; i < m_width; ++i)
            {
                for (int j = 0; j < m_height; ++j)
                {
                    m_map[i, j].WriteXml(writer);
                }
            }

            WriteListToXML(writer, m_mapObjects, "MapObjects");

            WriteListToXML(writer, m_monsterList, "Monsters");

            writer.WriteEndElement();
        }

        public static void WriteListToXML<T>(XmlWriter writer, List<T> list, string listName) where T : IXmlSerializable
        {
            writer.WriteStartElement(listName + "List");
            writer.WriteElementString("Count", list.Count.ToString());
            for (int i = 0; i < list.Count; ++i)
            {
                IXmlSerializable current = list[i];
                writer.WriteStartElement(string.Format(listName + "{0}", i));
                current.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public delegate void ReadListFromXMLCore(XmlReader reader);

        public static void ReadListFromXML(XmlReader reader, ReadListFromXMLCore del)
        {
            reader.ReadStartElement();

            int mapFeatureListLength = reader.ReadElementContentAsInt();

            for (int i = 0; i < mapFeatureListLength; ++i)
            {
                reader.ReadStartElement();

                del(reader);

                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        #endregion
    }
}
