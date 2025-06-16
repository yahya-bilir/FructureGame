using UnityEngine;
using VContainer.Unity;

namespace Initialization
{
    public class GameManager : IStartable
    {
        public void Start()
        {
            Application.targetFrameRate = 60;
            Debug.Log("Logging check");
#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = false;
#endif
        }
    }
}