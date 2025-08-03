using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using VContainer;
using FlingTamplate.UIParticle;
using Perks.PerkActions;
using PerkSystem;
using Utils.UIComponents.UIToolkit;

namespace Gameplay.UI.InGameView
{
    public class PerkView : UIView
    {
        private VisualElement _perksHolder;
        private readonly List<VisualElement> _createdPerks = new();
        private readonly PerkViewData _perkViewData;
        private UIParticleVisualElement _particleVisualElement;
        private IObjectResolver _objectResolver;
        private IEventBus _eventBus;

        public PerkView(VisualElement root, PerkViewData perkViewData) : base(root)
        {
            _perkViewData = perkViewData;
        }

        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus)
        {
            _objectResolver = resolver;
            _eventBus = eventBus;
            SetAfterInjection();
        }

        private void SetAfterInjection()
        {
            _eventBus.Subscribe<OnLevelUpgraded>(CreatePerks);
            _objectResolver.Inject(_particleVisualElement);
        }
        protected override void SetVisualElements()
        {
            _perksHolder = _rootElement.Q("PerksHolder");
            _particleVisualElement = _rootElement.Query<UIParticleVisualElement>();
            _particleVisualElement.style.display = DisplayStyle.None;
        }

        protected override void RegisterButtonCallbacks() { }

        private void OnPerkSelected(ClickEvent evt, PerkAction perkAction)
        {
            Time.timeScale = 1;
            _createdPerks.ForEach(x =>
            {
                x.pickingMode = PickingMode.Ignore;
                x.RemoveFromClassList("perk");
                x.AddToClassList("perkNotHover");
            });
            VisualElement target = (VisualElement)evt.currentTarget;
            FlipAnimation(target).Forget();
            PlayParticle(target);

            perkAction.Execute();
        }

        private void PlayParticle(VisualElement target)
        {
            _particleVisualElement.style.display = DisplayStyle.Flex;
            _particleVisualElement.BringToFront();
            Vector2 targetWorldPos = target.worldBound.position;
            Vector2 targetLocalPos = _particleVisualElement.parent.WorldToLocal(targetWorldPos);
            Vector2 particleSize = _particleVisualElement.worldBound.size;
            Vector2 targetSize = target.worldBound.size;
            Vector2 offset = (particleSize - targetSize) / 2f;

            _particleVisualElement.style.position = Position.Absolute;
            _particleVisualElement.style.left = targetLocalPos.x + offset.x;
            _particleVisualElement.style.top = targetLocalPos.y + offset.y;
        }

        private void CreatePerks(OnLevelUpgraded eventData)
        {
            var perks = eventData.PerkActions;
            _particleVisualElement.style.display = DisplayStyle.None;
            ClearCreatedPerks();

            foreach (var perkAction in perks)
            {
                VisualElement perkVisualElement = _perkViewData.PerkAsset.CloneTree();
                perkVisualElement.pickingMode = PickingMode.Ignore;
                perkVisualElement.AddToClassList("perk--container");

                Label name = perkVisualElement.Q<Label>("PerkNameLabel");
                name.style.color = perkAction.NameColor;
                name.text = perkAction.PerkName;

                Label info = perkVisualElement.Q<Label>("PerkInfoLabel");
                info.text = perkAction.Description;

                VisualElement icon = perkVisualElement.Q<VisualElement>("Icon");
                icon.style.backgroundImage = new StyleBackground(perkAction.Icon);

                VisualElement background = perkVisualElement.Q<VisualElement>("Container");
                background.style.backgroundImage = new StyleBackground(perkAction.Background);

                var perkRoot = perkVisualElement.Q("PerkVertical");
                perkRoot.RegisterCallback<ClickEvent>((x) => OnPerkSelected(x, perkAction));

                _perksHolder.Add(perkVisualElement);
                _createdPerks.Add(perkRoot);
            }

            Show();
            PerksAnimation().Forget();
        }

        private void ClearCreatedPerks()
        {
            _createdPerks.ForEach(x => x.parent?.RemoveFromHierarchy());
            _createdPerks.Clear();
        }

        public override void Hide() => HideWithAnimation();
        public override void Show() => ShowWithAnimation();

        private async UniTaskVoid PerksAnimation()
        {
            _createdPerks.ForEach(x => x.transform.scale = new Vector2(0.3f, .3f));
            await UniTask.DelayFrame(1);
            for (int i = 0; i < _createdPerks.Count; i++)
            {
                var crr = _createdPerks[i];
                crr.experimental.animation.Scale(1.2f, 300).OnCompleted(() =>
                {
                    crr.experimental.animation.Scale(1f, 100);
                });
                await UniTask.Delay(50);
            }

            Time.timeScale = 0;
        }

        public async UniTask FlipAnimation(VisualElement element, float duration = 0.3f, float rotationSpeed = 3f)
        {
            float scaleUp = 1.3f;
            float time = 0f;
            var originalScale = element.transform.scale;
            element.experimental.animation.Scale(scaleUp, 200).Ease(Easing.OutBounce);
            await UniTask.Delay(210);

            int rotations = 2;
            float totalRadians = rotations * Mathf.PI;
            float scale = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float progress = time / duration;
                float angle = progress * totalRadians;
                scale = Mathf.Cos(angle);
                element.transform.scale = new Vector2(scale * scaleUp, scaleUp);
                await UniTask.Yield();
            }

            element.experimental.animation.Scale(1f, 200).Ease(Easing.OutBounce);
            element.transform.scale = originalScale;
            await UniTask.Delay(200);
            Hide();
        }

        public override void Dispose()
        {
            base.Dispose();
            _eventBus.Unsubscribe<OnLevelUpgraded>(CreatePerks);
        }
    }

    [System.Serializable]
    public class PerkViewData
    {
        public VisualTreeAsset PerkAsset;
    }
}
