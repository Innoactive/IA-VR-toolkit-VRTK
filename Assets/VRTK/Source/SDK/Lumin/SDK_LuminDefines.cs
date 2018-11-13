// Lumin Defines|SDK_Lumin|001
namespace VRTK
{
    using System;
    /// <summary>
    /// Handles all the scripting define symbols for the Lumin SDK.
    /// </summary>
    public static class SDK_LuminDefines
    {
        /// <summary>
        /// The scripting define symbol for the Lumin SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_LUMIN";

        private const string BuildTargetGroupName = "Lumin";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        private static bool IsLuminAvailable()
        {
            Type mlDevice = VRTK_SharedMethods.GetTypeUnknownAssembly("UnityEngine.XR.MagicLeap.MagicLeapDevice");

            if (mlDevice == null)
            {
                return false;
            }
            
            return true;
        }
    }
}
