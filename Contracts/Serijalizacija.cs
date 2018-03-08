using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Contracts
{
    public class Serijalizacija
    {

        public T DeSerializeObject<T>(string fName)
        {
            if (string.IsNullOrEmpty(fName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                string attributeXml = string.Empty;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    //  Type outType = typeof(T);
                    XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(T) })[0];
                    //  XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch
            {

            }

            return objectOut;
        }

        public void SerializeObject<T>(T serializableObject, string fName)
        {
            if (serializableObject == null) { return; }

            try
            {


                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fName);
                    stream.Close();
                }
            }
            catch
            {

            }
        }
    }
}
