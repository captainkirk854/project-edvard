using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameKey.Adapter;
using GameKey.Binding.Readers;
using Helper;
using Items;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Utility;

namespace UnitTests.GameKey.Binding.Readers.KeyBindingReaderEliteDangerousTests
{
    [TestClass]
    public class GetBindableActions
    {
        private PrivateObject privateObject;
        private static readonly string FilePath = AppRuntime.SolutionDirectory + "\\TestData\\Unit\\EliteDangerous\\GoodKeys.binds";

        [TestInitialize]
        public void Initialize()
        {
            var testedClassInstance = new KeyBindingReaderEliteDangerous(FilePath);
            privateObject = new PrivateObject(testedClassInstance);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var plop = UsePrivateMethod_Object_GetBindableActions();
            var plip = 1;
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
