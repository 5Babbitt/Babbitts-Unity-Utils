using UnityEngine;

namespace BabbittsUnityUtils
{
    public class Bootstrapper : PersistantSingleton<Bootstrapper>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            DontDestroyOnLoad(Instantiate(Resources.Load("Systems")));
        }
    }
}

