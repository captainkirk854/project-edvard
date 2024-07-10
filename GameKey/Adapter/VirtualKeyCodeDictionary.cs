namespace GameKey.Adapter
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Input;
    using Utility;

    /// <summary>
    /// Key Type/Code Mapping using Virtual Key Codes ..
    /// </summary>
    public static class VirtualKeyCodeDictionary
    {
        /// <summary>
        /// Enumeration for available keyboard mapping types
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646307(v=vs.85).aspx
        /// </remarks>
        private enum MapType : uint
        {
            /// <summary>
            /// parameter is a virtual-key code and is translated into a scan code.
            /// </summary>
            MAPVK_VK_TO_VSC = 0x0,

            /// <summary>
            /// parameter is a scan code and is translated into a virtual-key code that does not distinguish between left- and right-hand keys
            /// </summary>
            MAPVK_VSC_TO_VK = 0x1,
            
            /// <summary>
            /// parameter is a virtual-key code and is translated into an unshifted character value
            /// </summary>
            MAPVK_VK_TO_CHAR = 0x2,

            /// <summary>
            /// Parameter is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand keys.
            /// </summary>
            MAPVK_VSC_TO_VK_EX = 0x3,

            /// <summary>
            /// Parameter is a virtual-key code and is translated into a scan code
            /// </summary>
            MAPVK_VK_TO_VSC_EX = 0x4
        }

        /// <summary>
        /// Get Unicode Key Type from Windows Forms Keys Enum value with (optional) modifier option(s)
        /// </summary>
        /// <remarks>
        /// A slightly different, and actually much simpler method than the methods beneath, of obtaining a key value from its enum
        /// </remarks>
        /// <param name="windowsFormsKeysEnum"></param>
        /// <param name="shift"></param>
        /// <param name="caps"></param>
        /// <param name="altGr"></param>
        /// <returns></returns>
        public static string GetUnicodeValueFromWindowsInputKeyEnumValueWithOptionalModifiers(string windowsFormsKeysEnum, bool shift, bool caps, bool altGr)
        {
            // Initialise ..
            var outputBuffer = new StringBuilder(256);
            var keyboardState = new byte[256];
            byte keyPressed = 0xff;

            //Parse input string as a System.Windows.Forms.Keys enum ..
            System.Windows.Forms.Keys enumKey = HandleStrings.ParseStringToEnum<System.Windows.Forms.Keys>(windowsFormsKeysEnum);

            // Set keyboard state(s) with optional modifiers ..
            if (shift)
            {
                keyboardState[(int)System.Windows.Forms.Keys.ShiftKey] = keyPressed;
            }

            if (caps)
            {
                keyboardState[(int)System.Windows.Forms.Keys.CapsLock] = keyPressed;
            }

            if (altGr)
            {
                keyboardState[(int)System.Windows.Forms.Keys.ControlKey] = keyPressed;
                keyboardState[(int)System.Windows.Forms.Keys.Menu] = keyPressed;
            }

            // Translate key code coupled with keyboard state to corresponding unicode character(s) ..
            ToUnicode((uint)enumKey, 0, keyboardState, outputBuffer, outputBuffer.Capacity, 0);
            
            // Return ..
            return outputBuffer.ToString();
        }

        /// <summary>
        /// Get Unicode Key Type from Windows Input Key Enum value
        /// </summary>
        /// <param name="windowsInputKeyEnum"></param>
        /// <returns></returns>
        public static string GetUnicodeValueFromWindowsInputKeyEnumValue(string windowsInputKeyEnum)
        {
            //Parse input string as System.Windows.Input.Key enum ..
            System.Windows.Input.Key enumKey = HandleStrings.ParseStringToEnum<Key>(windowsInputKeyEnum);

            return GetUnicodeValueFromWindowsInputKeyEnum(enumKey);
        }

        /// <summary>
        /// Get Unicode Key Type from Windows Forms Keys Enum value
        /// </summary>
        /// <remarks>
        /// (see above)
        ///  Windows Forms Keys enumeration has same integer values as Win 32 virtual key codes.
        ///  https://msdn.microsoft.com/en-us/library/system.windows.input.keyinterop.aspx
        /// </remarks>
        /// <param name="windowsFormsKeysEnum"></param>
        /// <returns></returns>
        public static string GetUnicodeValueFromWindowsFormsKeysEnumValue(string windowsFormsKeysEnum)
        {
            //Parse input string as a System.Windows.Forms.Keys enum ..
            System.Windows.Forms.Keys enumKey = HandleStrings.ParseStringToEnum<System.Windows.Forms.Keys>(windowsFormsKeysEnum);

            // Find its Windows Input key equivalent ..
            System.Windows.Input.Key inputKey = KeyInterop.KeyFromVirtualKey((int)enumKey);

            return GetUnicodeValueFromWindowsInputKeyEnum(inputKey);
        }

        /// <summary>
        /// Get Unicode Key Type from its Windows Input Key Enum
        /// </summary>
        /// <remarks>
        /// Taken from: 
        ///  > http://stackoverflow.com/questions/5825820/how-to-capture-the-character-on-different-locale-keyboards-in-wpf-c
        ///  > http://web.archive.org/web/20111229040043/http://huddledmasses.org/how-to-get-the-character-and-virtualkey-from-a-wpf-keydown-event
        ///  > Dependencies:
        ///    o using System.Windows.Input;
        ///    o Reference: WindowsBase (user32.lib)
        /// </remarks>
        /// <param name="inputKey"></param>
        /// <returns></returns>
        private static string GetUnicodeValueFromWindowsInputKeyEnum(System.Windows.Input.Key inputKey)
        {
            // Initialise ..
            string keyUnicodeValue = "\0";
            StringBuilder stringBuilder = new StringBuilder(2);
            uint scanCode = 0;
            int result = 0;

            // Get keyboard state .. (detect any modifier keys, caps lock, depressed key, etc)
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            // Find its virtual key code ..
            int virtualKey = KeyInterop.VirtualKeyFromKey(inputKey);

            // Translate virtual key code to scan code ..
            scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC_EX);

            // Translate virtual key code coupled with keyboard state to corresponding unicode character(s) ..
            result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);

            // Handle non-printable keys ..
            if (stringBuilder.Length > 0)
            {
                keyUnicodeValue = stringBuilder[0].ToString();
                if (keyUnicodeValue.Trim().Length == 0)
                {
                    keyUnicodeValue = inputKey.ToString();
                }
            }

            // Handle return (ToUnicode result can be: -1, 0, 1) ..
            if (result == 1)
            {
                return keyUnicodeValue;
            }
            else
            {
                return inputKey.ToString();
            }
        }

        /// <summary>
        /// Creates a language identifier from a primary language identifier and a sublanguage identifier.
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/dd373908(v=vs.85).aspx
        /// ref: https://msdn.microsoft.com/en-us/library/dd318693(v=vs.85).aspx
        /// </remarks>
        /// <param name="primaryLanguage"></param>
        /// <param name="subLanguage"></param>
        /// <returns></returns>
        private static ushort MakeLangId(ushort primaryLanguage, ushort subLanguage)
        {
            return Convert.ToUInt16((subLanguage << 10) | primaryLanguage);
        }

        /// <summary>
        /// Loads a new input locale identifier (formerly called the keyboard layout) into the system.
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646305(v=vs.85).aspx
        /// 
        /// Examples: 
        ///  o LoadKeyboardLayout("00000429", 1);
        ///  o LoadKeyboardLayout(InputLanguage.FromCulture(new System.Globalization.CultureInfo("fa-IR"))); //switch to Persian(IR) language 
        ///  o LoadKeyboardLayout(InputLanguage.FromCulture(new System.Globalization.CultureInfo("En-US"))); //switch to English(US) language
        /// </remarks>
        /// <param name="pwszKLID">locale identifier</param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint flags);

        /// <summary>
        /// Translates the specified virtual-key code and keyboard state to the corresponding Unicode character or characters.
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/ms646322(VS.85).aspx
        /// </remarks>
        /// <param name="virtualKeyCodeForTranslate"></param>
        /// <param name="hardwareScanCodeOfKeyForTranslate"></param>
        /// <param name="currentKeyboardState"></param>
        /// <param name="receiveBuffer"></param>
        /// <param name="sizeOfReceiveBuffer"></param>
        /// <param name="resultFlags"></param>
        /// <param name="dwhkl"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int ToUnicodeEx(uint virtualKeyCodeForTranslate,
                                              uint hardwareScanCodeOfKeyForTranslate,
                                              byte[] currentKeyboardState,
                                              [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
                                              StringBuilder receiveBuffer,
                                              int sizeOfReceiveBuffer,
                                              uint resultFlags,
                                              IntPtr dwhkl);

        /// <summary>
        /// Translates the specified virtual-key code and keyboard state to the corresponding Unicode character or characters.
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646320(v=vs.85).aspx
        /// </remarks>
        /// <param name="virtualKeyCodeForTranslate"></param>
        /// <param name="hardwareScanCodeOfKeyForTranslate"></param>
        /// <param name="currentKeyboardState"></param>
        /// <param name="receiveBuffer"></param>
        /// <param name="sizeOfReceiveBuffer"></param>
        /// <param name="resultFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int ToUnicode(uint virtualKeyCodeForTranslate,
                                            uint hardwareScanCodeOfKeyForTranslate,
                                            byte[] currentKeyboardState,
                                            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
                                            StringBuilder receiveBuffer,
                                            int sizeOfReceiveBuffer,
                                            uint resultFlags);

        /// <summary>
        /// Copies the status of the 256 virtual keys to the specified buffer.
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646299(v=vs.85).aspx
        /// </remarks>
        /// <param name="keyStatus"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] keyStatus);

        /// <summary>
        /// Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code.
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646306(v=vs.85).aspx
        /// </remarks>
        /// <param name="virtualKeyCode"></param>
        /// <param name="translationType"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint virtualKeyCode, MapType translationType);
    }
}