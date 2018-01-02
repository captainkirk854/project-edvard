using GameKey.Binding.Readers;
using Items;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Xml.Linq;
using Utility;

namespace UnitTests.GameKey.Binding.Readers.KeyBindingReaderEliteDangerousTests
{
    /// <summary>
    /// https://blogs.msdn.microsoft.com/jamesnewkirk/2004/06/07/testing-private-methodsmember-variables-should-you-or-shouldnt-you/
    /// </summary>
    [TestClass]
    public class GetBindableActions
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
        public void KeyBindingReaderEliteDangerousTests_GetBindableActions()
        {
            var bindableActions = UsePrivateMethod_Object_GetBindableActions();

            object field01 = bindableActions.Rows[0][0];
            object field02 = bindableActions.Rows[1][1];
            object field03 = bindableActions.Rows[26][1];
            object field04 = bindableActions.Rows[28][4];

            if (field01.ToString() == Application.Name.EliteDangerous.ToString() &&
                field02.ToString() == "MouseReset" &&
                field03.ToString() == "UseBoostJuice" &&
                field04.ToString() == "Keyboard" &&
                1 == 1)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        private DataTable UsePrivateMethod_Object_GetBindableActions()
        {
            // Get value of private/protected field ..
            var bindingsXDocument = (XDocument) privateObject.GetField("bindingsXDocument");

            // Return ..
            return (DataTable) privateObject.Invoke("GetBindableActions", bindingsXDocument);
        }
    }
}
