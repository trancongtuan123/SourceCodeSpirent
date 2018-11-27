using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RxAgent.Util
{
    internal static class StringXmlSerializer
    {
        public static string Serialize<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            string result;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                {
                    XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                    xmlSerializerNamespaces.Add("", "");
                    xmlSerializer.Serialize(xmlTextWriter, obj, xmlSerializerNamespaces);
                    xmlTextWriter.Flush();
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8))
                    {
                        string text = streamReader.ReadToEnd();
                        result = text;
                    }
                }
            }
            return result;
        }

        public static T Deserialize<T>(string ser)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T result;

            using (StringReader stringReader = new StringReader(ser))
            {
                using (XmlTextReader xmlTextReader = new XmlTextReader(stringReader))
                {
                    T t = (T)((object)xmlSerializer.Deserialize(xmlTextReader));
                    result = t;
                }
            }
            return result;
        }
    }
}
