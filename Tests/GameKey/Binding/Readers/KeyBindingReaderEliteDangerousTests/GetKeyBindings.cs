using GameKey.Binding.Readers;
using Items;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Xml.Linq;
using Utility;

namespace UnitTests.GameKey.Binding.Readers.KeyBindingReaderEliteDangerousTests
{
    [TestClass]
    public class GetKeyBindings
    {
        private PrivateObject privateObject;
        private static readonly string FilePath = AppRuntime.SolutionDirectory + "\\TestData\\Unit\\EliteDangerous\\1.8.Selection01.binds";

        [TestInitialize]
        public void Initialize()
        {
            var testedClassInstance = new KeyBindingReaderEliteDangerous(FilePath);
            privateObject = new PrivateObject(testedClassInstance);
        }

        [TestMethod]
        public void KeyBindingReaderEliteDangerousTests_GetPrimaryKeyBindings()
        {
            var keyActionDefinitions = UsePrivateMethod_Object_GetPrimaryKeyBindings();

            object field01 = keyActionDefinitions.Rows[0][0];
            object field02 = keyActionDefinitions.Rows[1][1];
            object field03 = keyActionDefinitions.Rows[3][5];
            object field04 = keyActionDefinitions.Rows[3][6];
            object field05 = keyActionDefinitions.Rows[9][10];

            if (field01.ToString() == Application.Name.EliteDangerous.ToString() &&
                field02.ToString() == KeyEnum.Type.WindowsForms.ToString() &&
                field03.ToString() == "R" &&
                field04.ToString() == "82" &&
                field05.ToString() == "RShiftKey" &&
                1 == 1)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void KeyBindingReaderEliteDangerousTests_GetSecondaryKeyBindings()
        {
            var keyActionDefinitions = UsePrivateMethod_Object_GetSecondaryKeyBindings();

            object field01 = keyActionDefinitions.Rows[0][0];
            object field02 = keyActionDefinitions.Rows[0][1];
            object field03 = keyActionDefinitions.Rows[0][3];
            object field04 = keyActionDefinitions.Rows[0][4];

            if (field01.ToString() == Application.Name.EliteDangerous.ToString() &&
                field02.ToString() == KeyEnum.Type.WindowsForms.ToString() &&
                field03.ToString() == Application.EliteDangerousDevicePriority.Secondary.ToString() &&
                field04.ToString() == "LeftAlt" &&
                1 == 1)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        private DataTable UsePrivateMethod_Object_GetPrimaryKeyBindings()
        {
            // Get value of private/protected field ..
            var bindingsXDocument = (XDocument)privateObject.GetField("bindingsXDocument");

            // Return ..
            return (DataTable)privateObject.Invoke("GetKeyBindings", bindingsXDocument, Application.EliteDangerousDevicePriority.Primary);
        }

        private DataTable UsePrivateMethod_Object_GetSecondaryKeyBindings()
        {
            // Get value of private/protected field ..
            var bindingsXDocument = (XDocument)privateObject.GetField("bindingsXDocument");

            // Return ..
            return (DataTable)privateObject.Invoke("GetKeyBindings", bindingsXDocument, Application.EliteDangerousDevicePriority.Secondary);
        }
    }
}
