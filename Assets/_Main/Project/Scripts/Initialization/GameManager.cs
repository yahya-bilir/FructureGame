using UnityEngine;
using VContainer.Unity;

namespace Initialization
{
    public class GameManager : IStartable
    {
        public void Start()
        {
            Application.targetFrameRate = 60;
#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = false;
#endif
        }
    }
}