namespace Helper
{
    using System;
    using System.IO;
    using System.Xml.Linq;

    /// <summary>
    /// XML-related Helper Methods
    /// </summary>
    public static class HandleXml
    {
        /// <summary>
        /// Extension method to provide XML element name
        /// </summary>
        /// <remarks>
        /// Avoids throwing a NullReference exception when XML element does not exist
        /// </remarks>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string SafeElementName(this XElement element)
        {
            if (element != null)
            {
                return element.Name.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Extension method to provide XML element value 
        /// </summary>
        /// <remarks>
        /// Avoids throwing a NullReference exception when XML element does not exist
        /// </remarks>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string SafeElementValue(this XElement element)
        {
            if (element != null)
            {
                return element.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Extension method to provide XML attribute name
        /// </summary>
        /// <remarks>
        /// Avoids throwing a NullReference exception when XML attribute does not exist
        /// </remarks>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string SafeAttributeName(this XElement element, string attributeName)
        {
            if (element == null)
            {
                return string.Empty;
            }
            else
            {
                return attributeName;
            }
        }

        /// <summary>
        /// Extension method to provide XML attribute value
        /// </summary>
        /// <remarks>
        /// Avoids throwing a NullReference exception when XML attribute does not exist
        /// </remarks>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string SafeAttributeValue(this XElement element, string attributeName)
        {
            if (element == null)
            {
                return string.Empty;
            }
            else
            {
                XAttribute attr = element.Attribute(attributeName);
                return attr == null ? string.Empty : attr.Value;
            }
        }

        /// <summary>
        /// Read XML file as XDocument
        /// </summary>
        /// <remarks>
        /// Cannot ensured that XML file is encoded correctly (e.g an erroneous utf-16 reference) ..
        /// > To prevent: System.Xml.XmlException ("There is no Unicode byte order mark. Cannot switch to Unicode")
        /// > Read file into stream
        /// > Let StreamReader detect encoding
        /// > Create XDocument ..
        /// </remarks>
        /// <param name="xmlFilePath"></param>
        /// <returns></returns>
        public static XDocument ReadXDoc(string xmlFilePath)
        {
            using (StreamReader sr = new StreamReader(xmlFilePath, true))
            {
                return XDocument.Load(sr);
            }
        }
    }
}
