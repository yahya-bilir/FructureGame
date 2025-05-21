using DG.Tweening;
using UnityEngine;

namespace Characters.Player
{
    public class WeaponAreaSpinning : MonoBehaviour
    {
        private void Awake()
        {
            transform.DOLocalRotate(
                    new Vector3(0f, 0f, -360),   // 360 derece Z ekseninde
                    1f,                          // 1 saniyede tamamla
                    RotateMode.FastBeyond360     // 360'ı geçmesine izin ver
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental); // Sonsuz döngü
        
        }
    }
}