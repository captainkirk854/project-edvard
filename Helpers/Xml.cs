namespace Helpers
{
    using System.Xml.Linq;

    public static class Xml
    {
        /// <summary>
        /// Extension method to handle missing XML element
        /// </summary>
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
        /// Extension method to handle missing XML attribute
        /// </summary>
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
