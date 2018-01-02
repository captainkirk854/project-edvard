using GameKey.Binding.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Utility;

namespace UnitTests.GameKey.Binding.Readers.KeyBindingReaderVoiceAttackTests
{
    [TestClass]
    public class GetCommandStringsForCommandCategory
    {
        private PrivateObject privateObject;
        private static readonly string FilePath = AppRuntime.SolutionDirectory + "\\TestData\\Unit\\VoiceAttack\\Orion 2.0 Full House.vap";

        [TestInitialize]
        public void Initialize()
        {
            var testedClassInstance = new KeyBindingReaderVoiceAttack(FilePath);
            privateObject = new PrivateObject(testedClassInstance);
        }

        [TestMethod]
        public void KeyBindingReaderVoiceAttackTests_GetCommandStringsForCommandCategory()
        {
            var bindableActions = UsePrivateMethod_Object_GetCommandStringsForCommandCategory();
            Assert.AreEqual(656, bindableActions.Count());
        }

        private IEnumerable<object> UsePrivateMethod_Object_GetCommandStringsForCommandCategory()
        {
            // Get value of private/protected field ..
            var bindingsXDocument = (XDocument)privateObject.GetField("bindingsXDocument");

            // Invoke another private method to get VoiceAttack Command Categories ..
            var allCommandCategories = (IEnumerable<string>)privateObject.Invoke("GetAllCommandCategories", bindingsXDocument);

            // Return ..
            return (IEnumerable<object>)privateObject.Invoke("GetCommandStringsForCommandCategory", bindingsXDocument, allCommandCategories);
        }
    }
}
