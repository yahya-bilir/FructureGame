using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private Image cursorImage;
        [SerializeField] private float scaleDuration = 0.2f;

        private void Update()
        {
            // Mouse pozisyonunu takip et
            transform.position = Input.mousePosition;
            
            // Z tuşu: aç + scale animasyonu
            if (Input.GetKeyDown(KeyCode.Z))
            {
                cursorImage.enabled = true;

                // Scale önce %80'e çekilsin, sonra %100'e animasyonla büyüsün
                cursorImage.transform.localScale = Vector3.one * 0.8f;
                cursorImage.transform.DOScale(1f, scaleDuration).SetEase(Ease.OutBack);
            }

            // X tuşu: kapat
            if (Input.GetKeyDown(KeyCode.X))
            {
                cursorImage.enabled = false;
            }
        }
    }
}