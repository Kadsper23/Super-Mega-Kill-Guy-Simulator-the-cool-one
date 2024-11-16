using System.Collections.Generic;
using UnityEditor;

namespace Mirror
{
    static class PreprocessorDefine
    {
        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        [InitializeOnLoadMethod]
        public static void AddDefineSymbols()
        {
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            HashSet<string> defines = new HashSet<string>(currentDefines.Split(';'))
            {
                "MIRROR",
<<<<<<< Updated upstream
                "MIRROR_1726_OR_NEWER",
                "MIRROR_3_0_OR_NEWER",
                "MIRROR_3_12_OR_NEWER",
                "MIRROR_4_0_OR_NEWER",
                "MIRROR_5_0_OR_NEWER",
                "MIRROR_6_0_OR_NEWER",
                "MIRROR_7_0_OR_NEWER",
                "MIRROR_8_0_OR_NEWER",
                "MIRROR_9_0_OR_NEWER",
                "MIRROR_10_0_OR_NEWER",
                "MIRROR_11_0_OR_NEWER",
                "MIRROR_12_0_OR_NEWER",
                "MIRROR_13_0_OR_NEWER",
                "MIRROR_14_0_OR_NEWER",
                "MIRROR_15_0_OR_NEWER",
                "MIRROR_16_0_OR_NEWER",
=======
>>>>>>> Stashed changes
                "MIRROR_17_0_OR_NEWER",
                "MIRROR_18_0_OR_NEWER",
                "MIRROR_24_0_OR_NEWER",
                "MIRROR_26_0_OR_NEWER",
                "MIRROR_27_0_OR_NEWER",
                "MIRROR_28_0_OR_NEWER",
                "MIRROR_29_0_OR_NEWER",
                "MIRROR_30_0_OR_NEWER",
                "MIRROR_30_5_2_OR_NEWER",
                "MIRROR_32_1_2_OR_NEWER",
<<<<<<< Updated upstream
                "MIRROR_32_1_4_OR_NEWER"
=======
                "MIRROR_32_1_4_OR_NEWER",
                "MIRROR_35_0_OR_NEWER",
                "MIRROR_35_1_OR_NEWER",
                "MIRROR_37_0_OR_NEWER",
                "MIRROR_38_0_OR_NEWER",
                "MIRROR_39_0_OR_NEWER",
                "MIRROR_40_0_OR_NEWER",
                "MIRROR_41_0_OR_NEWER",
                "MIRROR_42_0_OR_NEWER",
                "MIRROR_43_0_OR_NEWER",
                "MIRROR_44_0_OR_NEWER",
                "MIRROR_46_0_OR_NEWER",
                "MIRROR_47_0_OR_NEWER",
                "MIRROR_53_0_OR_NEWER",
                "MIRROR_55_0_OR_NEWER",
                "MIRROR_57_0_OR_NEWER",
                "MIRROR_58_0_OR_NEWER",
                "MIRROR_65_0_OR_NEWER",
                "MIRROR_66_0_OR_NEWER"
>>>>>>> Stashed changes
            };

            // only touch PlayerSettings if we actually modified it.
            // otherwise it shows up as changed in git each time.
            string newDefines = string.Join(";", defines);
            if (newDefines != currentDefines)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
            }
        }
    }
}
