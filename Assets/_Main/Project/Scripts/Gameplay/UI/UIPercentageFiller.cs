using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPercentageFiller : MonoBehaviour
    {
        [SerializeField] private Image fillableImage;
        [SerializeField] private bool isReversed;
        private  float _currentValue;
        private void Awake()
        {
            var initialValue = isReversed ? 100f : 0f;
            fillableImage.fillAmount = initialValue / 100f;
            _currentValue = fillableImage.fillAmount * 100;
          
            SetUIPercentage(initialValue);
            DisableOrEnableObjectsVisibility(false);
        }

        public void SetUIPercentage(float percentage)
        {
            if (percentage is < 0 or > 100) return;
            //percentageText.text = $"{percentageToInt}%";
            fillableImage.fillAmount = percentage / 100f;
        }

        public void DisableOrEnableObjectsVisibility(bool visibility)
        {
           if (visibility == false) DOTween.Kill(GetHashCode());
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(visibility);
            }
        }

        public void ReturnToGivenValue(float value = 100)
        {
            DOTween.Kill(GetHashCode());
            var tween = DOTween.Sequence();
          
                tween.Append(DOVirtual.Float(fillableImage.fillAmount * 100, value, 2f, i => {
                        _currentValue = i;
                        fillableImage.fillAmount = _currentValue /100;
                        SetUIPercentage(i);
                }));
            
        
            tween.OnComplete(() => DisableOrEnableObjectsVisibility(false));
            tween.SetId(GetHashCode());
        }
    }
}
