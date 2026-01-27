using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace BabbittsUnityUtils
{
    public class Bootstrapper : PersistantSingleton<Bootstrapper>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            var systemsPrefab = Resources.Load("Systems");

            if (!systemsPrefab)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[BabbittsUnityUtils] Systems prefab not found. Creating default setup...");
                CreateDefaultSystemsSetup();
                systemsPrefab = Resources.Load("Systems");
#endif               
            }

            if (systemsPrefab != null)
            {
                DontDestroyOnLoad(Instantiate(systemsPrefab));
            }
        }

#if UNITY_EDITOR
        private static void CreateDefaultSystemsSetup()
        {
            // Create Resources folder if it doesn't exist
            string resourcesPath = "Assets/Resources";
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                AssetDatabase.Refresh();
                Debug.Log($"[BabbittsUnityUtils] Created Resources folder at {resourcesPath}");
            }

            // Create Systems prefab
            string prefabPath = $"{resourcesPath}/Systems.prefab";
            if (!File.Exists(prefabPath))
            {
                GameObject systemsObject = new GameObject("Systems");

                // Save as prefab
                PrefabUtility.SaveAsPrefabAsset(systemsObject, prefabPath);
                DestroyImmediate(systemsObject);

                AssetDatabase.Refresh();
                Debug.Log($"[BabbittsUnityUtils] Created Systems prefab at {prefabPath}");
                Debug.Log("[BabbittsUnityUtils] Please add your system components to the Systems prefab in the Resources folder.");
            }
        }
#endif
    }
}

