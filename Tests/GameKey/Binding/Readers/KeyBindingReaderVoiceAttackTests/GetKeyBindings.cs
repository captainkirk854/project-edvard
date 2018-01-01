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
    public class GetKeyBindings
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
        public void KeyBindingReaderVoiceAttackTests_ConvertPressKeyActionsToKeyActionDefinitionDataTable()
        {
            var keyActionDefinitions = UsePrivateMethod_Object_GetKeyBindings();

            object field01 = keyActionDefinitions.Rows[0][0];
            object field02 = keyActionDefinitions.Rows[1][1];
            object field03 = keyActionDefinitions.Rows[2][7];
            object field04 = keyActionDefinitions.Rows[17][2];
            object field05 = keyActionDefinitions.Rows[17][5];
            object field06 = keyActionDefinitions.Rows[17][6];

            if (field01.ToString() == Application.Name.VoiceAttack.ToString() &&
                field02.ToString() == KeyEnum.Type.WindowsForms.ToString() &&
                field03.ToString() == "PressKey" &&
                field04.ToString() == "((Afterburners))" &&
                field05.ToString() == "Tab" &&
                field06.ToString() == "9" &&
                1 == 1)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        private DataTable UsePrivateMethod_Object_GetKeyBindings()
        {
            // Get value of private/protected field ..
            var bindingsXDocument = (XDocument)privateObject.GetField("bindingsXDocument");

            // Return ..
            return (DataTable)privateObject.Invoke("GetKeyBindings", bindingsXDocument);
        }
    }
}
