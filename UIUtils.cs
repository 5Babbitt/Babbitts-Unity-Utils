using UnityEngine;

namespace BabbittsUnityUtils
{ 
    public static class UIUtils
    {
        public static void CopyToClipboard(this string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }

        public static string PasteFromClipboard()
        {
            return GUIUtility.systemCopyBuffer;
        }
    }
}
