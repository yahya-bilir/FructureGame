using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace Utils
{
    public class RateChanger : MonoBehaviour
    {
        [SerializeField] private ParticleSystem targetParticleSystem;
        [SerializeField] private float fadeDuration = 2f;

        private float _initialRate;
        private bool _isFading = false;

        [Button("Rate Over Time'ı Zamanla Sıfırla")]
        private void FadeOutRateOverTime()
        {
            if (targetParticleSystem == null)
            {
                Debug.LogWarning("ParticleSystem atanmadı!");
                return;
            }

            if (!_isFading)
            {
                FadeRoutine(); // Forget() yok çünkü dönüş tipi UniTaskVoid
            }
        }

        private async UniTaskVoid FadeRoutine()
        {
            _isFading = true;

            var emission = targetParticleSystem.emission;
            _initialRate = emission.rateOverTime.constant;

            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                float newRate = Mathf.Lerp(_initialRate, 0f, t);
                emission.rateOverTime = newRate;
                await UniTask.Yield();
            }

            emission.rateOverTime = 0f;
            _isFading = false;
        }

        [Button("Rate Over Time'ı Eski Haline Getir")]
        private void RestoreOriginalRate()
        {
            if (targetParticleSystem == null)
            {
                Debug.LogWarning("ParticleSystem atanmadı!");
                return;
            }

            var emission = targetParticleSystem.emission;
            emission.rateOverTime = _initialRate;
        }
    }
}