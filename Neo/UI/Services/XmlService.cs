using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Neo.Crypto;
// ReSharper disable PossibleNullReferenceException
// TODO: Use null propagation with C# 6.0.

namespace Neo.UI.Services
{
    class XmlService
    {
        public static void Initialize()
        {
            if (File.Exists(@".\Config\Connections.xml")) return;

            var doc = new XDocument(new XElement("Connections"));
            doc.Save(@".\Config\Connections.xml");
        }

        public static void SaveConnection(List<string> value)
        {
            var data = new DataProtection();

            try
            {
                var fileDir = XDocument.Load(@".\Config\Connections.xml");

                var newConnection = new XElement("Connection",
                                               new XElement("Address", data.Encrpyt(value[0])),
                                               new XElement("Username", data.Encrpyt(value[1])),
                                               new XElement("Password", data.Encrpyt(value[2])),
                                               new XElement("Database", data.Encrpyt(value[3]))
                                           );

                newConnection.SetAttributeValue("ID", value[4]);
                newConnection.SetAttributeValue("Default", value[5]);

                var xElement = fileDir.Element("Connections");
                if (xElement != null) xElement.Add(newConnection);

                fileDir.Save(@".\Config\Connections.xml");
            }
            catch (Exception e)
            {
                Log.Error("Unable to save connection. " + e.Message);
            }
        }

        public static List<string> GetIdAttributes()
        {
            var connectionList = new List<string>();

            try
            {
                var root = XElement.Load(@".\Config\Connections.xml");
                var connection =
                    from el in root.Elements("Connection")
                    select el;

                connectionList.AddRange(connection.Select(el => el.Attribute("ID").Value));
                return connectionList;
            }
            catch (Exception e)
            {
                Log.Error("Unable to read connection. " + e.Message);
            }
            return null;
        }

        public static List<string> ReadConnection(string id)
        {
            var connectionList = new List<string>();
            var data = new DataProtection();

            try
            {
                var root = XElement.Load(@".\Config\Connections.xml");
                var connection =
                    from el in root.Elements("Connection")
                    where (string) el.Attribute("ID") == id
                    select el;
                foreach (var el in connection)
                {
                    connectionList.Add(data.Decrpyt(el.Element("Address").Value));
                    connectionList.Add(data.Decrpyt(el.Element("Username").Value));
                    connectionList.Add(data.Decrpyt(el.Element("Password").Value));
                    connectionList.Add(data.Decrpyt(el.Element("Database").Value));
                }
                return connectionList;
            }
            catch (Exception e)
            {
                Log.Error("Unable to read connection. " + e.Message);
            }
            return null;
        }

        public static void DeleteConnection(string id)
        {
            try
            {
                var xdoc = XDocument.Load(@".\Config\Connections.xml");
                xdoc.Element("Connections").Elements("Connection")
                    .Where(x => (string) x.Attribute("ID") == id)
                    .Remove();
                xdoc.Save(@".\Config\Connections.xml");
            }
            catch (Exception e)
            {
                Log.Error("Unable to delete connection. " + e.Message);
            }
        }
    }
}
