﻿using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Level;

namespace Magecrawl.GameEngine.SaveLoad
{
    public class SaveLoadCore : IXmlSerializable 
    {
        private const int SaveVersion = 1;

        private bool useSavegameCompression = false;
        private bool permDeath = true;

        internal bool SaveGame(CoreGameEngine engine, string filename)
        {
            if (useSavegameCompression)
                SaveGameCompressed(filename);
            else
                SaveGameXML(filename);
            
            return true;
        }

        internal bool LoadGame(CoreGameEngine engine, string filename)
        {
            if (useSavegameCompression)
                LoadGameCompressed(filename);
            else
                LoadGameXML(filename);
            
            if (permDeath)
                File.Delete(filename); // Woot perm-death
            return true;
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
            string versionString = reader.ReadElementContentAsString();

            if (versionString != SaveVersion.ToString())
                throw new System.InvalidOperationException("Attemping to load bad savefile.");

            Map loadMap = new Map();
             
            reader.ReadStartElement();
            loadMap.ReadXml(reader);
            reader.ReadEndElement();
            
            Player loadPlayer = new Player();
            loadPlayer.ReadXml(reader);

            CoreGameEngine.Instance.SetWithSaveData(loadPlayer, loadMap);

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("MagecrawlSaveFile");
            writer.WriteElementString("Version", SaveVersion.ToString());
          
            writer.WriteStartElement(string.Format("Map{0}", 0));
            (CoreGameEngine.Instance.Map as Map).WriteXml(writer);
            writer.WriteEndElement();

            (CoreGameEngine.Instance.Player as Player).WriteXml(writer);

            writer.WriteEndElement();
        }

        #endregion

        private void SaveGameXML(string fileName)
        {
            XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
            TextWriter w = new StreamWriter(fileName);
            s.Serialize(w, this);
            w.Close();
        }

        private void SaveGameCompressed(string fileName)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                TextWriter memoryWriter = new StreamWriter(memoryStream);
                XmlSerializer xmlSerial = new XmlSerializer(typeof(SaveLoadCore));
                xmlSerial.Serialize(memoryWriter, this);
                memoryWriter.Close();

                FileStream outputFile = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                GZipStream compressedStream = new GZipStream(outputFile, CompressionMode.Compress, true);

                byte[] byteArray = memoryStream.ToArray();

                compressedStream.Write(byteArray, 0, byteArray.Length);

                compressedStream.Close();
                outputFile.Close();
                memoryWriter.Close();
            }
        }

        private void LoadGameXML(string fileName)
        {
            XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
            TextReader r = new StreamReader(fileName);
            s.Deserialize(r);
            r.Close();
        }

        private void LoadGameCompressed(string fileName)
        {
            using (FileStream inputFile = new FileStream(fileName, FileMode.Open))
            {
                using (MemoryStream uncompressedStream = new MemoryStream())
                {
                    using (GZipStream uncompresser = new GZipStream(inputFile, CompressionMode.Decompress))
                    {
                        while (true)
                        {
                            byte[] buffer = new byte[4096];
                            int count = uncompresser.Read(buffer, 0, 4096);
                            if (count > 0)
                                uncompressedStream.Write(buffer, 0, count);
                            else
                                break;
                        }
                    }
                    uncompressedStream.Seek(0, SeekOrigin.Begin);
                    XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
                    TextReader r = new StreamReader(uncompressedStream);
                    s.Deserialize(r);
                    r.Close();
                }
            }
        }
    }
}
