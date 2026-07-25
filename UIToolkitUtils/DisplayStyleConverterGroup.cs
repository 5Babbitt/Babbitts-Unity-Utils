using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DisplayStyleConverterGroup
{
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
    public static void RegisterConverters()
    {
        var boolToDisplayGroup = new ConverterGroup("Bool to Display");
        var invertedBoolToDisplayGroup = new ConverterGroup("Invert Bool to Display");
        var stringToDisplayGroup = new ConverterGroup("String to Display");
        
        boolToDisplayGroup.AddConverter((ref bool value) => 
            new StyleEnum<DisplayStyle>((value) ? DisplayStyle.Flex : DisplayStyle.None));
        
        invertedBoolToDisplayGroup.AddConverter((ref bool value) => 
            new StyleEnum<DisplayStyle>((value) ? DisplayStyle.None : DisplayStyle.Flex));
        
        // Hides display if string is empty
        stringToDisplayGroup.AddConverter((ref string value) => 
            new StyleEnum<DisplayStyle>(string.IsNullOrEmpty(value) ? DisplayStyle.None : DisplayStyle.Flex));
        
        ConverterGroups.RegisterConverterGroup(boolToDisplayGroup);
        ConverterGroups.RegisterConverterGroup(invertedBoolToDisplayGroup);
        ConverterGroups.RegisterConverterGroup(stringToDisplayGroup);
    }
}