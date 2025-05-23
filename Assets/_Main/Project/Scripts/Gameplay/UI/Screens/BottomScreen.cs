using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.UIComponents.UIToolkit;

namespace UI.Screens
{
    public class BottomScreen : UIView
    {
        private VisualElement _shineElement;
        private VisualElement _imageContainer;
        private bool _increasing = true;

        public BottomScreen(VisualElement rootElement) : base(rootElement)
        {
        }

        protected override void SetVisualElements()
        {
            _shineElement = _rootElement.Q("ImageShine");
            _imageContainer = _rootElement.Q("ImageContainer");
            
            Show();
        }

        protected override void RegisterButtonCallbacks()
        {
        }
        
        public void ContinuesShineAnim()
        {
            DOVirtual.Float(97f, 103f, 0.5f, value =>
                {
                    _shineElement.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(new Length(value, LengthUnit.Percent), new Length(value, LengthUnit.Percent)));
                })
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            
            ScaleUp().Forget();
        }

        private async UniTask ScaleUp()
        {
            var initialImageContainer = _imageContainer.resolvedStyle.width;
            var targetSize = initialImageContainer * 1.2f;

            _imageContainer.experimental.animation
                .Size(new Vector2(targetSize, targetSize), 600)
                //.Ease(t => t * t) // InQuad easing
                .OnCompleted(() =>
                {
                    _imageContainer.experimental.animation
                        .Size(new Vector2(initialImageContainer, initialImageContainer), 600)
                        .Ease(t => 1 - Mathf.Pow(2, -10 * t)); // OutExpo easing
                });

            await UniTask.Yield(); // Asenkron yapıya uygunluk için eklenmiştir
        }
    }
}