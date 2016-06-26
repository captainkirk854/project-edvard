namespace Helper
{
    using System.Xml.Linq;

    /// <summary>
    /// XML-related Helper Methods
    /// </summary>
    public static class StockXml
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
        /// <param name="xmlFilePath"></param>
        /// <returns></returns>
        public static XDocument ReadXDoc(string xmlFilePath)
        {
            return XDocument.Load(xmlFilePath);
        }
    }
}
