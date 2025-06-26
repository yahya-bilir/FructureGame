using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VisualEffects
{
    public class IslandOpener : MonoBehaviour
    {
        private Scaler _scaler;
        private RateChanger _rateChanger;
        private void Awake()
        {
            _scaler = GetComponentInChildren<Scaler>();
            _rateChanger = GetComponentInChildren<RateChanger>();
        }

        [Button]
        private void CallIslandOpener()
        {
            OpenIslandUp().Forget();
        }

        private async UniTask OpenIslandUp()
        {
            _rateChanger.FadeOutRateOverTime();
            await UniTask.WaitForSeconds(1f);
            _scaler.ScaleUpInOrder();
        }
    }
}
