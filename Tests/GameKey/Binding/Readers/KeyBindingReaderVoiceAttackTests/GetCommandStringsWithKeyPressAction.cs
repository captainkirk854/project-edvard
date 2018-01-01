using GameKey.Adapter;
using GameKey.Binding.Readers;
using Helper;
using Items;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Utility;

namespace UnitTests.GameKey.Binding.Readers.KeyBindingReaderVoiceAttackTests
{
    [TestClass]
    public class GetCommandStringsWithKeyPressAction
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
        public void KeyBindingReaderVoiceAttackTests_ConvertPressKeyActionsToBindableActionsDataTable()
        {
            var bindableActions = UsePrivateMethod_Object_GetCommandStringsWithKeyPressAction();

            object field01 = bindableActions.Rows[0][0];
            object field02 = bindableActions.Rows[1][1];
            object field03 = bindableActions.Rows[2][2];

            if (field01.ToString() == Application.Name.VoiceAttack.ToString() &&
                field02.ToString() == "((10%))" &&
                field03.ToString() == "PressKey" &&
                1 == 1)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        private DataTable UsePrivateMethod_Object_GetCommandStringsWithKeyPressAction()
        {
            // Get value of private/protected field ..
            var bindingsXDocument = (XDocument)privateObject.GetField("bindingsXDocument");

            // Return ..
            return (DataTable)privateObject.Invoke("GetCommandStringsWithKeyPressAction", bindingsXDocument);
        }
    }
}
