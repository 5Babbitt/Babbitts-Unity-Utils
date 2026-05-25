using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundTextureConverterGroup
{
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
    public static void RegisterConverters()
    {
        var texture2DToStyleBackground = new ConverterGroup("Texture2D to StyleBackground");
        
        texture2DToStyleBackground.AddConverter((ref Texture2D texture) =>
            texture != null ? new StyleBackground(texture) : new StyleBackground(StyleKeyword.None));
        
        ConverterGroups.RegisterConverterGroup(texture2DToStyleBackground);
    }
}