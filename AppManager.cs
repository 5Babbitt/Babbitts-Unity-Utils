using UnityEditor;
using UnityEngine;

namespace BabbittsUnityUtils
{
    public class AppManager : Singleton<AppManager>
    {
        public void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}