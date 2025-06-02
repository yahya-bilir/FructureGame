using Lofelt.NiceVibrations;
using UnityEngine;

namespace Utilities.Vibrations
{
    public class HapticManager : MonoBehaviour
    {
        private void Awake()
        {
            HapticController.Init();
            HapticController.hapticsEnabled = true;
        }

        public static void Haptic(HapticPatterns.PresetType hapticType)
        {
            //if(PlayerPrefs.GetInt("Vibration", 0) == 0) return;
            HapticPatterns.PlayPreset(hapticType);
#if UNITY_EDITOR
            Debug.Log($"{hapticType} Triggered!");
#endif
        }
    }
}
