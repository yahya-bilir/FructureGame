using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Utils.UIComponents.UIToolkit
{
    public abstract class UIView : IDisposable
    {
        protected bool _hideOnAwake = true;
        protected VisualElement _rootElement;
        public VisualElement Root => _rootElement;
        public bool IsHidden => _rootElement.style.display == DisplayStyle.None;

        public UIView(VisualElement rootElement)
        {
            _rootElement = rootElement;
            Initialize();
        }

        private void Initialize()
        {
            if (_hideOnAwake) Hide();
            SetVisualElements();
            RegisterButtonCallbacks();
        }

        protected abstract void SetVisualElements();


        protected abstract void RegisterButtonCallbacks();


        public virtual void Show()
        {
            _rootElement.style.display = DisplayStyle.Flex;
        }


        public virtual void Hide()
        {
            _rootElement.style.display = DisplayStyle.None;
        }

        public virtual void Dispose()
        {
        }

        public void ShowWithAnimation()
        {
            _rootElement.style.opacity = 0f;
            _rootElement.style.scale = new Scale(new Vector2(0.8f, 0.8f));
            _rootElement.style.display = DisplayStyle.Flex;

            _rootElement.experimental.animation
                .Start(new StyleValues { opacity = 1f }, 150);

            _rootElement.experimental.animation.Scale(1.1f, 200).OnCompleted(() =>
            {
                _rootElement.experimental.animation.Scale(1f, 200);
            });
        }

        public void HideWithAnimation()
        {
            _rootElement.style.opacity = 1f;
            _rootElement.style.scale = new Scale(new Vector2(1f, 1f));
            _rootElement.style.display = DisplayStyle.Flex;

            _rootElement.experimental.animation
                .Start(new StyleValues { opacity = 0f }, 150);

            _rootElement.experimental.animation.Scale(1.1f, 200).OnCompleted(() =>
            {
                _rootElement.experimental.animation.Scale(.8f, 200);
            }).OnCompleted(() => _rootElement.style.display = DisplayStyle.None);
        }
    }
}