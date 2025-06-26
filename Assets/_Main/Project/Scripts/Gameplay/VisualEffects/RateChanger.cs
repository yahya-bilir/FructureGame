using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VisualEffects
{
    public class RateChanger : MonoBehaviour
    {
        [SerializeField] private ParticleSystem targetParticleSystem;
        [SerializeField] private float fadeDuration = 2f;

        private float _initialRate;
        private bool _isFading = false;

        [Button("Rate Over Time'ı Zamanla Sıfırla")]
        public void FadeOutRateOverTime()
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
            var main = targetParticleSystem.main;
            var emission = targetParticleSystem.emission;

            main.startLifetime = 0.1f;
            emission.rateOverTime = 0;

            // Color Over Lifetime
            var colorOverLifetime = targetParticleSystem.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0.0f),
                    new GradientColorKey(Color.white, 1.0f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f),
                    new GradientAlphaKey(0.0f, 1.0f)
                });
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);

            // Size Over Lifetime
            var sizeOverLifetime = targetParticleSystem.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 1.0f);
            curve.AddKey(1.0f, 0.0f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
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